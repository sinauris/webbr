using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ahd.Graphite;
using AngleSharp;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using RecurrentTasks;
using Webbr.Extensions;
using Webbr.Hubs;
using Webbr.Models;
using Webbr.Models.TasksModel;

namespace Webbr.Tasks
{
    public class NaumenLicenseTask : IRunnable
    {
        #region Field
        private IHubClients Clients { get; }
        private readonly IMemoryCache _cache;
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public NaumenLicenseTask(IWebbrDatabase webbrDatabase, IMemoryCache memoryCache, IHubContext<WebbrHub> context)
        {
            _cache = memoryCache;
            Clients = context.Clients;
            _webbrDatabase = webbrDatabase;
        }
        #endregion

        
        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            await ParseLicensePage();
        }


        #region ParseLicensePage
        private async Task ParseLicensePage()
        {
            var naumenConnectionString = await _webbrDatabase.QueryAsync<NaumenAuthModel>("SELECT username, password, url FROM configuration_variables_naumen");
            
            var username = naumenConnectionString.First().username;
            var password = naumenConnectionString.First().password;
            var url = naumenConnectionString.First().url;
            
            var config = Configuration.Default.WithDefaultLoader().WithCookies();
            using (var doc = await BrowsingContext.New(config).OpenAsync(url))
            {
                var form = doc.Forms[0];

                foreach (var element in doc.Forms[0].QuerySelectorAll("table > tbody > tr > td > input"))
                {
                    if (element.GetAttribute("type") == "text") element.SetAttribute("value", username);
                    else if (element.GetAttribute("type") == "password") element.SetAttribute("value", password);
                }
                await form.SubmitAsync();

                var docSecond = await BrowsingContext.New(config).OpenAsync($"{url}");

                var menuItems = docSecond.GetElementById("License.Licenses");
                var tableRows = menuItems.QuerySelectorAll("table > tbody > tr");

                var naumenLicenseList = new List<NaumenLicenseParseModel>();

                var graphiteClient = new GraphiteClient("10.254.6.86") {HttpApiPort = 80, UseSsl = false};
                ICollection<Datapoint> graphiteDatapoints = new List<Datapoint>();

                foreach (var element in tableRows.Skip(1))
                {
                    var licenseName = Regex.Replace(element.ChildNodes[1].TextContent,@"^\d.\d.\s\w\w\w\s", string.Empty);
                    var graphiteName = licenseName
                        .Replace(" ", "_")
                        .Replace("-", "_")
                        .Replace("(", "")
                        .Replace(")", "")
                        .Replace("/", "_");

                    var licenseUse = Convert.ToInt32(element.ChildNodes[3].TextContent);
                    var licenseAll = Convert.ToInt32(element.ChildNodes[2].TextContent);

                    var naumenParseLicenseModel = new NaumenLicenseParseModel
                    {
                        license_name = licenseName,
                        license_use = licenseUse,
                        license_all = licenseAll,
                    };
                    naumenLicenseList.Add(naumenParseLicenseModel);

                    // Добавляем в список Datapoint для отправки в Graphite
                    graphiteDatapoints.Add(new Datapoint($"License_63_Main.{graphiteName}", licenseUse, DateTime.Now));
                }

                // Отправляем в Graphite
                await graphiteClient.SendAsync(graphiteDatapoints);
                
                doc.Close();
                doc.Dispose();

                var naumenHostsList = await _webbrDatabase.QueryAsync<GrafanaNaumenHosts>("SELECT d.id, d.naumen, d.hosts_count, p.place_description, d.updated FROM dashboard_main_grafana_naumen_hosts d LEFT JOIN configuration_variables_places p ON p.placeid = d.placeid");
                var naumenHostsListOrder = naumenHostsList.OrderBy(x => x.Place_description);
                var naumenParseLicenseList = await _webbrDatabase.QueryAsync<NaumenLicenseParseModel>("SELECT license_name, license_use, license_all, license_description FROM dashboard_main_naumen_licenses");
                var naumenParseLicenseListEdit = naumenParseLicenseList.Where(x => x.license_all != 0 && x.license_all != 1 && x.license_use != 0).OrderBy(x => x.license_name);

                var dict = new Dictionary<string, dynamic>{{"naumenHostsList", naumenHostsListOrder}, {"naumenLicenseList", naumenParseLicenseListEdit}};
                
                _cache.Set("dashboard_naumen", dict, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                await Clients.All.SendAsync("dashboard_naumen", dict);
                
                const string transactionQuery = "INSERT dashboard_main_naumen_licenses (license_name, license_use, license_all) VALUES(@license_name, @license_use, @license_all) ON DUPLICATE KEY UPDATE license_name=@license_name, license_use=@license_use, license_all=@license_all";
                await _webbrDatabase.TransactionAsync(transactionQuery, naumenLicenseList);
            }
        }
        #endregion
    }
}
