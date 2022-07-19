using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ahd.Graphite.Exceptions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RecurrentTasks;
using Webbr.Extensions;
using Webbr.Hubs;
using Webbr.Models.TasksModel;

namespace Webbr.Tasks
{
    public class ZabbixTask : IRunnable
    {
        #region Field
        private IHubClients Clients { get; }
        private readonly IMemoryCache _cache;
        private readonly IWebbrDatabase _webbrDatabase;
        private static readonly HttpClient StaticClient = new HttpClient();
        #endregion

        #region Constructor
        public ZabbixTask(IWebbrDatabase webbrDatabase, IMemoryCache memoryCache, IHubContext<WebbrHub> context)
        {
            _cache = memoryCache;
            Clients = context.Clients;
            _webbrDatabase = webbrDatabase;
        }
        #endregion


        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            await ZabbixData();
        }


        #region ZabbixData
        private async Task ZabbixData()
        {
            var zabbixConnectionString = await _webbrDatabase.QueryAsync<ZabbixAuthModel>("SELECT username, password, url FROM configuration_variables_zabbix");
            
            var username = zabbixConnectionString.First().username;
            var password = zabbixConnectionString.First().password;
            var url = zabbixConnectionString.First().url;

            var requestauth = new
            {
                jsonrpc = "2.0",
                method = "user.login",
                @params = new // @ потому что params 'ключевое' слово
                {
                    user = $"{username}",
                    password = $"{password}",
                },
                id = 1
            };

            using (var responseauth = await StaticClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(requestauth), Encoding.UTF8, "application/json")))
            {
                if (responseauth.IsSuccessStatusCode)
                {
                    var resultAuth = await responseauth.Content.ReadAsStringAsync();
                    var auth = JsonConvert.DeserializeObject<Auth>(resultAuth);
                    var requestTrigger = new
                    {
                        jsonrpc = auth.Jsonrpc,
                        method = "trigger.get",
                        @params = new
                        {
                            monitored = 1,
                            active = 1,
                            only_true = 1,
                            expandData = 1,
                            expandDescription = 1,
                            min_severity = 1,

                            sortfield = "hostname",
                            output = new[] {"description", "priority", "lastchange"}
                        },
                        auth = auth.Result,
                        id = auth.Id
                    };

                    using (var responseTrigger = await StaticClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(requestTrigger), Encoding.UTF8, "application/json")).ConfigureAwait(false))
                    {
                        await responseTrigger.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);
                        var resultTrigger = await responseTrigger.Content.ReadAsStringAsync().ConfigureAwait(false);
                        
                        var triggers = JsonConvert.DeserializeObject<Triggers>(resultTrigger).Result.OrderByDescending(x => x.Priority).ThenByDescending(x => DateTime.ParseExact(x.Lastchange, "O", CultureInfo.InvariantCulture)).ToList();

                        _cache.Set("dashboard_zabbix", triggers,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                        await Clients.All.SendAsync("dashboard_zabbix", triggers);
                        
                        await _webbrDatabase.ExecuteAsync("TRUNCATE TABLE dashboard_main_zabbix_triggers");
                        const string zabbixQuery = @"INSERT dashboard_main_zabbix_triggers (hostname, description, lastchange, priority, triggerid) VALUES(@Hostname, @Description, @Lastchange, @Priority, @Triggerid)";
                        await _webbrDatabase.TransactionAsync(zabbixQuery, triggers);
                    }
                }
            }
        }
        #endregion
        
        
        // Классы требуются для конвертации из Json в List
        private class Auth
        {
            public string Jsonrpc { get; set; }
            public string Result { get; set; }
            public int Id { get; set; }
        }
        private class Triggers
        {

            public string Jsonrpc { get; set; }
            public List<Result> Result { get; set; }
            public int Id { get; set; }
        }
        private class Result
        {
            private string _lastchange;
            private string _hostname;

            public string Triggerid { get; set; }

            public string Description { get; set; }
            public string Priority { get; set; }

            public string Lastchange
            {
                get => new DateTime(1970, 1, 1).AddSeconds(Convert.ToDouble(_lastchange)).ToLocalTime().ToString(@"O");
                set => _lastchange = value;
            }

            public string Hostname
            {
                get
                {
                    var val = new Regex("(_\\(.*?\\))", RegexOptions.IgnoreCase).Match(_hostname).Value;
                    if(val.Length != 0) return _hostname.Replace(val,string.Empty);
                    return _hostname;
                }
                set => _hostname = value;
            }
        }
    }
}