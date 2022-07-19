using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using Webbr.Extensions;
using Webbr.Jwt;
using Webbr.Jwt.Helpers;
using Webbr.Models.DomainModels;
using Webbr.ViewModels;

namespace Webbr.Controllers
{
    [Authorize(Policy = Constants.JwtPolicy.Basic)]
    [Route("api/[controller]/[action]")]
    [Route("auth/[action]")]
    public class DomainController : Controller
    {
        #region Fields
        private const string MemberOfAttribute = "memberOf";
        private const string DisplayNameAttribute = "displayName";
        private const string SAMAccountNameAttribute = "sAMAccountName";
        private const string DescriptionAttribute = "description";
        private const string lAttribute = "l"; // Атрибут City
        private const string GUID = "objectGUID";

        private readonly IMemoryCache _cache;
        private readonly IJwtFactory _jwtFactory;
        private readonly IWebbrDatabase _webbrDatabase;
        private static readonly LdapConnection Connection = new LdapConnection {SecureSocketLayer = false};
        #endregion
        
        #region Constructor
        public DomainController(IWebbrDatabase webbrDatabase, IMemoryCache memoryCache, IJwtFactory jwtFactory)
        {
            _cache = memoryCache;
            _jwtFactory = jwtFactory;
            _webbrDatabase = webbrDatabase;
        }
        #endregion


        #region Token
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Token([FromBody] CredentialsViewModel credentials)
        {
            // Если модель не валидна, возвращает ошибку 400 (Bad Request)
            if (!ModelState.IsValid)
                return BadRequest(Errors.AddErrorToModelState("emptyform", "Пустой запрос на авторизацию", ModelState));

            // Проверяет наличие УЗ в Active Directory
            var identity = await TokenClaims(credentials.UserName, credentials.Password);

            if (identity == null)
                return BadRequest(Errors.AddErrorToModelState("emptyform", "Пустой запрос на авторизацию", ModelState));

            if (identity is DomainUserAuthModel && !string.IsNullOrEmpty(identity.ExceptionKey))
                return BadRequest(Errors.AddErrorToModelState(identity.ExceptionKey, identity.ExceptionError,
                    ModelState));

            // В случае успеха возвращает токен JWT
            var jwt = await GenerateJwt(identity, _jwtFactory, credentials.UserName, new JsonSerializerSettings {Formatting = Formatting.Indented});
            
            return new OkObjectResult(jwt);
        }
        #endregion

        #region GenerateJwt
        private static async Task<string> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JsonSerializerSettings serializerSettings)
        {
            var validTotalSeconds = new JwtIssuerOptions().ValidFor;
            var response = new
            {
                id = identity.Claims.Single(c => c.Type == "id").Value,
                auth_token = await jwtFactory.GenerateEncodedToken(userName, identity),
                expires_in = (int)validTotalSeconds.TotalSeconds
            };

            return JsonConvert.SerializeObject(response, serializerSettings);
        }
        #endregion
        
        #region TokenClaims
        private async Task<dynamic> TokenClaims(string user, string password)
        {
            // Проверяет в Active Directory наличие пользователя
            var verifyResult = await Login(user, password);

            // Если пользователь отсутствует, возвращает ошибку
            if (!string.IsNullOrEmpty(verifyResult.ExceptionKey)) return await Task.FromResult(verifyResult);

            // В случае успеха генерирует JWT Token
            return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(verifyResult.DisplayName, user,
                verifyResult.Role, verifyResult.L, verifyResult.Guid, verifyResult.Theme));
        }
        #endregion

        #region Login
        public async Task<DomainUserAuthModel> Login(string username, string password)
        {
            var configurationLdap = await _webbrDatabase.QueryAsync<dynamic>("SELECT * FROM configuration_variables_domain");
            var ldap = configurationLdap.ToList();

            var regexusername = new Regex(@"^[^@]+", RegexOptions.IgnoreCase).Match(username).Value; // Регулярное выражение для возможности авторизации по Email (v.morozov@newcontact.su)
            var searchFilter = string.Format(ldap.First().search_filter, regexusername);

            Connection.Connect(ldap.First().url, LdapConnection.DEFAULT_PORT);
            Connection.Bind(ldap.First().bind_dn, ldap.First().bind_credentials);

            var queue = Connection.Search(ldap.First().search_base, LdapConnection.SCOPE_SUB, searchFilter,new[]
                {
                    MemberOfAttribute, DisplayNameAttribute, SAMAccountNameAttribute, DescriptionAttribute, lAttribute,
                    GUID
                }, false, null, null);
            
            if (Connection.Connected)
                try
                {
                    LdapMessage message;
                    while ((message = queue.getResponse()) != null)
                        if (message is LdapSearchResult searchResult)
                        {
                            var user = searchResult.Entry;

                            Connection.Bind(user.DN, password);
                            if (Connection.Bound)
                            {
                                var updated = DateTime.Now.ToString("O");

                                string computeDepartment;
                                string computeRole;

                                if (user.getAttribute(SAMAccountNameAttribute)?.StringValue == "tux")
                                {
                                    computeDepartment = "ГОСТЬ";
                                    computeRole = Constants.JwtClaims.Guest;
                                }
                                else if (user.getAttribute(MemberOfAttribute).StringValueArray.Any(x => x == ldap.First().cn_otp))
                                {
                                    computeDepartment = "ОТП";
                                    computeRole = Constants.JwtClaims.Otp;
                                }
                                else if (user.getAttribute(MemberOfAttribute).StringValueArray.Any(x => x == ldap.First().cn_oit))
                                {
                                    computeDepartment = "ОИТ";
                                    computeRole = Constants.JwtClaims.Oit;
                                }
                                else if (user.getAttribute(MemberOfAttribute).StringValueArray.Any(x => x == ldap.First().cn_oaod))
                                {
                                    computeDepartment = "ОАОД";
                                    computeRole = Constants.JwtClaims.Oaod;
                                }

                                else
                                {
                                    return new DomainUserAuthModel
                                    {
                                        ExceptionKey = "forbidden",
                                        ExceptionError = @"Вы не состоите в отделах ОТП\ОИТ\ОАОД"
                                    };
                                }


                                // Берём из БД роль пользователя
                                var webbrConfiguration = await _webbrDatabase.QueryAsync<dynamic>($"SELECT webbr_role, webbr_theme, webbr_banned FROM configuration_personal WHERE ad_login='{username}'");
                                var configuration = webbrConfiguration.ToList();

                                if (configuration.Count != 0 && configuration.First().webbr_banned == 1)
                                    return new DomainUserAuthModel
                                    {
                                        ExceptionKey = "banned",
                                        ExceptionError = "Ваша учётная запись заблокирована"
                                    };

                                var ad_guid = new Guid((byte[]) (Array) user.getAttribute("objectGUID")?.ByteValue).ToString();
                                var ad_login = user.getAttribute(SAMAccountNameAttribute)?.StringValue;
                                var ad_name = user.getAttribute(DisplayNameAttribute)?.StringValue;
                                var ad_department = computeDepartment;
                                var ad_description = !string.IsNullOrEmpty(user.getAttribute(DescriptionAttribute)?.StringValue) ? user.getAttribute(DescriptionAttribute)?.StringValue : string.Empty;
                                var ad_place = !string.IsNullOrEmpty(user.getAttribute(lAttribute)?.StringValue) ? user.getAttribute(lAttribute)?.StringValue : string.Empty;

                                var webbr_role = configuration.Count != 0 ? configuration.First()?.webbr_role : computeRole;
                                var webbr_theme = configuration.Count != 0 ? configuration.First().webbr_theme : Constants.WebbrTheme.Light;


                                // Добавление УЗ
                                await _webbrDatabase.ExecuteAsync(@"
                                    INSERT configuration_personal (ad_guid, ad_login, ad_name, ad_department, ad_description, ad_place, webbr_role, webbr_theme, webbr_register_datetime, webbr_auth_datetime)
                                    VALUES(@ad_guid, @ad_login, @ad_name, @ad_department, @ad_description, @ad_place, @webbr_role, @webbr_theme, @webbr_register_datetime, @webbr_auth_datetime)
                                    ON DUPLICATE KEY UPDATE ad_guid=@ad_guid, ad_login=@ad_login, ad_name=@ad_name, ad_department=@ad_department, ad_description=@ad_description, ad_place=@ad_place, webbr_role=@webbr_role, webbr_theme=@webbr_theme, webbr_auth_datetime=@webbr_auth_datetime",
                                    new
                                    {
                                        ad_guid, ad_login, ad_name, ad_department, ad_description, ad_place, webbr_role,
                                        webbr_theme, webbr_register_datetime = updated, webbr_auth_datetime = updated
                                    });


                                return new DomainUserAuthModel
                                {
                                    DisplayName = ad_name,
                                    Username = ad_login,
                                    Role = webbr_role,
                                    Description = ad_description,
                                    Department = ad_department,
                                    L = ad_place,
                                    Guid = ad_guid,
                                    Theme = webbr_theme
                                };
                            }
                        }
                        else
                        {
                            return new DomainUserAuthModel
                            {
                                ExceptionKey = "invalid",
                                ExceptionError = "Неверный логин или пароль"
                            };
                        }
                }
                catch
                {
                    return new DomainUserAuthModel
                    {
                        ExceptionKey = "invalid",
                        ExceptionError = "Неверный логин или пароль"
                    };
                }
            else
                return new DomainUserAuthModel
                {
                    ExceptionKey = "connectissue",
                    ExceptionError = "Подключение не установлено"
                };

            Connection.Disconnect();
            //Connection.Dispose();
            return new DomainUserAuthModel
            {
                ExceptionKey = "error",
                ExceptionError = "Неизвестная ошибка"
            };
        }
        #endregion

        
        #region GetDomainUsers
        [HttpGet]
        public async Task<IEnumerable<DomainUserModel>> GetDomainUsers()
        {
            if (!_cache.TryGetValue("domain_users", out List<DomainUserModel> list))
            {
                const string query = "SELECT user_guid, user_dn, user_username, user_login, user_mail, user_department, user_description, user_city, user_groups, user_mobile_phone, user_naumen_phone, user_computer, user_logon_datetime, user_account_type, user_disabled FROM domain_users";
                list = await _webbrDatabase.QueryAsync<DomainUserModel>(query);
                if (list.Count != 0) _cache.Set("domain_users", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
            }
            return list;
        }
        #endregion
        
        #region GetDomainComputers
        [HttpGet]
        public async Task<IEnumerable<DomainComputerModel>> GetDomainComputers()
        {
            if (!_cache.TryGetValue("domain_computers", out List<DomainComputerModel> list))
            {
                const string query = "SELECT computer_guid, computer_dn, computer_name, computer_ip, computer_mac, computer_login, computer_os, computer_groups, computer_power_datetime, computer_logon_datetime, computer_create_datetime FROM domain_computers";
                list = await _webbrDatabase.QueryAsync<DomainComputerModel>(query);
                if (list.Count != 0) _cache.Set("domain_computers", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
            }
            return list;
        }
        #endregion

        #region GetDomainGroups
        [HttpGet]
        public async Task<IEnumerable<DomainGroupModel>> GetDomainGroups()
        {
            if (!_cache.TryGetValue("domain_groups", out List<DomainGroupModel> list))
            {
                const string query = "SELECT group_guid, group_name, group_description, group_member, group_memberOf, group_change_datetime, group_create_datetime FROM domain_groups";
                list = await _webbrDatabase.QueryAsync<DomainGroupModel>(query);
                if (list.Count != 0) _cache.Set("domain_groups", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
            }
            return list;
        }
        #endregion
    }
}