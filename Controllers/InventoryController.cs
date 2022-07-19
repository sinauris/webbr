using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Webbr.Extensions;
using Webbr.Jwt.Helpers;
using Webbr.Models;

namespace Webbr.Controllers
{
    [Authorize(Policy = Constants.JwtPolicy.Basic)]
    public class InventoryController : Controller
    {
        #region Field
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public InventoryController(IWebbrDatabase webbrDatabase) => _webbrDatabase = webbrDatabase;
        #endregion
        

        #region Inventory
        [HttpGet("api/[controller]/getinventory{place}")]
        public async Task<IEnumerable<InventoryModel>> Inventory(string place)
        {
            if (place.Length < 5)
            {
                var pl = await _webbrDatabase.QueryAsync<PlacesModel>($"SELECT placeid FROM configuration_variables_places WHERE place_short_description='{place}'");
                if (pl.Count != 0)
                    return await _webbrDatabase.QueryAsync<InventoryModel>($"SELECT unq_portmap.rm, inv.* FROM inventory inv JOIN ( SELECT DISTINCT mac, rm FROM ( SELECT * FROM portmap ORDER BY updatetime DESC ) AS unq_portmap ) AS unq_portmap ON inv.mac = unq_portmap.mac WHERE inv.placeid={pl.First().placeid};");
            }
            return new List<InventoryModel>();
        }
        #endregion
    }
}