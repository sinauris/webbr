using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ahd.Graphite;
using ahd.Graphite.Base;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using RecurrentTasks;
using Webbr.Extensions;
using Webbr.Hubs;
using Webbr.Models.DashboardModel;

namespace Webbr.Tasks
{
    public class GrafanaTask : IRunnable
    {
        #region Field
        private IHubClients Clients { get; }
        private readonly IMemoryCache _cache;
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public GrafanaTask(IWebbrDatabase webbrDatabase, IMemoryCache memoryCache, IHubContext<WebbrHub> context)
        {
            _cache = memoryCache;
            Clients = context.Clients;
            _webbrDatabase = webbrDatabase;
        }
        #endregion

        
        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            await GrafanaData();
        }


        #region GrafanaData
        private async Task GrafanaData()
        {
            var sections = await _webbrDatabase.QueryAsync<GrafanaSectionModel>("SELECT id, section_name FROM dashboard_main_grafana_servers_section");
            var mainArray = await _webbrDatabase.QueryAsync<GrafanaModel>("SELECT id, name, ip, `load`, load_treshold, comment, tag, section FROM webbr.dashboard_main_grafana_servers WHERE enable=1 ORDER BY id");
            var mainArrayList = mainArray.ToList();
            var tagList = mainArrayList.Select(x => x.tag).Distinct();
            var tagJoin = string.Join(",", tagList);

            var graphiteClient = new GraphiteClient("10.254.6.86") {HttpApiPort = 8085, UseSsl = false};
            var loadPath = new GraphitePath("{" + tagJoin + "}").DotWildcard().Dot("load").Dot("load").Dot("midterm");
            var loadData = await graphiteClient.GetMetricsDataAsync(loadPath, "-2min");
            
            var servers = mainArrayList.Join(loadData,
                z => z?.name.ToUpper(),
                x => x?.Target.Split(".")[1].ToUpper(),
                (z, x) => new GrafanaModel
                {
                    name = z?.name,
                    ip = z?.ip,
                    load = x?.Datapoints.LastOrDefault(c => !string.IsNullOrEmpty(c.Value.ToString()))?.Value.ToString(),
                    load_treshold = z?.load_treshold,
                    comment = z?.comment,
                    tag = z?.tag,
                    section = z.section
                }).ToList();
            
            var completeDict = new Dictionary<string, dynamic> {{"servers", servers}, {"sections", sections}};
            
            _cache.Set("dashboard_grafana", completeDict, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            await Clients.All.SendAsync("dashboard_grafana", completeDict);
            
            const string grafanaUpdateQuery = @"UPDATE dashboard_main_grafana_servers SET `load`=@load WHERE name=@name";
            await _webbrDatabase.TransactionAsync(grafanaUpdateQuery, servers);
        }
        #endregion
    }
}