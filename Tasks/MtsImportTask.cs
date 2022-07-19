using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using RecurrentTasks;
using Webbr.Extensions;
using Webbr.Hubs;

namespace Webbr.Tasks
{
    public class MtsImportTask : IRunnable
    {
        #region Field
        private IHubClients Clients { get; }
        private readonly IWebbrSsh _webbrSsh;
        private readonly IMemoryCache _cache;
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public MtsImportTask(IHubContext<WebbrHub> context, IWebbrDatabase webbrDatabase, IMemoryCache memoryCache, IWebbrSsh webbrSsh)
        {
            Clients = context.Clients;
            _webbrSsh = webbrSsh;
            _cache = memoryCache;
            _webbrDatabase = webbrDatabase;
        }
        #endregion


        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            await CheckImport();
        }


        #region CheckImport
        private async Task CheckImport()
        {
            var import = await _webbrDatabase.QueryAsync<dynamic>($"SELECT ip, port FROM dashboard_main_mts_import WHERE enabled='1'");

            foreach (var v in import)
            {
                var sshResult = string.Empty;
                var sshException = string.Empty;
                var importResult = string.Empty;

                try
                {
                    var command = _webbrSsh.SshCommandExecute(v.ip, v.port, "root","ps ax | grep \"/bin/sh -c /bin/bash /home/import/runner/import.sh prod import/index > /dev/null 2>&1\" | grep -v grep");
                    var commandResult = command.Replace("\n", string.Empty);
                    if (string.IsNullOrEmpty(commandResult)) importResult = "STOPPED";
                    else if (!string.IsNullOrEmpty(commandResult)) importResult = "RUNNING";
                    sshResult = ("OK");
                }
                catch (Exception ex)
                {
                    sshResult = ("FAIL");
                    sshException = ex.Message;
                }

                await _webbrDatabase.ExecuteAsync(@"UPDATE dashboard_main_mts_import SET ssh_result=@ssh_result, ssh_exception=@ssh_exception, import_result=@import_result, updated=@updated WHERE ip=@ip", new { ssh_result=sshResult, ssh_exception = sshException, import_result = importResult, updated = DateTime.Now.ToString("O"), v.ip });
            }
            
            var dbResult = await _webbrDatabase.QueryAsync<dynamic>("SELECT * FROM dashboard_main_mts_import WHERE enabled = 1");
            
            _cache.Set("dashboard_mts_import", dbResult, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            await Clients.All.SendAsync("dashboard_mts_import", dbResult);
        }
        #endregion
    }
}