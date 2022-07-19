using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using RecurrentTasks;
using Webbr.Extensions;
using Webbr.Hubs;
using Webbr.Models.MtsModel;

namespace Webbr.Tasks
{
    public class MtsMgwAgentTask : IRunnable
    {
        #region Field
        private IHubClients Clients { get; }
        private readonly IMemoryCache _cache;
        private readonly IWebbrSsh _webbrSsh;
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public MtsMgwAgentTask(IHubContext<WebbrHub> context, IWebbrDatabase webbrDatabase, IMemoryCache memoryCache, IWebbrSsh webbrSsh)
        {
            Clients = context.Clients;
            _cache = memoryCache;
            _webbrSsh = webbrSsh;
            _webbrDatabase = webbrDatabase;
        }
        #endregion


        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            await CheckMgwAgent();
        }


        #region CheckMgwAgent
        private async Task CheckMgwAgent()
        {
            var oracleList = await _webbrDatabase.QueryAsync<dynamic>($"SELECT * FROM configuration_variables_mts_mgw_oracle WHERE enabled='1'");

            var mgwAgentList = new List<MgwAgentDbModel>();

            oracleList.ForEach(async x =>
            {
                var queryResult = await _webbrDatabase.OracleQueryAsyncConnection<MgwAgentDbModel>(
                    $"user id={x.user_id};password={x.password};data source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST={x.ip})(PORT={x.port}))(CONNECT_DATA=(SERVICE_NAME={x.service})))", 
                    "SELECT * FROM MGW_GATEWAY Where AGENT_USER = 'MGWAGENT'");
                mgwAgentList.Add(queryResult.First());
                
                const string transactionQuery = @"
INSERT dashboard_main_mts_mgw_agent (AGENT_NAME, AGENT_STATUS, AGENT_PING, AGENT_JOB, AGENT_USER, AGENT_DATABASE, AGENT_INSTANCE, AGENT_START_TIME, MAX_CONNECTIONS, MAX_MEMORY, MAX_THREADS, LAST_ERROR_DATE, LAST_ERROR_TIME, LAST_ERROR_MSG, CONNTYPE, SERVICE, INITFILE, COMMENTS, UPDATED)
VALUES(@AGENT_NAME, @AGENT_STATUS, @AGENT_PING, @AGENT_JOB, @AGENT_USER, @AGENT_DATABASE, @AGENT_INSTANCE, @AGENT_START_TIME, @MAX_CONNECTIONS, @MAX_MEMORY, @MAX_THREADS, @LAST_ERROR_DATE, @LAST_ERROR_TIME, @LAST_ERROR_MSG, @CONNTYPE, @SERVICE, @INITFILE, @COMMENTS, @UPDATED)
ON DUPLICATE KEY UPDATE AGENT_STATUS=@AGENT_STATUS, AGENT_PING=@AGENT_PING, AGENT_JOB=@AGENT_JOB, AGENT_USER=@AGENT_USER, AGENT_DATABASE=@AGENT_DATABASE, AGENT_INSTANCE=@AGENT_INSTANCE, AGENT_START_TIME=@AGENT_START_TIME, MAX_CONNECTIONS=@MAX_CONNECTIONS, MAX_MEMORY=@MAX_MEMORY, MAX_THREADS=@MAX_THREADS, LAST_ERROR_DATE=@LAST_ERROR_DATE, LAST_ERROR_TIME=@LAST_ERROR_TIME, LAST_ERROR_MSG=@LAST_ERROR_MSG, CONNTYPE=@CONNTYPE, SERVICE=@SERVICE, INITFILE=@INITFILE, COMMENTS=@COMMENTS, UPDATED=@UPDATED";
                await _webbrDatabase.TransactionAsync(transactionQuery, mgwAgentList);
            });
            
            _cache.Set("dashboard_mts_agent", mgwAgentList, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            await Clients.All.SendAsync("dashboard_mts_agent", mgwAgentList);
        }
        #endregion
    }
}