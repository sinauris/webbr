using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Webbr.Extensions;
using Webbr.Jwt.Helpers;
using Webbr.Models;
using Webbr.Models.DashboardModel;
using Webbr.Models.TasksModel;

namespace Webbr.Controllers
{
    [Authorize(Policy = Constants.JwtPolicy.Guest)]
    [Route("api/[controller]/[action]")]
    public class DashboardController : Controller
    {
        #region Field
        private readonly IWebbrDatabase _webbrDatabase;
        private readonly IMemoryCache _cache;
        #endregion

        #region Constructor
        public DashboardController(IWebbrDatabase webbrDatabase, IMemoryCache memoryCache)
        {
            _webbrDatabase = webbrDatabase;
            _cache = memoryCache;
        }
        #endregion


        #region GetGrafanaData
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetGrafanaData()
        {
            try
            {
                if (!_cache.TryGetValue("dashboard_grafana", out Dictionary<string, dynamic> list))
                {
                    var servers = await _webbrDatabase.QueryAsync<GrafanaModel>("SELECT id, name, ip, `load`, load_treshold, comment, tag, section FROM dashboard_main_grafana_servers WHERE enable=1 ORDER BY id");
                    var sections = await _webbrDatabase.QueryAsync<GrafanaSectionModel>("SELECT id, section_name FROM dashboard_main_grafana_servers_section");
                    list = new Dictionary<string, dynamic> {{"servers", servers}, {"sections", sections}};
                    
                    if (list.Count != 0) _cache.Set("dashboard_grafana", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
                return list;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        
        #region GetZabbixData
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetZabbixData()
        {
            try
            {
                if (!_cache.TryGetValue("dashboard_zabbix", out List<ZabbixTriggerTaskDbModel> list))
                {
                    const string query = "SELECT hostname, description, lastchange, priority, triggerid FROM dashboard_main_zabbix_triggers";
                    list = await _webbrDatabase.QueryAsync<ZabbixTriggerTaskDbModel>(query);
                    list = list.OrderByDescending(x => x.Priority).ThenByDescending(x => DateTime.ParseExact(x.Lastchange, "O", CultureInfo.InvariantCulture)).ToList();
                    if (list.Count != 0) _cache.Set("dashboard_zabbix", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
                return list;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        
        #region GetUpsData
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetUpsData()
        {
            try
            {
                if (!_cache.TryGetValue("dashboard_ups", out List<UpsTaskDbModel> list))
                {
                    const string query = "SELECT ip, name, input_vol, output_vol, ups_load, battery_temp, battery_capacity, battery_second, battery_min_remaining, tooltip, placeid FROM dashboard_main_ups WHERE enable = 1 ORDER BY placeid";
                    list = await _webbrDatabase.QueryAsync<UpsTaskDbModel>(query);
                    if (list.Count != 0) _cache.Set("dashboard_ups", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
                return list;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region GetNaumenData
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetNaumenData()
        {
            try
            {
                if (!_cache.TryGetValue("dashboard_naumen", out Dictionary<string, dynamic> list))
                {
                    var naumenHostsList = await _webbrDatabase.QueryAsync<GrafanaNaumenHosts>("SELECT d.id, d.naumen, d.hosts_count, p.place_description, d.updated FROM dashboard_main_grafana_naumen_hosts d LEFT JOIN configuration_variables_places p ON p.placeid = d.placeid");
                    var naumenHostsListOrder = naumenHostsList.OrderBy(x => x.Place_description);
                    var naumenParseLicenseList = await _webbrDatabase.QueryAsync<NaumenLicenseParseModel>("SELECT license_name, license_use, license_all, license_description FROM dashboard_main_naumen_licenses");
                    var naumenParseLicenseListEdit = naumenParseLicenseList.Where(x => x.license_all != 0 && x.license_all != 1 && x.license_use != 0);
                    list = new Dictionary<string, dynamic>{{"naumenHostsList", naumenHostsListOrder}, {"naumenLicenseList", naumenParseLicenseListEdit}};
                    
                    if (list.Count != 0) _cache.Set("dashboard_naumen", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
                return list;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        

        #region GetMtsMqTunnelData
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetMtsMqTunnelData()
        {
            if (!_cache.TryGetValue("dashboard_mts_tunnel", out List<dynamic> list))
            {
                const string query = "SELECT * FROM dashboard_main_mts_mq_tunnel WHERE enabled = 1";
                list = await _webbrDatabase.QueryAsync<dynamic>(query);
                if (list.Count != 0) _cache.Set("dashboard_mts_tunnel", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return list;
        }
        #endregion
        
        #region GetMtsMqChannelData
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetMtsMqChannelData()
        {
            if (!_cache.TryGetValue("dashboard_mts_channel", out List<dynamic> list))
            {
                const string query = "SELECT * FROM dashboard_main_mts_mq_channel";
                list = await _webbrDatabase.QueryAsync<dynamic>(query);
                if (list.Count != 0) _cache.Set("dashboard_mts_channel", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return list;
        }
        #endregion

        #region GetMtsMqQueueData
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetMtsMqQueueData()
        {
            if (!_cache.TryGetValue("dashboard_mts_queue", out List<dynamic> list))
            {
                const string query = "SELECT * FROM dashboard_main_mts_mq_queue";
                list = await _webbrDatabase.QueryAsync<dynamic>(query);
                if (list.Count != 0) _cache.Set("dashboard_mts_queue", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return list;
        }
        #endregion
        
        #region GetMtsImportData
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetMtsImportData()
        {
            if (!_cache.TryGetValue("dashboard_mts_import", out List<dynamic> list))
            {
                const string query = "SELECT * FROM dashboard_main_mts_import WHERE enabled = 1";
                list = await _webbrDatabase.QueryAsync<dynamic>(query);
                if (list.Count != 0) _cache.Set("dashboard_mts_import", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return list;
        }
        #endregion
        
        #region GetMtsMgwAgentData
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetMtsMgwAgentData()
        {
            if (!_cache.TryGetValue("dashboard_mts_agent", out List<dynamic> list))
            {
                const string query = "SELECT * FROM dashboard_main_mts_mgw_agent";
                list = await _webbrDatabase.QueryAsync<dynamic>(query);
                if (list.Count != 0) _cache.Set("dashboard_mts_agent", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return list;
        }
        #endregion
        
        #region GetMtsMgwJobData
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetMtsMgwJobData()
        {
            if (!_cache.TryGetValue("dashboard_mts_job", out List<dynamic> list))
            {
                const string query = "SELECT * FROM dashboard_main_mts_mgw_job";
                list = await _webbrDatabase.QueryAsync<dynamic>(query);
                if (list.Count != 0) _cache.Set("dashboard_mts_job", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return list;
        }
        #endregion
    }
}