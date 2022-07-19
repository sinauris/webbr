using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Webbr.Extensions;
using Webbr.Jwt.Helpers;
using Webbr.ViewModels.CommandsViewModel;

namespace Webbr.Controllers
{
    [Authorize(Policy = Constants.JwtPolicy.Basic)]
    [Route("api/[controller]/[action]")]
    public class CommandsController : Controller
    {
        #region Field
        private readonly IWebbrSsh _webbrSsh;
        private readonly IWebbrLogger _webbrLogger;
        #endregion
        
        #region Constructor
        public CommandsController(IWebbrSsh webbrSsh, IWebbrLogger webbrLogger)
        {
            _webbrSsh = webbrSsh;
            _webbrLogger = webbrLogger;
        }
        #endregion

        
        #region SshCommand
        [HttpPost]
        public async Task SshCommand([FromBody] RseCommandViewModel data)
        {
            if (!ModelState.IsValid) return;

            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "SshCommand", "success");
        }
        #endregion

        #region VncCommand
        [HttpPost]
        public async Task VncCommand([FromBody] VncCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "VNCCommand", "success");
        }
        #endregion

        #region VncVOCommand
        [HttpPost]
        public async Task VncVOCommand([FromBody] VncVOCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "VNCVOCommand", "success");
        }
        #endregion

        #region PulseCommand
        [HttpPost]
        public async Task PulseCommand([FromBody] PulseCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "PulseCommand", "success");
        }
        #endregion

        #region RseCommand
        [HttpPost]
        public async Task RseCommand([FromBody] RseCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            string result;
            string exception;

            if (string.IsNullOrEmpty(data.Ip)) return;
            try
            {
                await _webbrSsh.ExecOperatorCommand(data.Ip, 22, "newcontact", "578",@"export DISPLAY=:0 && timeout 8 rse");

                result = "success";
                exception = string.Empty;
            }
            catch (Exception ex)
            {
                result = "fail";
                exception = ex.Message;
            }

            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "RseCommand", result, exception);
        }
        #endregion

        #region MessageCommand
        [HttpPost]
        public async Task MessageCommand([FromBody] MessageCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            string result;
            string exception;

            if (string.IsNullOrEmpty(data.Ip)) return;
            try
            {
                if (string.IsNullOrEmpty(data.Title)) data.Title = "Сообщение от ОТП";

                await _webbrSsh.ExecOperatorCommand(data.Ip, 22, "newcontact", "578",
                    $"DISPLAY=:0.0 zenity --info --title \"{data.Title}\" --text \"{data.Message}\"");
                result = $@"{data.Title} - {data.Message}";
                exception = string.Empty;
            }
            catch (Exception ex)
            {
                result = "fail";
                exception = ex.Message;
            }

            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "MessageCommand", result, exception);
        }
        #endregion

        #region RebootLinuxCommand
        [HttpPost]
        public async Task RebootLinuxCommand([FromBody] RebootLinuxCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            string result;
            string exception;

            if (string.IsNullOrEmpty(data.Ip)) return;
            try
            {
                await _webbrSsh.ExecOperatorCommand(data.Ip, 22, "newcontact", "578",
                    "echo 123qwe$%^ | sudo -S reboot -f");
                result = "success";
                exception = string.Empty;
            }
            catch (Exception ex)
            {
                result = "fail";
                exception = ex.Message;
            }

            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "RebootLinuxCommand", result,
                exception);
        }
        #endregion

        #region ShutdownLinuxCommand
        [HttpPost]
        public async Task ShutdownLinuxCommand([FromBody] ShutdownLinuxCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            string result;
            string exception;

            if (string.IsNullOrEmpty(data.Ip)) return;
            try
            {
                await _webbrSsh.ExecOperatorCommand(data.Ip, 22, "newcontact", "578",
                    "echo 123qwe$%^ | sudo -S poweroff -f");
                result = "success";
                exception = string.Empty;
            }
            catch (Exception ex)
            {
                result = "fail";
                exception = ex.Message;
            }

            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "ShutdownLinuxCommand", result,
                exception);
        }
        #endregion

        #region RdpCommand
        [HttpPost]
        public async Task RdpCommand([FromBody] RdpCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "RdpCommand", "success");
        }
        #endregion
        
        #region ExplorerCCommand
        [HttpPost]
        public async Task ExplorerCCommand([FromBody] ExplorerCCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "ExplorerCCommand", "success");
        }
        #endregion

        #region ExplorerDCommand
        [HttpPost]
        public async Task ExplorerDCommand([FromBody] ExplorerDCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "ExplorerDCommand", "success");
        }
        #endregion

        #region CmdCommand
        [HttpPost]
        public async Task CmdCommand([FromBody] CmdCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "CmdCommand", "success");
        }
        #endregion
        
        #region PwsCommand
        [HttpPost]
        public async Task PwsCommand([FromBody] PwsCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "PwsCommand", "success");
        }
        #endregion
        
        #region RebootWindowsCommand
        [HttpPost]
        public async Task RebootWindowsCommand([FromBody] RebootWindowsCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "RebootWindowsCommand", "success");
        }
        #endregion

        #region ShutdownWindowsCommand
        [HttpPost]
        public async Task ShutdownWindowsCommand([FromBody] ShutdownWindowsCommandViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrLogger.OperatorsCommandToLog(HttpContext, data.Rm, data.Ip, "ShutdownWindowsCommand",
                "success");
        }
        #endregion


        #region GetWebbrUtils
        public VirtualFileResult GetWebbrUtils()
        {
            var filepath = Path.Combine("~/files", "webbrUtils.bat");
            return File(filepath, "application/bat", "webbrUtils.bat");
        }
        #endregion
    }
}