using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Webbr.Extensions
{
    public interface IWebbrLogger
    {
        Task OperatorsCommandToLog(HttpContext context, string rm, string ip, string command, string result, string exception = null);
    }

    public class WebbrLogger : IWebbrLogger
    {
        #region Fields
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public WebbrLogger(IWebbrDatabase webbrDatabase) => _webbrDatabase = webbrDatabase;
        #endregion


        #region OperatorsCommandToLog
        public async Task OperatorsCommandToLog(HttpContext context, string rm, string ip, string command, string result, string exception = null)
        {
            try
            {
                var contextUser = context?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var contextRole = context?.User.Claims.FirstOrDefault(c => c.Type == "rol")?.Value;
                await _webbrDatabase.ExecuteAsync("INSERT log_portmap_commands (user, role, rm, ip, command, result, exception) VALUES(@User, @Role, @Rm, @Ip, @Command, @Result, @Exception)", new { User = contextUser, Role = contextRole, Rm = rm, Ip = ip, Command = command, Result = result, Exception = exception });
            }
            catch(Exception)
            {
                // ignored
            }
        }
        #endregion
    }
}