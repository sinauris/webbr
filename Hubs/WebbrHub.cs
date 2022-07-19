using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Webbr.Jwt.Helpers;

namespace Webbr.Hubs
{
    [Authorize(Policy = Constants.JwtPolicy.Guest)]
    public class WebbrHub : Hub
    {
        #region Field
        private readonly IMemoryCache _cache;
        #endregion

        #region Constructor
        public WebbrHub(IMemoryCache memoryCache) => _cache = memoryCache;
        #endregion


        #region GetValues
        public async Task GetValues(string key)
        {
            _cache.TryGetValue(key, out var list);
            await Clients.Caller.SendAsync(key, list);
        }
        #endregion
    }
}