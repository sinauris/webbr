using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using RecurrentTasks;
using Webbr.Extensions;
using Webbr.Hubs;

namespace Webbr.Tasks
{
    public class MtsTunnelTask : IRunnable
    {
        #region Field
        private IHubClients Clients { get; }
        private readonly IMemoryCache _cache;
        private readonly IWebbrSsh _webbrSsh;
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public MtsTunnelTask(IHubContext<WebbrHub> context, IWebbrDatabase webbrDatabase, IMemoryCache memoryCache, IWebbrSsh webbrSsh)
        {
            Clients = context.Clients;
            _cache = memoryCache;
            _webbrSsh = webbrSsh;
            _webbrDatabase = webbrDatabase;
        }
        #endregion


        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            await CheckMqTunnel();
        }


        #region CheckMqTunnel
        private async Task CheckMqTunnel()
        {
            var mq = await _webbrDatabase.QueryAsync<dynamic>($"SELECT ip, port, tunnel_ip FROM dashboard_main_mts_mq_tunnel WHERE enabled='1'");

            foreach (var v in mq)
            {
                var mqIpArray = v.tunnel_ip.Split(";");

                var sshResult = string.Empty;
                var sshException = string.Empty;
                var pingResult = new StringBuilder();

                foreach (var ip in mqIpArray)
                {
                    try
                    {
                        var pingCommand = _webbrSsh.SshCommandExecute(v.ip, v.port, "root","ping -qc4 " + ip + " 2>&1 | awk -F'/' 'END{ print (/^rtt/? \"OK\":\"FAIL\") }'");
                        pingResult.Append($"{ip} - {pingCommand.Replace("\n", string.Empty)};");
                        sshResult = "OK";
                    }
                    catch (Exception ex)
                    {
                        pingResult.Append($"{ip} - FAIL;");
                        sshResult = "FAIL";
                        sshException = ex.Message;
                    }
                }

                await _webbrDatabase.ExecuteAsync(@"UPDATE dashboard_main_mts_mq_tunnel SET ssh_result=@ssh_result, ssh_exception=@ssh_exception, ping_result=@ping_result, updated=@updated WHERE ip=@ip", new { ssh_result=sshResult, ssh_exception = sshException, ping_result = pingResult.ToString(), updated = DateTime.Now.ToString("O"), v.ip });
            }

            var dbResult = await _webbrDatabase.QueryAsync<dynamic>("SELECT * FROM dashboard_main_mts_mq_tunnel WHERE enabled = 1");
            
            _cache.Set("dashboard_mts_tunnel", dbResult, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            await Clients.All.SendAsync("dashboard_mts_tunnel", dbResult);
        }
        #endregion
    }
}