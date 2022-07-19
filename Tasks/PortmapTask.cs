using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ahd.Graphite;
using Microsoft.Extensions.Caching.Memory;
using RecurrentTasks;
using RunProcessAsTask;
using Webbr.Extensions;
using Webbr.Models;
using Webbr.Models.PortmapModel;
using Webbr.Models.TasksModel;

namespace Webbr.Tasks
{
    public class PortmapTask : IRunnable
    {
        #region Fields
        private readonly IWebbrSsh _webbrSsh;
        private readonly IMemoryCache _cache;
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public PortmapTask(IWebbrDatabase webbrDatabase, IWebbrSsh webbrSsh, IMemoryCache memoryCache)
        {
            _webbrSsh = webbrSsh;
            _cache = memoryCache;
            _webbrDatabase = webbrDatabase;
        }
        #endregion


        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            var placesList = await _webbrDatabase.QueryAsync<PlacesModel>($"SELECT placeid, place_short_description, place_description, dhcp_server, dhcp_server_command, naumen_server FROM configuration_variables_places WHERE place_enabled='1'");
            var tasks = placesList.Select(async z =>
            {
                var dataList = await Commutator(z.placeid);
                dataList = await Dhcp(dataList, z.dhcp_server, z.dhcp_server_command);
                if(dataList.Count != 0) dataList = await Naumen(dataList, z.naumen_server, z.place_short_description, z.placeid);
                if (dataList.Count != 0) await Transaction(dataList, z.placeid);
            });
            await Task.WhenAll(tasks);
            
            await Caching();
        }

        
        #region Commutator
        private async Task<List<MainPortmapModel>> Commutator(int placeid)
        {
            try
            {
                // Список IP-адресов и портов коммутаторов с БД для последующего опроса
                using (var commutatorList = _webbrDatabase.QueryAsync<CommutatorModel>($"SELECT ip, port, port_offset, snmp_public_string, snmp_oid FROM configuration_commutator WHERE enable='1' AND placeid='{placeid}'"))
                {
                    var commutatorModels = commutatorList.Result.ToList();

                    // Время обновления для записи в БД
                    var updated = DateTime.Now.ToString("O");

                    // Считает количество хостов на площадке
                    var hostsCount = 0;

                    var mainListBigData = new List<MainPortmapModel>();

                    var tasks = commutatorModels.Select(async a =>
                    {
                        var ports = a.port.Split(new[] {' ', ',', ';'}, StringSplitOptions.RemoveEmptyEntries).Select(x => (int.Parse(x) + a.port_offset).ToString()).ToArray();

                        var pf = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "/usr/bin/snmpbulkwalk" : @"C:\SNMP\bin\snmpbulkwalk.exe";
                        var pr = await ProcessEx.RunAsync(pf,$"-Cc -v2c -c {a.snmp_public_string} {a.ip} {a.snmp_oid}");
                        
                        
                        if (pr.StandardOutput.Length != 0)
                        {
                            pr.StandardOutput.ToList().ForEach(d =>
                            {
                                try
                                {
                                    var test = d.Split(" = ");
                                    var id = test[0];
                                    var data = test[1].Replace("INTEGER: ", "");
                                    
                                    var resultParse = id.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
                                    if (!ports.Contains(data) && resultParse[13] != "1" && resultParse[13] != "192" &&
                                        resultParse[13] != "255")
                                    {
                                        var mac =
                                            Convert.ToString(Convert.ToInt32(resultParse[14]), 16).PadLeft(2, '0') +
                                            ":" +
                                            Convert.ToString(Convert.ToInt32(resultParse[15]), 16).PadLeft(2, '0') +
                                            ":" +
                                            Convert.ToString(Convert.ToInt32(resultParse[16]), 16).PadLeft(2, '0') +
                                            ":" + 
                                            Convert.ToString(Convert.ToInt32(resultParse[17]), 16).PadLeft(2, '0') + 
                                            ":" +
                                            Convert.ToString(Convert.ToInt32(resultParse[18]), 16).PadLeft(2, '0') +
                                            ":" + 
                                            Convert.ToString(Convert.ToInt32(resultParse[19]), 16).PadLeft(2, '0');
                                        
                                        var sw = a.ip;
                                        var swport = (Convert.ToInt32(data) - a.port_offset);
                                        var vlan = Convert.ToInt32(resultParse[13]);

                                        mainListBigData.Add(new MainPortmapModel
                                        {
                                            mac = mac,
                                            ip = string.Empty,
                                            sw = sw,
                                            swport = swport,
                                            vlan = vlan,
                                            nau_login = string.Empty,
                                            typeos = "Linux",
                                            typeip = string.Empty,
                                            onlinestatus = "online",
                                            updatetime = updated,
                                            placeid = placeid
                                        });
                                        hostsCount++;
                                    }
                                }

                                catch (Exception ex)
                                {
                                    Console.WriteLine(d + " - " + ex.Message);
                                }
                            });
                        }
                        
                        pr.Dispose();
                    });
                    await Task.WhenAll(tasks);

                    await _webbrDatabase.ExecuteAsync(@"INSERT dashboard_main_grafana_naumen_hosts (hosts_count, placeid, updated) VALUES(@hosts_count, @placeid, @updated) ON DUPLICATE KEY UPDATE hosts_count=@hosts_count, placeid=@placeid, updated=@updated", new {hosts_count = hostsCount, placeid, updated});

                    return mainListBigData;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Dhcp
        private async Task<List<MainPortmapModel>> Dhcp(List<MainPortmapModel> dataList, string dhcpServer, string searchQuery)
        {
            try
            {
                var dhcpLeaseCommand = _webbrSsh.SshCommandExecute(dhcpServer, 22, "root",searchQuery + @" | awk -F""[ ;]"" '/lease|.*ethernet|.*ends/ {print $2 $5 "" "" $6}' | xargs -n 4");
                if(string.IsNullOrEmpty(dhcpLeaseCommand)) return null;

                // Добавление в список dhcpLease
                var dhcpLeaseList = new List<DhcpModel>();

                var dhcpLeaseLog = dhcpLeaseCommand.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries).ToList();

                var regex = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}[ ]\d{1,4}\/\d{1,2}\/\d{1,2}[ ]\d{1,2}\:\d{1,2}\:\d{1,2}[ ]\S{1,2}\:\S{1,2}\:\S{1,2}\:\S{1,2}\:\S{1,2}\:\S{1,2}");

                var lst = dhcpLeaseLog.Where(l => regex.IsMatch(l)).Distinct();

                foreach (var str in lst)
                {
                    var parts = str.Split(new[] {' '});
                    dhcpLeaseList.Add(new DhcpModel
                    {
                        Ip = parts[0],
                        Mac = parts[3],
                        DhcpEndTime = DateTime.ParseExact(parts[1] + " " + parts[2], "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture)
                    });
                }

                var dhcpLeaseArray = dhcpLeaseList
                    .Where(i => DateTime.Compare(i.DhcpEndTime, DateTime.Now.AddDays(-5)) > 0)
                    .Select(x => new {ip = x.Ip, mac = x.Mac})
                    .Distinct()
                    .GroupBy(i => i.mac)
                    .Select(i => i.Last())
                    .ToList();

                dhcpLeaseArray.ForEach(i => dataList.ForEach(j =>
                {
                    if (!string.Equals(i.mac, j.mac, StringComparison.OrdinalIgnoreCase)) return;

                    dataList.First(d => string.Equals(d.mac, j.mac, StringComparison.OrdinalIgnoreCase)).ip = i.ip;
                    dataList.First(d => string.Equals(d.mac, j.mac, StringComparison.OrdinalIgnoreCase)).typeip = "Dynamic";
                }));

                const string patternconf = @"^host.*{hardware.*ethernet.*([a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2});.*fixed-address.*?\s([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})";

                var dhcpdConfCommand = _webbrSsh.SshCommandExecute(dhcpServer, 22, "root","cat /etc/dhcp/dhcpd.conf");
                var dhcpdConfArray = dhcpdConfCommand.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries).ToList();

                // Добавление в список dhcpdConfList
                var dhcpdConfList = new List<DhcpModel>();
                dhcpdConfArray.ForEach(l => dhcpdConfList.AddRange(Regex.Matches(l, patternconf).Select(m => new DhcpModel
                {
                    Ip = m.Groups[2].Value,
                    Mac = m.Groups[1].Value
                })));

                dhcpdConfList.ForEach(i => dataList.ForEach(j =>
                {
                    if (!string.Equals(i.Mac, j.mac, StringComparison.OrdinalIgnoreCase)) return;

                    dataList.First(d => string.Equals(d.mac, j.mac, StringComparison.OrdinalIgnoreCase)).ip = i.Ip;
                    dataList.First(d => string.Equals(d.mac, j.mac, StringComparison.OrdinalIgnoreCase)).typeip = "Static";
                }));

                return dataList;
            }
            catch{}

            return dataList;
        }
        #endregion

        #region Naumen
        private async Task<List<MainPortmapModel>> Naumen(List<MainPortmapModel> dataList, string naumenServer, string placeShortDescription, int placeid)
        {
            try
            {
                var naumenHostsCommand = _webbrSsh.SshCommandExecute(naumenServer, 22, "root",$@"netstat -n | egrep "".*${naumenServer}:3242.*ESTABLISHED"" | wc -l");
                var naumen = 0;
                if(!string.IsNullOrEmpty(naumenHostsCommand)) naumen = int.Parse(naumenHostsCommand);

                var naumenCommand = _webbrSsh.SshCommandExecute(naumenServer, 22, "root",@"naucore show connections | awk -F ""[ :]"" '$4 ~ /client/ {system(""naucore show connection ""$2"" & "")}' | perl -n -e '/.*\sas\s\<(.*)\>(\s)with*|.*remote_address=(.*):.*(\n)/ && print ""$1$2$3$4""'");
                if(string.IsNullOrEmpty(naumenCommand)) naumenCommand = _webbrSsh.SshCommandExecute(naumenServer, 22, "root",@"naucore show connections | awk -F ""[ :]"" '$4 ~ /client/ {system(""naucore show connection ""$2"" & "")}' | perl -n -e '/.*\sas\s\<(.*)\>(\s)with*|.*remote_address=(.*):.*(\n)/ && print ""$1$2$3$4""'");

                if (!string.IsNullOrEmpty(naumenCommand))
                {
                    var naumenList = new List<NaumenListModel>();

                    var naumenCommandList = naumenCommand?.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
                    var naumenCommandListResult = naumenCommandList?.Where(x => Regex.IsMatch(x, @"^[a-zA-Z]+\.?([a-zA-Z]+\.)?\w+\s\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b"));

                    foreach (var line in naumenCommandListResult)
                    {
                        var parts = line.Split(new[] {' '});
                        naumenList.Add(new NaumenListModel
                        {
                            Ip = parts.LastOrDefault(),
                            Login = parts.FirstOrDefault()
                        });
                    }

                    dataList.ForEach(i => naumenList.ForEach(j =>
                    {
                        if (!string.Equals(i.ip, j.Ip)) return;

                        dataList.First(d => d.ip == j.Ip).nau_login = j.Login;
                    }));

                    // Заливаем количество включенных РМ в Graphite
                    var graphiteClient = new GraphiteClient("10.254.6.86") {HttpApiPort = 80, UseSsl = false};
                    await graphiteClient.SendAsync(new Datapoint($"Naumen.Online_{placeShortDescription}", naumen, DateTime.Now));

                    await _webbrDatabase.ExecuteAsync(@"INSERT dashboard_main_grafana_naumen_hosts (naumen, placeid) VALUES(@naumen, @placeid) ON DUPLICATE KEY UPDATE placeid=@placeid, naumen=@naumen", new {placeid, naumen});
                    return dataList;
                }
            }
            catch{}

            return dataList;
        }
        #endregion

        #region Transaction
        private async Task Transaction(IReadOnlyCollection<MainPortmapModel> dataList, int placeid)
        {
            await _webbrDatabase.TransactionAsync($@"UPDATE portmap SET onlinestatus='offline' WHERE placeid={placeid}");

            // Добавляем отдельной транзакцией список с пустыми IP-адресами, не меняя поле IP
            var listWhereIpEmpty = dataList.Where(x => string.IsNullOrEmpty(x.ip));
            const string queryWhereIpEmpty = @"
INSERT portmap (mac, ip, sw, swport, vlan, naumenlogin, typeos, typeip, onlinestatus, updatetime, placeid)
VALUES (@mac, @ip, @sw, @swport, @vlan, @nau_login, @typeos, @typeip, @onlinestatus, @updatetime, @placeid)
ON DUPLICATE KEY UPDATE mac=@mac, sw=@sw, swport=@swport, vlan=@vlan, naumenlogin=@nau_login, typeos=@typeos, typeip=@typeip, onlinestatus=@onlinestatus, updatetime=@updatetime, placeid=@placeid";
            await _webbrDatabase.TransactionAsync(queryWhereIpEmpty, listWhereIpEmpty);

            // Добавляем отдельной транзакцией список с IP-адресами, меняя поле IP
            var listWhereIpNotEmpty = dataList.Where(x => !string.IsNullOrEmpty(x.ip));
            const string queryWhereIpNotEmpty = @"
INSERT portmap (mac, ip, sw, swport, vlan, naumenlogin, typeos, typeip, onlinestatus, updatetime, placeid)
VALUES (@mac, @ip, @sw, @swport, @vlan, @nau_login, @typeos, @typeip, @onlinestatus, @updatetime, @placeid)
ON DUPLICATE KEY UPDATE mac=@mac, ip=@ip, sw=@sw, swport=@swport, vlan=@vlan, naumenlogin=@nau_login, typeos=@typeos, typeip=@typeip, onlinestatus=@onlinestatus, updatetime=@updatetime, placeid=@placeid";
            await _webbrDatabase.TransactionAsync(queryWhereIpNotEmpty, listWhereIpNotEmpty);
        }
        #endregion

        #region Caching
        private async Task Caching()
        {
            const string query = @"
SELECT map.mac, map.rm, map.ip, map.sw, map.swport, map.vlan, map.image, map.naumenlogin as nau_login,
       nau.id_employee as nau_uuid, nau.first_name as nau_first_name, nau.middle_name as nau_middle_name, nau.last_name as nau_last_name, nau.department as nau_department,
       nau.roles as nau_roles, nau.projects as nau_projects,
       nau.location_name as nau_location_name, nau.profile_name as nau_profile_name, nau.creation_time as nau_creation_time, nau.skills as nau_skills,
       users.user_username as win_user_username, users.user_login as win_user_login, users.user_department as win_user_department, users.user_description as win_user_description, users.user_city as win_user_city, 
       comp.computer_name as win_computer_name, comp.computer_logon_datetime as win_computer_logon_datetime, comp.computer_power_datetime as win_computer_power_datetime,
       inv.motherboard as inv_motherboard, inv.cpu as inv_cpu, inv.ram as inv_ram, inv.hdd as inv_hdd, inv.monitor as inv_monitor,
       CASE WHEN comp.computer_os IS NULL THEN 'Linux' ELSE comp.computer_os END AS typeos,
       map.typeip, map.onlinestatus, map.updatetime, map.placeid FROM portmap map
    LEFT JOIN webbr.inventory inv ON map.mac = inv.mac
    LEFT JOIN webbr.domain_computers comp ON map.mac = comp.computer_mac AND map.ip = comp.computer_ip
    LEFT JOIN webbr.domain_users users ON comp.computer_login = users.user_login
    LEFT JOIN webbr.naumen_employee nau ON map.naumenlogin = nau.login
WHERE comp.computer_logon_datetime > now() - INTERVAL 30 DAY AND comp.computer_power_datetime > now() - INTERVAL 30 DAY OR comp.computer_logon_datetime IS NULL";

            var list = await _webbrDatabase.QueryAsync<MainPortmapModel>(query);
            if (list.Count != 0) _cache.Set("portmap", list,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
        }
        #endregion
    }
}