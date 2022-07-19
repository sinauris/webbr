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
using Webbr.Models.ConfigurationModel;
using Webbr.Models.PersonalModel;
using Webbr.ViewModels.ConfigurationViewModel;
using Webbr.ViewModels.ConfigurationViewModel.PlacesViewModel;

namespace Webbr.Controllers
{
    [Authorize(Policy = Constants.JwtPolicy.Guest)]
    public class ConfigurationController : Controller
    {
        #region Field
        private readonly IMemoryCache _cache;
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public ConfigurationController(IMemoryCache memoryCache, IWebbrDatabase webbrDatabase)
        {
            _cache = memoryCache;
            _webbrDatabase = webbrDatabase;
        }
        #endregion


        // General //

        #region GetAvailablePlaces
        [HttpGet("api/[controller]/[action]")]
        public async Task<IEnumerable> GetAvailablePlaces()
        {
            const string query = @"SELECT placeid, place_short_description, place_description FROM configuration_variables_places WHERE place_enabled='1'";
            var list = await _webbrDatabase.QueryAsync<PlacesModel>(query);
            return list.OrderBy(x => x.place_description);
        }
        #endregion


        // Commutators //

        #region CreateCommutator
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpPost("api/[controller]/[action]")]
        public async void CreateCommutator([FromBody] ConfigurationCommutatorCreate data)
        {
            if (!ModelState.IsValid) return;
            await _webbrDatabase.ExecuteAsync(
                "INSERT configuration_commutator (ip, port, port_offset, snmp_public_string, snmp_oid, comment, placeid) VALUES(@ip, @port, @port_offset, @snmp_public_string, @snmp_oid, @comment, @placeid)",
                new
                {
                    data.ip, data.port, data.port_offset, data.snmp_public_string, data.snmp_oid, data.comment, data.placeid
                });
        }
        #endregion

        #region GetCommutators
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpGet("api/[controller]/getswitch{place}")]
        public async Task<IEnumerable<ConfigurationCommutatorModel>> Commutator(string place)
        {
            if (place.Length < 5)
            {
                var pl = await _webbrDatabase.QueryAsync<PlacesModel>(
                    $"SELECT placeid FROM configuration_variables_places WHERE place_short_description='{place}'");
                if (pl.Count != 0)
                    return await _webbrDatabase.QueryAsync<ConfigurationCommutatorModel>(
                        $"SELECT id, ip, port, port_offset, snmp_public_string, snmp_oid, comment, enable FROM configuration_commutator WHERE placeid={pl.First().placeid}");
            }

            return new List<ConfigurationCommutatorModel>();
        }
        #endregion

        #region UpdateCommutator
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpPost("api/[controller]/[action]")]
        public async void UpdateCommutator([FromBody] ConfigurationCommutatorUpdate data)
        {
            if (!ModelState.IsValid) return;
            await _webbrDatabase.ExecuteAsync(
                "UPDATE configuration_commutator SET ip=@ip, port=@port, port_offset=@port_offset, snmp_public_string=@snmp_public_string, snmp_oid=@snmp_oid, comment=@comment WHERE id=@id",
                new
                {
                    data.id, data.ip, data.port, data.port_offset, data.snmp_public_string, data.snmp_oid, data.comment
                });
        }
        #endregion

        #region SwitchStateCommutator
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpPost("api/[controller]/[action]")]
        public async void SwitchStateCommutator([FromBody] ConfigurationCommutatorSwitchState data)
        {
            if (!ModelState.IsValid) return;
            await _webbrDatabase.ExecuteAsync("UPDATE configuration_commutator SET enable=@Enable WHERE id=@Id",
                new {data.Id, Enable = data.Enable == 1 ? 0 : 1});
        }
        #endregion

        #region DeleteCommutator
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpPost("api/[controller]/[action]")]
        public async void DeleteCommutator([FromBody] ConfigurationCommutatorDelete data)
        {
            if (!ModelState.IsValid) return;
            await _webbrDatabase.ExecuteAsync("DELETE FROM configuration_commutator WHERE id=@Id", new {data.Id});
        }
        #endregion


        // Users //

        #region GetUsers
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpGet("api/[controller]/[action]")]
        public async Task<IEnumerable<PersonalModel>> GetUsers()
        {
            return await _webbrDatabase.QueryAsync<PersonalModel>("SELECT ad_guid, ad_login, ad_name, ad_department, ad_place, webbr_role, webbr_banned, webbr_register_datetime, webbr_auth_datetime FROM configuration_personal");
        }
        #endregion
        

        // Places //

        #region CreatePlace
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpPost("api/[controller]/[action]")]
        public async void CreatePlace([FromBody] CreatePlacesViewModel data)
        {
            if (!ModelState.IsValid) return;
            
            if (_cache.TryGetValue("places", out List<ConfigurationPlaceModel> list))
            {
                var obj = new ConfigurationPlaceModel
                {
                    placeid = list.Last().placeid + 1,
                    place_description = data.place_description,
                    place_short_description = data.place_short_description,
                    dhcp_server = data.dhcp_server,
                    dhcp_server_command = data.dhcp_server_command,
                    naumen_server = data.naumen_server,
                    comment = data.comment,
                    place_enabled = 0
                };
                
                list.Add(obj);
                _cache.Set("places", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(30)));
            }
            
            await _webbrDatabase.ExecuteAsync("INSERT configuration_variables_places (place_description, place_short_description, dhcp_server, dhcp_server_command, naumen_server, comment) VALUES(@place_description, @place_short_description, @dhcp_server, @dhcp_server_command, @naumen_server, @comment)",new { data.place_short_description, data.place_description, data.dhcp_server, data.dhcp_server_command, data.naumen_server, data.comment });
        }
        #endregion

        #region GetPlaces
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpGet("api/[controller]/[action]")]
        public async Task<IEnumerable<ConfigurationPlaceModel>> GetPlaces()
        {
            const string query = @"SELECT placeid, place_short_description, place_description, dhcp_server, dhcp_server_command, naumen_server, comment, place_enabled FROM configuration_variables_places";

            if (!_cache.TryGetValue("places", out List<ConfigurationPlaceModel> list))
            {
                list = await _webbrDatabase.QueryAsync<ConfigurationPlaceModel>(query);
                _cache.Set("places", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(30)));
            }
            return list.OrderBy(x => x.place_description);
        }
        #endregion

        #region UpdatePlace
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpPost("api/[controller]/[action]")]
        public async void UpdatePlace([FromBody] UpdatePlacesViewModel data)
        {
            if (!ModelState.IsValid) return;
            
            if (_cache.TryGetValue("places", out List<ConfigurationPlaceModel> list))
            {
                list.First(x => x.placeid == data.placeid).place_description = data.place_description;
                list.First(x => x.placeid == data.placeid).place_short_description = data.place_short_description;
                list.First(x => x.placeid == data.placeid).dhcp_server = data.dhcp_server;
                list.First(x => x.placeid == data.placeid).naumen_server = data.naumen_server;
                list.First(x => x.placeid == data.placeid).comment = data.comment;
                _cache.Set("places", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(30)));
            }
            
            await _webbrDatabase.ExecuteAsync(@"UPDATE configuration_variables_places SET place_description=@place_description, place_short_description=@place_short_description, dhcp_server=@dhcp_server, dhcp_server_command=@dhcp_server_command, naumen_server=@naumen_server, comment=@comment WHERE placeid=@placeid", new { data.place_description, data.place_short_description, data.dhcp_server, data.dhcp_server_command, data.naumen_server, data.comment, data.placeid });
        }
        #endregion

        #region SwitchStatePlace
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpPost("api/[controller]/[action]")]
        public async void SwitchStatePlace([FromBody] SwitchPlacesViewModel data)
        {
            if (!ModelState.IsValid) return;
            
            if (_cache.TryGetValue("places", out List<ConfigurationPlaceModel> list))
            {
                list.First(x => x.placeid == data.placeid).place_enabled = data.place_enabled == 1 ? 0 : 1;
                _cache.Set("places", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(30)));
            }
            
            await _webbrDatabase.ExecuteAsync("UPDATE configuration_variables_places SET place_enabled=@place_enabled WHERE placeid=@placeid",new {data.placeid, place_enabled = data.place_enabled == 1 ? 0 : 1});
        }
        #endregion
        
        #region DeletePlace
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpPost("api/[controller]/[action]")]
        public async void DeletePlace([FromBody] DeleteCitiesViewModel data)
        {
            if (!ModelState.IsValid) return;
            
            if (_cache.TryGetValue("places", out List<ConfigurationPlaceModel> list))
            {
                list.RemoveAll(x => x.placeid == data.placeid);
                _cache.Set("places", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(30)));
            }
            
            await _webbrDatabase.ExecuteAsync("DELETE FROM configuration_variables_places WHERE placeid=@placeid",new {data.placeid});
        }
        #endregion
    }
}