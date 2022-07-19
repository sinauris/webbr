using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Webbr.Extensions;
using Webbr.Jwt.Helpers;
using Webbr.Models;
using Webbr.Models.PortmapModel;
using Webbr.ViewModels.PortmapViewModel;

namespace Webbr.Controllers
{
    [Authorize(Policy = Constants.JwtPolicy.Basic)]
    public class PortmapController : Controller
    {
        #region Field
        private readonly IWebbrDatabase _webbrDatabase;
        private readonly IMemoryCache _cache;
        #endregion

        #region Constructor
        public PortmapController(IWebbrDatabase webbrDatabase, IMemoryCache memoryCache)
        {
            _webbrDatabase = webbrDatabase;
            _cache = memoryCache;
        }
        #endregion


        #region Portmap
        [HttpGet("api/[controller]/getportmap{place}")]
        public async Task<IEnumerable> Portmap(string place)
        {
            if (place.Length < 5)
            {
                var pl = await _webbrDatabase.QueryAsync<PlacesModel>($"SELECT placeid FROM configuration_variables_places WHERE place_short_description='{place}' AND place_enabled='1'");

                const string query = @"
SELECT map.mac, map.rm, map.ip, map.sw, map.swport, map.vlan, map.image, map.naumenlogin as nau_login,
       nau.id_employee as nau_uuid, nau.first_name as nau_first_name, nau.middle_name as nau_middle_name, nau.last_name as nau_last_name, nau.department as nau_department,
       nau.roles as nau_roles, nau.projects as nau_projects,
       nau.location_name as nau_location_name, nau.profile_name as nau_profile_name, nau.creation_time as nau_creation_time, nau.skills as nau_skills,
       users.user_username as win_user_username, users.user_login as win_user_login, users.user_department as win_user_department, users.user_description as win_user_description, users.user_city as win_user_city, 
       comp.computer_name as win_computer_name, comp.computer_logon_datetime as win_computer_logon_datetime, comp.computer_power_datetime as win_computer_power_datetime,
       inv.motherboard as inv_motherboard, inv.cpu as inv_cpu, inv.ram as inv_ram, inv.hdd as inv_hdd, inv.monitor as inv_monitor,
       CASE WHEN comp.computer_os IS NULL THEN 'Linux' ELSE comp.computer_os END AS typeos,
       map.typeip, map.onlinestatus, map.updatetime, map.placeid FROM portmap map
    LEFT JOIN webbr.inventory inv ON map.mac = inv.mac
    LEFT JOIN webbr.domain_computers comp ON map.mac = comp.computer_mac AND map.ip = comp.computer_ip
    LEFT JOIN webbr.domain_users users ON comp.computer_login = users.user_login
    LEFT JOIN webbr.naumen_employee nau ON map.naumenlogin = nau.login
WHERE comp.computer_logon_datetime > now() - INTERVAL 30 DAY AND comp.computer_power_datetime > now() - INTERVAL 30 DAY OR comp.computer_logon_datetime IS NULL";

                if (!_cache.TryGetValue("portmap", out List<MainPortmapModel> list))
                {
                    list = await _webbrDatabase.QueryAsync<MainPortmapModel>(query);
                    if (list.Count != 0) _cache.Set("portmap", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
                return list.Where(x => x.placeid == pl.First().placeid);
            }

            return new List<MainPortmapModel>();
        }
        #endregion
        
        #region UpdatePortmap
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpPost("api/[controller]/[action]")]
        public async void UpdatePortmap([FromBody] UpdatePortmapViewModel data)
        {
            if (!ModelState.IsValid) return;
            
            if (_cache.TryGetValue("portmap", out List<MainPortmapModel> list))
            {
                list.First(x => x.sw == data.Sw && x.swport == Convert.ToInt32(data.Swport)).rm = data.Rm;
                list.First(x => x.sw == data.Sw && x.swport == Convert.ToInt32(data.Swport)).ip = data.Ip;
                if (list.Count != 0) _cache.Set("portmap", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            
            await _webbrDatabase.ExecuteAsync("UPDATE webbr.portmap SET rm=@Rm, ip=@Ip WHERE sw=@Sw AND swport=@Swport", new {data.Sw, data.Swport, data.Rm, data.Ip});
        }
        #endregion
        
        #region DeletePortmap
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpPost("api/[controller]/[action]")]
        public async void DeletePortmap([FromBody] DeletePortmapViewModel data)
        {
            if (!ModelState.IsValid) return;
            
            if (_cache.TryGetValue("portmap", out List<MainPortmapModel> list))
            {
                list.RemoveAll(x => x.sw == data.Sw && x.swport == Convert.ToInt32(data.Swport));
                if (list.Count != 0) _cache.Set("portmap", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            await _webbrDatabase.ExecuteAsync("DELETE FROM webbr.portmap WHERE sw=@Sw AND swport=@Swport", new {data.Sw, data.Swport});
        }
        #endregion
    }
}