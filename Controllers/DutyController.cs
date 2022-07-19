using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Webbr.Extensions;
using Webbr.Jwt.Helpers;
using Webbr.Models;
using Webbr.Models.DutyModel;
using Webbr.ViewModels.DutyViewModel;

namespace Webbr.Controllers
{
    [Authorize(Policy = Constants.JwtPolicy.Basic)]
    public class DutyController : Controller
    {
        #region Field
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public DutyController(IWebbrDatabase webbrDatabase) => _webbrDatabase = webbrDatabase;
        #endregion

        
        #region CreateDuty
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpPost("api/[controller]/[action]")]
        public async Task<IActionResult> CreateDuty([FromBody] CreateDutyViewModel data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var Name = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "din")?.Value;
            var Place = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "plc")?.Value;

            var pl = await _webbrDatabase.QueryAsync<PlacesModel>($"SELECT placeid FROM configuration_variables_places WHERE place_description='{Place}'");

            var placeid = pl.First().placeid;
            var date = DateTime.Now.ToString("O");
            var balance2 = string.IsNullOrEmpty(data.Balance2) ? string.Empty : data.Balance2;

            var statusDuty = string.Empty;


            var lastValue = await _webbrDatabase.QueryAsync<DutyModel>(
                $"SELECT id, date, stpinc, balance, balance2, name, status, comment FROM duty WHERE placeid={pl.First().placeid} ORDER BY id DESC LIMIT 1");

            if (lastValue.Count != 0)
            {
                var lastValueName = lastValue.First().name;
                var lastValueStatus = lastValue.First().status;

                if (lastValueStatus.Contains("Принял") && lastValueName == Name) statusDuty = "Сдал";
                else if (lastValueStatus.Contains("Принял") && lastValueName != Name)
                    statusDuty = "Сдал" + $" (за {lastValueName})";
                else if (lastValueStatus.Contains("Сдал")) statusDuty = "Принял";

                await _webbrDatabase.ExecuteAsync(
                    "INSERT duty (date, stpinc, balance, balance2, name, status, comment, placeid) VALUES(@Date, @Stpinc, @Balance, @Balance2, @Name, @Status, @Comment, @Placeid)",
                    new
                    {
                        Date = date, data.Stpinc, data.Balance, Balance2 = balance2, Name, Status = statusDuty,
                        data.Comment, Placeid = placeid
                    });
            }
            else
            {
                statusDuty = "Принял";
                await _webbrDatabase.ExecuteAsync(
                    "INSERT duty (date, stpinc, balance, balance2, name, status, comment, placeid) VALUES(@Date, @Stpinc, @Balance, @Balance2, @Name, @Status, @Comment, @Placeid)",
                    new
                    {
                        Date = date, data.Stpinc, data.Balance, Balance2 = balance2, Name, Status = statusDuty,
                        data.Comment, Placeid = placeid
                    });
            }

            return Ok();
        }
        #endregion
        
        #region Duty
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpGet("api/[controller]/getduty{place}")]
        public async Task<IEnumerable<DutyModel>> Duty(string place)
        {
            if (place.Length < 5)
            {
                var pl = await _webbrDatabase.QueryAsync<PlacesModel>($"SELECT placeid FROM configuration_variables_places WHERE place_short_description='{place}'");
                if (pl.Count != 0)
                    return await _webbrDatabase.QueryAsync<DutyModel>($"SELECT id, date, stpinc, balance, balance2, name, status, comment FROM duty WHERE placeid={pl.First().placeid} ORDER BY id DESC");
            }

            return new List<DutyModel>();
        }
        #endregion
        
        #region DutyUser
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpGet("api/[controller]/[action]")]
        public async Task<IEnumerable<DutyModel>> DutyUser()
        {
            var place = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "plc")?.Value;

            if (!string.IsNullOrEmpty(place))
            {
                var pl = await _webbrDatabase.QueryAsync<PlacesModel>($"SELECT placeid FROM configuration_variables_places WHERE place_description='{place}'");
                if (pl.Count != 0)
                    return await _webbrDatabase.QueryAsync<DutyModel>($"SELECT id, date, stpinc, balance, balance2, name, status, comment FROM duty WHERE placeid={pl.First().placeid} ORDER BY id DESC LIMIT 20");
            }

            return new List<DutyModel>();
        }
        #endregion
        
        #region UpdateDuty
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpPost("api/[controller]/[action]")]
        public async Task UpdateDuty([FromBody] UpdateDutyViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrDatabase.ExecuteAsync(
                "UPDATE webbr.duty SET stpinc=@Stpinc, balance=@Balance, balance2=@Balance2, comment=@Comment, status=@Status WHERE id=@Id",
                new {data.Id, data.Stpinc, data.Balance, data.Balance2, data.Comment, data.Status});
        }
        #endregion
        
        #region DeleteDuty
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpPost("api/[controller]/[action]")]
        public async Task DeleteDuty([FromBody] DeleteDutyViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrDatabase.ExecuteAsync("DELETE FROM webbr.duty WHERE id=@Id", new {data.Id});
        }
        #endregion
    }
}