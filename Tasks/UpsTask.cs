using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using RecurrentTasks;
using RunProcessAsTask;
using Webbr.Extensions;
using Webbr.Hubs;
using Webbr.Models.TasksModel;

namespace Webbr.Tasks
{
    public class UpsTask : IRunnable
    {
        #region Field
        private IHubClients Clients { get; }
        private readonly IMemoryCache _cache;
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public UpsTask(IWebbrDatabase webbrDatabase, IMemoryCache memoryCache, IHubContext<WebbrHub> context)
        {
            _cache = memoryCache;
            Clients = context.Clients;
            _webbrDatabase = webbrDatabase;
        }
        #endregion

        
        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            await UpsData();
        }


        #region UpsData
        private async Task UpsData()
        {
            var dbResult = await _webbrDatabase.QueryAsync<UpsTaskDbModel>($"SELECT * FROM dashboard_main_ups WHERE enable=1");
            var dbResultList = dbResult.ToList();
            
            var dataList = new List<UpsTaskDbModel>();

            var tasks = dbResultList.Select(async a =>
            {
                var prCount = 0;
                var pf = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "/usr/bin/snmpbulkwalk" : @"C:\SNMP\bin\snmpbulkwalk.exe";
                var pr = await ProcessEx.RunAsync(pf,$"-Cc -v2c -c {a.snmp_public_string} {a.ip} {a.snmp_oid}");
                if(pr.ExitCode != 0 && prCount != 2)
                {
                    prCount++;
                    pr = await ProcessEx.RunAsync(pf,$"-Cc -v2c -c {a.snmp_public_string} {a.ip} {a.snmp_oid}");
                }
                
                if (pr.StandardOutput.Length != 0)
                {
                    var inputVoltage = 0;
                    var outputVoltage = 0;
                    var outputPercentLoad = 0;
                    var batteryTemperature = 0;
                    var estimatedChargeRemaining = 0;
                    var secondsOnBattery = 0;
                    var estimatedMinutesRemaining = 0;
                    
                    pr.StandardOutput.ToList().ForEach(d =>
                    {
                        try
                        {
                            var test = d.Split(" = ");
                            var id = test[0].Replace("iso", "1");
                            var data = test[1].Replace("INTEGER: ", "");
                            
                            if (id == a.snmp_oid_input_voltage) inputVoltage = int.Parse(data);
                            if (id == a.snmp_oid_output_voltage) outputVoltage = int.Parse(data);
                            if (id == a.snmp_oid_output_percent_load) outputPercentLoad = int.Parse(data);
                            if (id == a.snmp_oid_battery_temperature) batteryTemperature = int.Parse(data);
                            if (id == a.snmp_oid_estimated_charge_remaining) estimatedChargeRemaining = int.Parse(data);
                            if (id == a.snmp_oid_seconds_on_battery) secondsOnBattery = int.Parse(data);
                            if (id == a.snmp_oid_estimated_minutes_remaining) estimatedMinutesRemaining = int.Parse(data);
                        }

                        catch (Exception ex)
                        {
                            Console.WriteLine(d + " - " + ex.Message);
                        }
                    });
                    
                    dataList.Add(new UpsTaskDbModel
                    {
                        ip = a.ip,
                        name = a.name,
                        input_vol = inputVoltage,
                        output_vol = outputVoltage,
                        ups_load = outputPercentLoad,
                        battery_temp = batteryTemperature,
                        battery_capacity = estimatedChargeRemaining,
                        battery_second = secondsOnBattery,
                        battery_min_remaining = estimatedMinutesRemaining,
                        tooltip = a.tooltip,
                        placeid = a.placeid,
                        updated = DateTime.Now.ToString("O")
                    });
                }
                    
                pr.Dispose();
            });
            await Task.WhenAll(tasks);
            
            _cache.Set("dashboard_ups", dataList.OrderBy(x => x.placeid), new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            await Clients.All.SendAsync("dashboard_ups", dataList.OrderBy(x => x.placeid));
            
            const string upsQuery = @"UPDATE dashboard_main_ups SET input_vol=@input_vol, output_vol=@output_vol, ups_load=@ups_load, battery_temp=@battery_temp, battery_capacity=@battery_capacity, battery_second=@battery_second, battery_min_remaining=@battery_min_remaining, updated=@updated WHERE ip=@ip";
            await _webbrDatabase.TransactionAsync(upsQuery, dataList);
        }
        #endregion
    }
}