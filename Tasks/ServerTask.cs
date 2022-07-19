using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RecurrentTasks;
using RunProcessAsTask;
using Webbr.Extensions;
using Webbr.Models.ServerModel;
using Webbr.Models.TasksModel;

namespace Webbr.Tasks
{
    public class ServerTask : IRunnable
    {
        #region Fields
        private readonly IWebbrSsh _webbrSsh;
        private readonly IMemoryCache _cache;
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public ServerTask(IWebbrDatabase webbrDatabase, IWebbrSsh webbrSsh, IMemoryCache memoryCache)
        {
            _webbrSsh = webbrSsh;
            _cache = memoryCache;
            _webbrDatabase = webbrDatabase;
        }
        #endregion
        

        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            var configurationVariablesHypervisors = await _webbrDatabase.QueryAsync<HypervisorsModel>($@"SELECT hypervisor, ip, username, password FROM configuration_variables_hypervisors WHERE enable=1");
            var configurationVariablesHypervisorsList = configurationVariablesHypervisors.ToList();
            
            var tasks = configurationVariablesHypervisorsList.Select(async i =>
            {
                if(i.hypervisor == "vSphere") await VSphere(i.ip, i.username, i.password);
            });
            
            await Task.WhenAll(tasks);

            const string query = "SELECT vmUuid, hypervisorIp, vmCluster, vmName, vmGuestos, vmState, vmHost, vmAnnotation, vmIpaddress, vmDnsname, vmCpu, vmMemory, vmHdd FROM virtualservers";
            var list = await _webbrDatabase.QueryAsync<VirtualServerModel>(query);
            if (list.Count != 0) _cache.Set("servers", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
        }

        private async Task VSphere(string ip, string username, string password)
        {
            await ProcessEx.RunAsync("/usr/bin/python3",$@"/home/asphy/Webbr/production/Scripts/vSphere/script.py -s {ip} -u {username} -p """"{password}"""" -S");
        }
    }
}