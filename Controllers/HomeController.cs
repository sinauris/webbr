using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webbr.Extensions;

namespace Webbr.Controllers
{
    public class HomeController : Controller
    {
        #region Field
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion
        
        #region Constructor
        public HomeController(IWebbrDatabase webbrDatabase) => _webbrDatabase = webbrDatabase;
        #endregion


        #region Index
        public IActionResult Index()
        {
            var webbrTheme = HttpContext.Request.Cookies["webbrTheme"];
            
            if (string.IsNullOrEmpty(webbrTheme))
            {
                HttpContext.Response.Cookies.Append("webbrTheme", "light", new CookieOptions { Expires = DateTime.MaxValue });
                webbrTheme = "light";
            }
            
            ViewBag.WebbrTheme = webbrTheme;
            
            return View();
        }
        #endregion
    }
}