using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Webbr.Extensions;
using Webbr.Jwt.Helpers;
using Webbr.Models.ServerModel;

namespace Webbr.Controllers
{
    [Authorize(Policy = Constants.JwtPolicy.Basic)]
    [Route("api/[controller]/[action]")]
    public class ServerController : Controller
    {
        #region Field
        private readonly IWebbrDatabase _webbrDatabase;
        private readonly IMemoryCache _cache;
        #endregion

        #region Constructor
        public ServerController(IWebbrDatabase webbrDatabase, IMemoryCache memoryCache)
        {
            _webbrDatabase = webbrDatabase;
            _cache = memoryCache;
        }
        #endregion


        #region GetVirtualServers
        [HttpGet]
        public async Task<IEnumerable<VirtualServerModel>> GetVirtualServers()
        {
            if (!_cache.TryGetValue("servers", out List<VirtualServerModel> list))
            {
                list = await _webbrDatabase.QueryAsync<VirtualServerModel>("SELECT vmUuid, hypervisorIp, vmCluster, vmName, vmGuestos, vmState, vmHost, vmAnnotation, vmIpaddress, vmDnsname, vmCpu, vmMemory FROM virtualservers");
                if (list.Count != 0) _cache.Set("servers", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            
            return list;
        }
        #endregion
    }
}