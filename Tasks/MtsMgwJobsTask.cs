using System;
using System.Collections.Generic;
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
    public class MtsMgwJobsTask : IRunnable
    {
        #region Field
        private IHubClients Clients { get; }
        private readonly IMemoryCache _cache;
        private readonly IWebbrSsh _webbrSsh;
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public MtsMgwJobsTask(IHubContext<WebbrHub> context, IWebbrDatabase webbrDatabase, IMemoryCache memoryCache, IWebbrSsh webbrSsh)
        {
            Clients = context.Clients;
            _cache = memoryCache;
            _webbrSsh = webbrSsh;
            _webbrDatabase = webbrDatabase;
        }
        #endregion


        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            await CheckMgwJobs();
        }


        #region CheckMgwJobs
        private async Task CheckMgwJobs()
        {
            var oracleList = await _webbrDatabase.QueryAsync<dynamic>($"SELECT * FROM configuration_variables_mts_mgw_oracle WHERE enabled='1'");
            var mgwJobsList = new List<MgwJobsDbModel>();

            oracleList.ForEach(async x =>
            {
                var queryResult = await _webbrDatabase.OracleQueryAsyncConnection<MgwJobsDbModel>(
                    $"user id={x.user_id};password={x.password};data source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST={x.ip})(PORT={x.port}))(CONNECT_DATA=(SERVICE_NAME={x.service})))", 
                    @"SELECT AGENT_NAME, ENABLED, SOURCE, PROPAGATED_MSGS, STATUS, SysTimeStamp SYSTIME, JOB_NAME, PROPAGATION_TYPE, DESTINATION, LINK_NAME, LAST_ERROR_MSG, LAST_ERROR_DATE FROM mgw_jobs");
                
                mgwJobsList.AddRange(queryResult);
            });

            _cache.Set("dashboard_mts_jobs", mgwJobsList, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            await Clients.All.SendAsync("dashboard_mts_jobs", mgwJobsList);

            await _webbrDatabase.ExecuteAsync("TRUNCATE TABLE dashboard_main_mts_mgw_job");
            
            const string transactionQuery = @"
INSERT dashboard_main_mts_mgw_job (AGENT_NAME, ENABLED, SOURCE, PROPAGATED_MSGS, STATUS, SYSTIME, JOB_NAME, PROPAGATION_TYPE, DESTINATION, LINK_NAME, LAST_ERROR_MSG, LAST_ERROR_DATE, UPDATED)
VALUES(@AGENT_NAME, @ENABLED, @SOURCE, @PROPAGATED_MSGS, @STATUS, @SYSTIME, @JOB_NAME, @PROPAGATION_TYPE, @DESTINATION, @LINK_NAME, @LAST_ERROR_MSG, @LAST_ERROR_DATE, @UPDATED)
ON DUPLICATE KEY UPDATE AGENT_NAME=@AGENT_NAME, ENABLED=@ENABLED, SOURCE=@SOURCE, PROPAGATED_MSGS=@PROPAGATED_MSGS, STATUS=@STATUS, SYSTIME=@SYSTIME, JOB_NAME=@JOB_NAME, PROPAGATION_TYPE=@PROPAGATION_TYPE, DESTINATION=@DESTINATION, LINK_NAME=@LINK_NAME, LAST_ERROR_MSG=@LAST_ERROR_MSG, LAST_ERROR_DATE=@LAST_ERROR_DATE, UPDATED=@UPDATED";

            await _webbrDatabase.TransactionAsync(transactionQuery, mgwJobsList);
        }
        #endregion
    }
}