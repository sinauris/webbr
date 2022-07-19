using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ServiceStack.Html;
using Webbr.Extensions;
using Webbr.Hubs;
using Webbr.Jwt.Helpers;
using Webbr.Models.IndexModel;
using Webbr.ViewModels.IndexViewModel;

namespace Webbr.Controllers
{
    [Authorize(Policy = Constants.JwtPolicy.Basic)]
    [Route("api/[controller]/[action]")]
    public class IndexController : Controller
    {
        #region Field
        private IHubClients Clients { get; }
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public IndexController(IHubContext<WebbrHub> context, IWebbrDatabase webbrDatabase)
        {
            Clients = context.Clients;
            _webbrDatabase = webbrDatabase;
        }
        #endregion

        
        #region GetLinks
        [HttpGet]
        public async Task<IEnumerable> GetLinks()
        {
            var links = await _webbrDatabase.QueryAsync<IndexLinkModel>("SELECT id, link, description, `column` FROM home_index");
            var sections = await _webbrDatabase.QueryAsync<IndexSectionModel>("SELECT id, section_name FROM home_index_section");
            var dict = new Dictionary<string, dynamic> {{"links", links}, {"sections", sections}};

            return dict;
        }
        #endregion
        
        #region CreateLink
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpPost]
        public async Task<IActionResult> CreateLink([FromBody] IndexLinkCreateViewModel data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _webbrDatabase.ExecuteAsync("INSERT INTO webbr.home_index (link, description, `column`) VALUES(@Link, @Description, @Column)",new {data.Link, data.Description, data.Column});
            await Clients.All.SendAsync("linksUpdate");
            return Ok();
        }
        #endregion
        
        #region UpdateLink
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpPost]
        public async void UpdateLink([FromBody] IndexLinkUpdateViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrDatabase.ExecuteAsync("UPDATE webbr.home_index SET link=@Link, description=@Description, `column`=@Column WHERE id=@Id",new {data.Id, data.Link, data.Description, data.Column});
            await Clients.All.SendAsync("linksUpdate");
        }
        #endregion
        
        #region DeleteLink
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpPost]
        public async void DeleteLink([FromBody] IndexLinkDeleteViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrDatabase.ExecuteAsync("DELETE FROM webbr.home_index WHERE id=@Id", new {data.Id});
            await Clients.All.SendAsync("linksUpdate");
        }
        #endregion


        #region CreateColumn
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpPost]
        public async Task<IActionResult> CreateColumn([FromBody] IndexSectionCreateViewModel data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _webbrDatabase.ExecuteAsync("INSERT INTO webbr.home_index_section (Section_name) VALUES(@Section_name)", new {data.Section_name});
            await Clients.All.SendAsync("linksUpdate");
            return Ok();
        }
        #endregion

        #region UpdateColumn
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpPost]
        public async void UpdateColumn([FromBody] IndexSectionUpdateViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrDatabase.ExecuteAsync("UPDATE webbr.home_index_section SET section_name=@Section_name WHERE id=@Id",new {data.Id, data.Section_name});
            await Clients.All.SendAsync("linksUpdate");
        }
        #endregion
        
        #region DeleteColumn
        [Authorize(Policy = Constants.JwtPolicy.Rg)]
        [HttpPost]
        public async void DeleteColumn([FromBody] IndexSectionDeleteViewModel data)
        {
            if (!ModelState.IsValid) return;
            await _webbrDatabase.ExecuteAsync("DELETE FROM webbr.home_index_section WHERE id=@Id", new {data.Id});
            await Clients.All.SendAsync("linksUpdate");
        }
        #endregion

        
        #region CreateNews
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpPost]
        public async Task<IActionResult> CreateNews([FromBody] CreateNewsViewModel data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var doc = await new HtmlParser().ParseDocumentAsync(data.Title);
            var title = doc.Body.Text().Replace("\n", string.Empty);
            var body = Minifiers.HtmlAdvanced.Compress(data.Body);

            var author = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "din")?.Value;
            var adLogin = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var dateOfCreation = DateTime.Now.ToString("O");

            var newsId = await _webbrDatabase.QueryAsync<int>(
                "INSERT INTO home_news (title, body, author, adlogin, createdtime) VALUES(@Title, @Body, @Author, @Adlogin, @Createdtime); SELECT LAST_INSERT_ID();",
                new {Title = title, Body = body, Author = author, Adlogin = adLogin, Createdtime = dateOfCreation});

            await Clients.All.SendAsync("newsUpdate");

            return Ok(newsId);
        }
        #endregion
        
        #region UpdateNews
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpPost]
        public async Task<IActionResult> UpdateNews([FromBody] UpdateNewsViewModel data)
        {
            if (!ModelState.IsValid) return BadRequest();

            var doc = await new HtmlParser().ParseDocumentAsync(data.Title);
            var title = doc.Body.Text().Replace("\n", string.Empty);
            var body = Minifiers.HtmlAdvanced.Compress(data.Body);

            var contextRole = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "rol")?.Value;
            var contextLogin = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var rec = await _webbrDatabase.QueryAsync<NewsModel>("SELECT AdLogin FROM home_news WHERE id=@Id",
                new {data.Id});
            if (contextRole != "admin" && contextRole != "rg" && contextLogin != rec.ToList()[0].AdLogin) return BadRequest();

            await _webbrDatabase.ExecuteAsync(
                "UPDATE webbr.home_news SET title=@Title, body=@Body, changetime=@Changetime, changelogin=@Changelogin WHERE id=@Id",
                new {data.Id, Title = title, Body = body, Changetime = DateTime.Now.ToString("O"), Changelogin = contextLogin});

            await Clients.All.SendAsync("newsUpdate");

            return Ok(data.Id);
        }
        #endregion
        
        #region GetNewsTitle
        [HttpGet]
        public async Task<IEnumerable<NewsModel>> GetNewsTitle()
        {
            return await _webbrDatabase.QueryAsync<NewsModel>("SELECT id, title, author, adlogin, createdtime, changelogin, changetime FROM home_news");
        }
        #endregion
        
        #region GetNews
        [HttpGet]
        public async Task<IEnumerable<NewsModel>> GetNews()
        {
            return await _webbrDatabase.QueryAsync<NewsModel>("SELECT id, title, body, author, adlogin, createdtime, changelogin, changetime FROM home_news");
        }
        #endregion
        
        #region GetOneNews
        [HttpGet]
        public async Task<IEnumerable<NewsModel>> GetOneNews(int id)
        {
            if (!ModelState.IsValid) return null;

            var rec = await _webbrDatabase.QueryAsync<NewsModel>("SELECT id, title, body, author, adlogin, createdtime, changelogin, changetime FROM home_news WHERE id=@Id",new {Id = id});
            var recList = rec.ToList();
            if (recList.Count != 0) return recList;

            return new List<NewsModel>{ new NewsModel {Title = "Ошибка", Body = "Запись с таким ID отсутствует, была удалена или не хватает прав"} };
        }
        #endregion

        #region GetLatestNews
        [HttpGet]
        public async Task<IEnumerable<NewsModel>> GetLatestNews()
        {
            return await _webbrDatabase.QueryAsync<NewsModel>("SELECT id, title, body, author, adlogin, createdtime, changelogin, changetime FROM home_news ORDER BY id DESC LIMIT 100");
        }
        #endregion

        #region DeleteNews
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpPost]
        public async void DeleteNews([FromBody] DeleteNewsViewModel data)
        {
            if (!ModelState.IsValid) return;

            var contextRole = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "rol")?.Value;
            var contextLogin = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var rec = await _webbrDatabase.QueryAsync<NewsModel>("SELECT * FROM home_news WHERE id=@Id", new {data.Id});
            if (contextRole != "admin" && contextRole != "rg" && contextLogin != rec.ToList()[0].AdLogin) return;

            await _webbrDatabase.ExecuteAsync("DELETE FROM webbr.home_news WHERE id=@Id", new {data.Id});

            await Clients.All.SendAsync("newsUpdate");
        }
        #endregion

        
        #region UploadFile
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> UploadFile([FromForm] IFormCollection file)
        {
            // Путь, отдающий файлы с сайта
            const string webbrPath = "https://webbr.newcontact.su/gluster/";

            // Время для создания папок
            var datetime = DateTime.Now;
            var year = datetime.Year.ToString();
            var month = datetime.Month.ToString();
            var day = datetime.Day.ToString();

            // Создаём случайное имя папки
            var lastFolder = Path.GetRandomFileName().Replace(".", "");

            // Собираем путь до папки
            var glusterPath = Path.Combine("/", "home", "glvolume", "webbr");
            var folderName = Path.Combine(year, month, day, lastFolder);
            var fileRoute = Path.Combine(glusterPath, folderName);

            // Берём тип файла
//            var mimeType = file.Files.First().ContentType;

            // Берём расширение файла
            var extension = Path.GetExtension(file.Files.First().FileName);

            var fileName = file.Files.First().FileName;
            if (fileName.Length > 70) fileName = Guid.NewGuid().ToString().Substring(0, 16) + extension;

            // Собираем путь до папки, включая файл
            var link = Path.Combine(fileRoute, fileName);

            // Создать директории в случае их отсутствия
            if (!Directory.Exists(fileRoute)) Directory.CreateDirectory(fileRoute);

            // Базовая проверка типа, расширения и размера файла
//            string[] fileMimetypes = { "text/plain", "application/msword", "application/x-pdf", "application/pdf", "application/json", "text/html" };
//            string[] fileExt = { ".txt", ".pdf", ".doc", ".json", ".html" };
            const int fileLength = 100 * 1024 * 1024;

            try
            {
                if (file.Files.First().Length <= fileLength)
                {
                    // Копируем файл в оперативную память
                    using (var stream = new MemoryStream())
                    {
                        file.Files.First().CopyTo(stream);
                        stream.Position = 0;
                        var serverPath = link;

                        // Сохраняем файл в нужное место
                        using (var writerFileStream = System.IO.File.Create(serverPath))
                        {
                            await stream.CopyToAsync(writerFileStream);
                            writerFileStream.Dispose();
                        }
                    }

                    // Возвращаем путь до файла в формате Json
                    var fileUrl = new Hashtable {{"link", webbrPath + folderName + "/" + fileName}};
                    return Json(fileUrl);
                }

                throw new ArgumentException("Неверный тип файла");
            }

            catch (ArgumentException ex)
            {
                return Json(ex.Message);
            }
        }
        #endregion

        #region UploadImage
        [Authorize(Policy = Constants.JwtPolicy.OtpOnly)]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> UploadImage([FromForm] IFormCollection image)
        {
            // Путь, отдающий файлы с сайта
            const string webbrPath = "https://webbr.newcontact.su/gluster/";

            // Время для создания папок
            var datetime = DateTime.Now;
            var year = datetime.Year.ToString();
            var month = datetime.Month.ToString();
            var day = datetime.Day.ToString();

            // Создаём случайное имя папки
            var lastFolder = Path.GetRandomFileName().Replace(".", "");

            // Собираем путь до папки
            var glusterPath = Path.Combine("/", "home", "glvolume", "webbr");
            var folderName = Path.Combine(year, month, day, lastFolder);
            var fileRoute = Path.Combine(glusterPath, folderName);

            // Берём тип файла
            var mimeType = image.Files.First().ContentType;

            // Берём расширение файла
            var extension = Path.GetExtension(image.Files.First().FileName);

            var fileName = image.Files.First().FileName;
            if (fileName.Length > 70) fileName = Guid.NewGuid().ToString().Substring(0, 16) + extension;

            // Собираем путь до папки, включая файл
            var link = Path.Combine(fileRoute, fileName);

            // Создать директории в случае их отсутствия
            if (!Directory.Exists(fileRoute)) Directory.CreateDirectory(fileRoute);

            // Базовая проверка типа, расширения и размера файла
            string[] imageMimetypes = {"image/gif", "image/jpeg", "image/pjpeg", "image/x-png", "image/png", "image/svg+xml"};
            string[] imageExt = {".jpeg", ".jpg", ".png", ".gif"};
            const int imageLength = 10 * 1024 * 1024;

            try
            {
                if (Array.IndexOf(imageMimetypes, mimeType) >= 0 && Array.IndexOf(imageExt, extension) >= 0 &&
                    image.Files.First().Length <= imageLength)
                {
                    // Копируем файл в оперативную память
                    using (var stream = new MemoryStream())
                    {
                        image.Files.First().CopyTo(stream);
                        stream.Position = 0;
                        var serverPath = link;

                        // Сохраняем файл в нужное место
                        using (var writerFileStream = System.IO.File.Create(serverPath))
                        {
                            await stream.CopyToAsync(writerFileStream);
                            writerFileStream.Dispose();
                        }
                    }

                    // Возвращаем путь до файла в формате Json
                    var fileUrl = new Hashtable {{"link", webbrPath + folderName + "/" + fileName}};
                    return Json(fileUrl);
                }

                throw new ArgumentException("Неверный тип файла");
            }

            catch (ArgumentException ex)
            {
                return Json(ex.Message);
            }
        }
        #endregion
    }
}