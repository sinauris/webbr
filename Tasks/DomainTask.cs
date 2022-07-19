using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Controls;
using RecurrentTasks;
using Webbr.Extensions;
using Webbr.Models.DomainModels;

namespace Webbr.Tasks
{
    public class DomainTask : IRunnable
    {
        #region Field
        private readonly IWebbrDatabase _webbrDatabase;
        private readonly IMemoryCache _cache;
        #endregion

        #region Constructor
        public DomainTask(IWebbrDatabase webbrDatabase, IMemoryCache memoryCache)
        {
            _webbrDatabase = webbrDatabase;
            _cache = memoryCache;
        }
        #endregion

        
        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            var configurationLdap = await _webbrDatabase.QueryAsync<dynamic>($"SELECT * FROM configuration_variables_domain");
            if (configurationLdap.Count != 0)
            {
                var ldap = configurationLdap.ToList().First();

                await GetComputers(ldap.url, ldap.bind_dn, ldap.bind_credentials);
                await GetUsers(ldap.url, ldap.bind_dn, ldap.bind_credentials, ldap.search_base);
                await GetGroups(ldap.url, ldap.bind_dn, ldap.bind_credentials);
            }
        }

        #region GetComputers
        private async Task GetComputers(string url, string bindDn, string bindCredentials)
        {
            using (var connection = new LdapConnection())
            {
                var list = new List<DomainComputerModel>();

                connection.Connect(url, LdapConnection.DEFAULT_PORT);
                connection.Bind(bindDn, bindCredentials);

                string[] attrs =
                {
                    "objectGUID", "distinguishedName", "name", "description", "extensionAttribute1", "extensionAttribute2", "extensionAttribute3", "extensionAttribute6",
                    "operatingSystem", "whenCreated", "memberOf", "ms-Mcs-AdmPwd", "ms-Mcs-AdmPwdExpirationTime"
                };

                var startIndex = 1;
                var contentCount = 0;
                var afterIndex = 1000;
                var count = 0;

                if (connection.Connected && connection.Bound)
                {
                    while (count <= contentCount)
                    {
                        var ctrl = new LdapVirtualListControl(startIndex, 0, afterIndex, contentCount);
                        var keys = new LdapSortKey[1];
                        keys[0] = new LdapSortKey("objectGUID");
                        var sort = new LdapSortControl(keys, true);

                        var constraints = connection.SearchConstraints;
                        constraints.setControls(new LdapControl[] {ctrl, sort});

                        var lsc = connection.Search("DC=NCC,DC=NEOVOX,DC=RU", LdapConnection.SCOPE_SUB, "(objectCategory=computer)", attrs, false, null, constraints);

                        LdapMessage message;
                        while ((message = lsc.getResponse()) != null)
                        {
                            if (message is LdapSearchResult searchResult)
                            {
                                try
                                {
                                    var attributeSet = searchResult.Entry;
                                    var computeComputerCreateDatetime = string.Empty;

                                    try
                                    {
                                        var attr = attributeSet.getAttribute("whenCreated").StringValue;
                                        computeComputerCreateDatetime = DateTime.ParseExact(attr, "yyyyMMddHHmmss.f'Z'", null).ToString("O", CultureInfo.InvariantCulture);
                                    }
                                    catch {}

                                    var computeComputerLogonDatetime = string.Empty;
                                    try
                                    {
                                        var attr = attributeSet.getAttribute("description").StringValue;
                                        var match = new Regex(@"^(\d+)([-])(\d+)([-])(\d+)([@])(\d+:\d+)").Match(attr);
                                        if (match.Success)
                                        {
                                            var tempDate = match.Groups[5].Value + "." + match.Groups[3].Value + "." + match.Groups[1].Value + " - " + match.Groups[7].Value;
                                            computeComputerLogonDatetime = DateTime.ParseExact(tempDate, "dd.MM.yyyy - HH:mm", CultureInfo.InvariantCulture).ToString("O");
                                        }
                                    }
                                    catch {}

                                    var computeComputerPowerDatetime = string.Empty;
                                    try
                                    {
                                        var attr = attributeSet.getAttribute("extensionAttribute6").StringValue;
                                        computeComputerPowerDatetime = DateTime.ParseExact(attr, "dd.MM.yyyy - HH:mm", CultureInfo.InvariantCulture).ToString("O");
                                    }
                                    catch {}

                                    var computeComputerGroups = string.Empty;
                                    try
                                    {
                                        var attr = attributeSet.getAttribute("memberOf").StringValueArray;
                                        computeComputerGroups = string.Join(";", attr.Select(x => new Regex("CN=(.*?),").Match(x).Groups[1]));
                                    }
                                    catch {}

                                    list.Add(new DomainComputerModel
                                    {
                                        computer_guid = new Guid((byte[]) (Array) attributeSet.getAttribute("objectGUID").ByteValue).ToString(),
                                        computer_dn = attributeSet.getAttribute("distinguishedName")?.StringValue,
                                        computer_ip = attributeSet.getAttribute("extensionAttribute2")?.StringValue,
                                        computer_mac = attributeSet.getAttribute("extensionAttribute3")?.StringValue,
                                        computer_name = attributeSet.getAttribute("name")?.StringValue,
                                        computer_login = attributeSet.getAttribute("extensionAttribute1")?.StringValue,
                                        computer_groups = computeComputerGroups,
                                        computer_os = attributeSet.getAttribute("operatingSystem")?.StringValue,
                                        computer_power_datetime = computeComputerPowerDatetime,
                                        computer_logon_datetime = computeComputerLogonDatetime,
                                        computer_create_datetime = computeComputerCreateDatetime
                                    });
                                }
                                catch {}
                            }
                            else
                            {
                                var responseMessage = (LdapResponse)message;
                                var controls = responseMessage.Controls;
                                if (controls != null)
                                {
                                    foreach (var control in controls)
                                    {
                                        if (control.ID == "2.16.840.1.113730.3.4.10")
                                        {
                                            var response = new LdapVirtualListResponse(control.ID, control.Critical, control.getValue());
                                            startIndex += afterIndex;
                                            contentCount = response.ContentCount;
                                            count += afterIndex;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    connection.Disconnect();

                    _cache.Set("domain_computers", list, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    
                    await _webbrDatabase.ExecuteAsync("TRUNCATE TABLE domain_computers");
                    
                    const string domainComputersQuery = @"
INSERT domain_computers (computer_guid, computer_dn, computer_ip, computer_mac, computer_name, computer_login, computer_groups, computer_os, computer_power_datetime, computer_logon_datetime, computer_create_datetime)
VALUES (@computer_guid, @computer_dn, @computer_ip, @computer_mac, @computer_name, @computer_login, @computer_groups, @computer_os, @computer_power_datetime, @computer_logon_datetime, @computer_create_datetime)";
                    await _webbrDatabase.TransactionAsync(domainComputersQuery, list);
                }
            }
        }
        #endregion

        #region GetUsers
        private async Task GetUsers(string url, string bindDn, string bindCredentials, string searchBase)
        {
            using (var connection = new LdapConnection())
            {
                var list = new List<DomainUserModel>();

                connection.Connect(url, LdapConnection.DEFAULT_PORT);
                connection.Bind(bindDn, bindCredentials);

                string[] attrs =
                {
                    "objectGUID", "distinguishedName", "givenName", "sn", "displayName", "sAMAccountName", "mail", "department", "description", "l", "extensionAttribute1", "extensionAttribute2", "extensionAttribute3",
                    "lastLogon", "mobile", "telephoneNumber", "proxyAddresses", "pwdLastSet", "whenCreated", "memberOf", "UserAccountControl", "msDS-SupportedEncryptionTypes"
                };

                var startIndex = 1;
                var contentCount = 0;
                var afterIndex = 1000;
                var count = 0;

                if (connection.Connected && connection.Bound)
                {
                    while (count <= contentCount)
                    {
                        var ctrl = new LdapVirtualListControl(startIndex, 0, afterIndex, contentCount);
                        var keys = new LdapSortKey[1];
                        keys[0] = new LdapSortKey("objectGUID");
                        var sort = new LdapSortControl(keys, true);

                        var constraints = connection.SearchConstraints;
                        constraints.setControls(new LdapControl[] {ctrl, sort});

                        var lsc = connection.Search("DC=CALL-CENTER,DC=NEWCONTACT,DC=SU", LdapConnection.SCOPE_SUB, "(sAMAccountType=805306368)", attrs, false, null, constraints);

                        LdapMessage message;
                        while ((message = lsc.getResponse()) != null)
                        {
                            if (message is LdapSearchResult searchResult)
                            {
                                try
                                {
                                    var attributeSet = searchResult.Entry;

                                    var computeUserComputer = string.Empty;
                                    var computeUserLogonDatetime = string.Empty;
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(attributeSet.getAttribute("extensionAttribute1")
                                            .StringValue))
                                        {
                                            var array = attributeSet.getAttribute("extensionAttribute1").StringValue
                                                .Split(" - ");
                                            var dateString = array?[1]?.Split("@");
                                            computeUserComputer = array?[0];
                                            computeUserLogonDatetime = DateTime
                                                .ParseExact(dateString?[0] + dateString?[1], "yyyy-MM-ddHH:mm", null)
                                                .ToString("O", CultureInfo.InvariantCulture);
                                        }
                                    }
                                    catch
                                    {
                                    }

                                    var computeUserCreateDatetime = string.Empty;
                                    try
                                    {
                                        var attr = attributeSet.getAttribute("whenCreated").StringValue;
                                        if (!string.IsNullOrEmpty(attr))
                                            computeUserCreateDatetime = DateTime
                                                .ParseExact(attr, "yyyyMMddHHmmss.f'Z'", null)
                                                .ToString("O", CultureInfo.InvariantCulture);
                                    }
                                    catch
                                    {
                                    }


                                    var computeUserGroups = string.Empty;
                                    try
                                    {
                                        var attr = attributeSet.getAttribute("memberOf").StringValueArray;
                                        if (attr.Length != 0)
                                            computeUserGroups = string.Join(";",
                                                attr.Select(x => new Regex("CN=(.*?),").Match(x).Groups[1]));
                                    }
                                    catch
                                    {
                                    }


                                    var computeUserProxy = string.Empty;
                                    try
                                    {
                                        var attr = attributeSet.getAttribute("proxyAddresses").StringValueArray;
                                        if (attr.Length != 0)
                                            computeUserProxy = string.Join(";", attr);
                                    }
                                    catch
                                    {
                                    }

                                    var userAccountType = GetUserAccountType(
                                        int.Parse(attributeSet.getAttribute("UserAccountControl")?.StringValue));

                                    var userEncryption = string.Empty;
                                    try
                                    {
                                        var attr = attributeSet.getAttribute("msDS-SupportedEncryptionTypes")
                                            .StringValue;
                                        if (!string.IsNullOrEmpty(attr))
                                        {
                                            int.TryParse(attr, out var variable);
                                            userEncryption = GetUserSupportedEncryptionType(variable);
                                        }
                                        else
                                        {
                                            userEncryption = "Not encrypted";
                                        }
                                    }
                                    catch
                                    {
                                        userEncryption = "Not encrypted";
                                    }

                                    list.Add(new DomainUserModel
                                    {
                                        user_guid = new Guid((byte[]) (Array) attributeSet.getAttribute("objectGUID").ByteValue).ToString(),
                                        user_dn = attributeSet.getAttribute("distinguishedName")?.StringValue,
                                        user_firstname = attributeSet.getAttribute("givenName")?.StringValue,
                                        user_lastname = attributeSet.getAttribute("sn")?.StringValue,
                                        user_username = attributeSet.getAttribute("displayName")?.StringValue,
                                        user_login = attributeSet.getAttribute("sAMAccountName")?.StringValue,
                                        user_mail = attributeSet.getAttribute("mail")?.StringValue,
                                        user_department = attributeSet.getAttribute("department")?.StringValue,
                                        user_description = attributeSet.getAttribute("description")?.StringValue,
                                        user_city = attributeSet.getAttribute("l")?.StringValue,
                                        user_groups = computeUserGroups,
                                        user_mobile_phone = attributeSet.getAttribute("mobile")?.StringValue,
                                        user_naumen_phone = attributeSet.getAttribute("telephoneNumber")?.StringValue,
                                        user_proxy = computeUserProxy,
                                        user_password_last_set = DateTime.FromFileTimeUtc(Convert.ToInt64(attributeSet.getAttribute("pwdLastSet")?.StringValue)).ToString("O"),
                                        user_ip = attributeSet.getAttribute("extensionAttribute2")?.StringValue,
                                        user_mac = attributeSet.getAttribute("extensionAttribute3")?.StringValue,
                                        user_computer = computeUserComputer,
                                        user_logon_datetime = computeUserLogonDatetime,
                                        user_create_datetime = computeUserCreateDatetime,
                                        user_account_type = int.Parse(attributeSet.getAttribute("UserAccountControl")?.StringValue),
                                        user_disabled = userAccountType.First().UserDisabled,
                                        user_password_expires = userAccountType.First().UserPasswordExpires,
                                        user_password_required = userAccountType.First().UserPasswordRequired,
                                        user_encryption = userEncryption
                                    });
                                }
                                catch {}
                            }
                            else
                            {
                                var responseMessage = (LdapResponse)message;
                                var controls = responseMessage.Controls;
                                if (controls != null)
                                {
                                    foreach (var control in controls)
                                    {
                                        if (control.ID == "2.16.840.1.113730.3.4.10")
                                        {
                                            var response = new LdapVirtualListResponse(control.ID, control.Critical, control.getValue());
                                            startIndex += afterIndex;
                                            contentCount = response.ContentCount;
                                            count += afterIndex;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    connection.Disconnect();

                    _cache.Set("domain_users", list, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    
                    await _webbrDatabase.ExecuteAsync("TRUNCATE TABLE domain_users");
                    const string domainUsersQuery = @"
INSERT domain_users (user_guid, user_dn, user_firstname, user_lastname, user_username, user_login, user_mail, user_department, user_description, user_city, user_groups, user_mobile_phone, user_naumen_phone, user_proxy, user_password_last_set, user_ip, user_mac, user_computer, user_logon_datetime, user_create_datetime, user_account_type, user_disabled, user_password_expires, user_password_required, user_encryption)
VALUES (@user_guid, @user_dn, @user_firstname, @user_lastname, @user_username, @user_login, @user_mail, @user_department, @user_description, @user_city, @user_groups, @user_mobile_phone, @user_naumen_phone, @user_proxy, @user_password_last_set, @user_ip, @user_mac, @user_computer, @user_logon_datetime, @user_create_datetime, @user_account_type, @user_disabled, @user_password_expires, @user_password_required, @user_encryption)";
                    await _webbrDatabase.TransactionAsync(domainUsersQuery, list);
                }
            }
        }
        #endregion

        #region GetGroups
        private async Task GetGroups(string url, string bindDn, string bindCredentials)
        {
            using (var connection = new LdapConnection())
            {
                var list = new List<DomainGroupModel>();

                connection.Connect(url, LdapConnection.DEFAULT_PORT);
                connection.Bind(bindDn, bindCredentials);

                string[] attrs =
                {
                    "objectGUID", "distinguishedName", "name", "sAMAccountName", "description", "member", "memberOf", "whenChanged", "whenCreated"
                };

                var startIndex = 1;
                var contentCount = 0;
                var afterIndex = 1000;
                var count = 0;

                if (connection.Connected && connection.Bound)
                {
                    while (count <= contentCount)
                    {
                        var ctrl = new LdapVirtualListControl(startIndex, 0, afterIndex, contentCount);
                        var keys = new LdapSortKey[1];
                        keys[0] = new LdapSortKey("objectGUID");
                        var sort = new LdapSortControl(keys, true);

                        var constraints = connection.SearchConstraints;
                        constraints.setControls(new LdapControl[] {ctrl, sort});

                        var lsc = connection.Search("DC=CALL-CENTER,DC=NEWCONTACT,DC=SU", LdapConnection.SCOPE_SUB, "(objectCategory=group)", attrs, false, null, constraints);

                        LdapMessage message;
                        while ((message = lsc.getResponse()) != null)
                        {
                            if (message is LdapSearchResult searchResult)
                            {
                                try
                                {
                                    var attributeSet = searchResult.Entry;

                                    var computeGroupMember = "";
                                    try
                                    {
                                        var attr = attributeSet.getAttribute("member").StringValueArray;
                                        computeGroupMember = string.Join(";", attr.Select(x => new Regex("CN=(.*?),").Match(x).Groups[1]));
                                    }
                                    catch {}

                                    var computeGroupMemberOf = "";
                                    try
                                    {
                                        var attr = attributeSet.getAttribute("memberOf").StringValueArray;
                                        computeGroupMemberOf = string.Join(";", attr.Select(x => new Regex("CN=(.*?),").Match(x).Groups[1]));
                                    }
                                    catch {}

                                    var computeGroupChangeDatetime = "";
                                    try
                                    {
                                        var attr = attributeSet.getAttribute("whenChanged").StringValue;
                                        computeGroupChangeDatetime = DateTime.ParseExact(attr, "yyyyMMddHHmmss.f'Z'", null).ToString("O", CultureInfo.InvariantCulture);
                                    }
                                    catch{}

                                    var computeGroupCreateDatetime = "";
                                    try
                                    {
                                        var attr = attributeSet.getAttribute("whenCreated").StringValue;
                                        computeGroupCreateDatetime = DateTime.ParseExact(attr, "yyyyMMddHHmmss.f'Z'", null).ToString("O", CultureInfo.InvariantCulture);
                                    }
                                    catch {}

                                    list.Add(new DomainGroupModel
                                    {
                                        group_guid = new Guid((byte[]) (Array) attributeSet.getAttribute("objectGUID").ByteValue).ToString(),
                                        group_dn = attributeSet.getAttribute("distinguishedName")?.StringValue,
                                        group_name = attributeSet.getAttribute("name")?.StringValue,
                                        group_login = attributeSet.getAttribute("sAMAccountName")?.StringValue,
                                        group_description = attributeSet.getAttribute("description")?.StringValue,
                                        group_member = computeGroupMember,
                                        group_memberOf = computeGroupMemberOf,
                                        group_change_datetime = computeGroupChangeDatetime,
                                        group_create_datetime = computeGroupCreateDatetime
                                    });
                                }
                                catch {}
                            }
                            else
                            {
                                var responseMessage = (LdapResponse)message;
                                var controls = responseMessage.Controls;
                                if (controls != null)
                                {
                                    foreach (var control in controls)
                                    {
                                        if (control.ID == "2.16.840.1.113730.3.4.10")
                                        {
                                            var response = new LdapVirtualListResponse(control.ID, control.Critical, control.getValue());
                                            startIndex += afterIndex;
                                            contentCount = response.ContentCount;
                                            count += afterIndex;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    connection.Disconnect();

                    _cache.Set("domain_groups", list, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    
                    await _webbrDatabase.ExecuteAsync("TRUNCATE TABLE domain_groups");

                    const string domainGroupsQuery = @"
INSERT domain_groups (group_guid, group_dn, group_name, group_login, group_description, group_member, group_memberOf, group_change_datetime, group_create_datetime)
VALUES (@group_guid, @group_dn, @group_name, @group_login, @group_description, @group_member, @group_memberOf, @group_change_datetime, @group_create_datetime)";
                    await _webbrDatabase.TransactionAsync(domainGroupsQuery, list);
                }
            }
        }
        #endregion


        #region GetUserSupportedEncryptionType
        private string GetUserSupportedEncryptionType(int encryption)
        {
            if (encryption == 1) return "DES_CRC";
            if (encryption == 2) return "DES_MD5";
            if (encryption == 3) return "DES_CRC,DES_MD5";
            if (encryption == 4) return "RC4";
            if (encryption == 8) return "AES128Only";
            if (encryption == 16) return "AES256Only";
            if (encryption == 24) return "AES128,AES256";
            if (encryption == 28) return "RC4,AES128,AES256";
            if (encryption == 31) return "DES_CRC,DES_MD5,RC4,AES128,AES256";
            return "Not encrypted";
        }
        #endregion

        #region GetUserAccountType
        private List<UserAccountType> GetUserAccountType(int userAccountType)
        {
            if (userAccountType == 512) return new List<UserAccountType>{new UserAccountType {UserDisabled = "normal_account", UserPasswordExpires = "password_not_expires", UserPasswordRequired = "password_required"}};
            if (userAccountType == 514) return new List<UserAccountType>{new UserAccountType {UserDisabled = "disabled_account", UserPasswordExpires = "password_not_expires", UserPasswordRequired = "password_required"}};
            if (userAccountType == 66048) return new List<UserAccountType>{new UserAccountType {UserDisabled = "normal_account", UserPasswordExpires = "password_expires", UserPasswordRequired = "password_required"}};
            if (userAccountType == 66050) return new List<UserAccountType>{new UserAccountType {UserDisabled = "disabled_account", UserPasswordExpires = "password_expires", UserPasswordRequired = "password_required"}};
            if (userAccountType == 66080) return new List<UserAccountType>{new UserAccountType {UserDisabled = "normal_account", UserPasswordExpires = "password_expires", UserPasswordRequired = "password_not_required"}};
            if (userAccountType == 66082) return new List<UserAccountType>{new UserAccountType {UserDisabled = "disabled_account", UserPasswordExpires = "password_expires", UserPasswordRequired = "password_not_required"}};
            return new List<UserAccountType>{new UserAccountType{UserDisabled = "disabled_undefined", UserPasswordExpires = "pwd_expires_undefined", UserPasswordRequired = "pwd_required_undefined"}};
        }
        #endregion
    }

    public class UserAccountType
    {
        public string UserDisabled { get; set; }
        public string UserPasswordExpires { get; set; }
        public string UserPasswordRequired { get; set; }
    }
}