using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using IBM.WMQ;
using IBM.WMQ.PCF;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using RecurrentTasks;
using Webbr.Extensions;
using Webbr.Hubs;
using Webbr.Models.MtsModel;

namespace Webbr.Tasks
{
    public class MtsChannelTask : IRunnable
    {
        #region Field
        private IHubClients Clients { get; }
        private readonly IMemoryCache _cache;
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public MtsChannelTask(IWebbrDatabase webbrDatabase, IMemoryCache memoryCache, IHubContext<WebbrHub> context)
        {
            _cache = memoryCache;
            Clients = context.Clients;
            _webbrDatabase = webbrDatabase;
        }
        #endregion


        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            await CheckMqChannel();
        }


        #region CheckMqChannel
        private async Task CheckMqChannel()
        {
            var mq = await _webbrDatabase.QueryAsync<dynamic>($"SELECT ip, queue_manager_name, description FROM dashboard_main_mts_mq_tunnel");

            string[] mqQueueNames = { "FROM.MBRD", "SYSTEM.CLUSTER.TRANSMIT.QUEUE" };
            var mqChannelsList = new List<MqChannelModel>();
            var mqQueuesList = new List<MqQueueModel>();

            foreach (var m in mq)
            {
                var mqIp = m.ip;
                var queueManagerName = m.queue_manager_name;

                var properties = new Hashtable { { MQC.CHANNEL_PROPERTY, "SYSTEM.ADMIN.SVRCONN" }, { MQC.HOST_NAME_PROPERTY, mqIp } };
                var mqqm = new MQQueueManager(queueManagerName, properties);
                var agent = new PCFMessageAgent(mqqm);
                var pfc = new PCFMessage(MQC.MQCMD_INQUIRE_CHANNEL_STATUS);
                pfc.AddParameter(MQC.MQCACH_CHANNEL_NAME, "TO.MQ*");
                pfc.AddParameter(CMQCFC.MQIACH_CHANNEL_INSTANCE_ATTRS, new[]
                {
                    CMQCFC.MQCACH_CHANNEL_NAME,
                    CMQCFC.MQCACH_LOCAL_ADDRESS,
                    CMQCFC.MQCACH_CONNECTION_NAME
                });

                foreach (var i in agent.Send(pfc))
                {
                    mqChannelsList.Add(new MqChannelModel
                    {
                        mq_ip = mqIp,
                        description = m.description,
                        queue_manager_name = queueManagerName,
                        channel_name = i.GetStringParameterValue(CMQCFC.MQCACH_CHANNEL_NAME).Replace(" ",string.Empty),
                        channel_status = GetMqChannelStatus(i.GetIntParameterValue(CMQCFC.MQIACH_CHANNEL_STATUS)),
                        channel_state = GetMqChannelState(i.GetIntParameterValue(MQC.MQIACH_CHANNEL_SUBSTATE)),
                        channel_connection_name = i.GetStringParameterValue(CMQCFC.MQCACH_CONNECTION_NAME).Replace(" ",string.Empty),
                        channel_local_ip = i.GetStringParameterValue(CMQCFC.MQCACH_LOCAL_ADDRESS).Replace(" ",string.Empty),
                        channel_rqm = i.GetStringParameterValue(MQC.MQCA_REMOTE_Q_MGR_NAME).Replace(" ",string.Empty),
                        updated = DateTime.Now.ToString("O")
                    });
                }
                agent.Disconnect();

                foreach (var qn in mqQueueNames)
                {
                    mqQueuesList.Add(new MqQueueModel
                    {
                        mq_ip = mqIp,
                        description = m.description,
                        queue_manager_name = queueManagerName,
                        queue_name = qn.Replace("SYSTEM.CLUSTER.", string.Empty),
                        queue_depth = GetQueueDepth(mqqm, qn),
                        updated = DateTime.Now.ToString("O")
                    });

//                    mqMsgList.AddRange(GetQueueMsg(mq_ip, mqqm, queue_manager_name, qn));
                }

                mqqm.Disconnect();
            }
            
            _cache.Set("dashboard_mts_channel", mqChannelsList, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            await Clients.All.SendAsync("dashboard_mts_channel", mqChannelsList);
            
            _cache.Set("dashboard_mts_queue", mqQueuesList, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            await Clients.All.SendAsync("dashboard_mts_queue", mqQueuesList);

            const string transactionChannelQuery = @"
INSERT dashboard_main_mts_mq_channel (mq_ip, queue_manager_name, channel_name, channel_connection_name, channel_status, channel_state, channel_local_ip, channel_rqm, updated)
VALUES(@mq_ip, @queue_manager_name, @channel_name, @channel_connection_name, @channel_status, @channel_state, @channel_local_ip, @channel_rqm, @updated)
ON DUPLICATE KEY UPDATE mq_ip=@mq_ip, queue_manager_name=@queue_manager_name, channel_name=@channel_name, channel_connection_name=@channel_connection_name, channel_status=@channel_status, channel_state=@channel_state, channel_local_ip=@channel_local_ip, channel_rqm=@channel_rqm, updated=@updated";
            await _webbrDatabase.TransactionAsync(transactionChannelQuery, mqChannelsList);

            const string transactionQueueQuery = @"
INSERT dashboard_main_mts_mq_queue (mq_ip, queue_manager_name, queue_name, queue_depth, updated)
VALUES(@mq_ip, @queue_manager_name, @queue_name, @queue_depth, @updated)
ON DUPLICATE KEY UPDATE mq_ip=@mq_ip, queue_manager_name=@queue_manager_name, queue_name=@queue_name, queue_depth=@queue_depth, updated=@updated";
            await _webbrDatabase.TransactionAsync(transactionQueueQuery, mqQueuesList);
        }
        #endregion
        
        #region GetQueueDepth
        private static int GetQueueDepth(MQQueueManager queuemgr, string queue)
        {
            var q = queuemgr.AccessQueue(queue, MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING + MQC.MQOO_INQUIRE + MQC.MQOO_BROWSE);
            var depth = q.CurrentDepth;

            q.Close();
            return depth;
        }
        #endregion

        #region GetQueueMsg
        private static List<MqMsgModel> GetQueueMsg(string mqIp, MQQueueManager queuemgr, string queueManagerName, string queue)
        {
            var mqMsgList = new List<MqMsgModel>();
            var q = queuemgr.AccessQueue(queue, MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING + MQC.MQOO_INQUIRE + MQC.MQOO_BROWSE);

            for (var i = 1; i <= q.CurrentDepth; i++)
            {
                var message = new MQMessage();

                try
                {
                    q.Get(message);
                    if (message.MessageLength != 0)
                    {
                        var doc = XDocument.Parse(message.ReadString(message.MessageLength));

                        mqMsgList.Add(new MqMsgModel
                        {
                            mq_ip = mqIp,
                            queue_manager_name = queueManagerName,
                            queue_name = queue,
                            msg_request_id = doc.Descendants("requestId").First().Value,
                            msg_length = message.MessageLength,
                            msg_put_datetime = message.PutDateTime.ToString("O"),
                            updated = DateTime.Now.ToString("O")
                        });
                        message.ClearMessage();
                    }
                }
                catch
                {
                    // ignored
                }
            }

            q.Close();
            return mqMsgList;
        }
        #endregion


        #region GetMqChannelStatus
        private static string GetMqChannelStatus(int status)
        {
            switch (status)
            {
                case MQC.MQCHS_BINDING: return "Binding";
                case MQC.MQCHS_DISCONNECTED: return "Disconnected";
                case MQC.MQCHS_INACTIVE: return "Inactive";
                case MQC.MQCHS_INITIALIZING: return "Initializing";
                case MQC.MQCHS_PAUSED: return "Paused";
                case MQC.MQCHS_REQUESTING: return "Requesting";
                case MQC.MQCHS_RETRYING: return "Retrying";
                case MQC.MQCHS_RUNNING: return "Running";
                case MQC.MQCHS_STARTING: return "Starting";
                case MQC.MQCHS_STOPPED: return "Stopped";
                case MQC.MQCHS_STOPPING: return "Stopping";
                case MQC.MQCHS_SWITCHING: return "Switching";
                default: return string.Empty;
            }
        }
        #endregion

        #region GetMqChannelState
        private static string GetMqChannelState(int state)
        {
            switch (state)
            {
                case MQC.MQCHSSTATE_END_OF_BATCH: return "Endbatch";
                case MQC.MQCHSSTATE_SENDING: return "Send";
                case MQC.MQCHSSTATE_RECEIVING: return "Receive";
                case MQC.MQCHSSTATE_SERIALIZING: return "Serialize";
                case MQC.MQCHSSTATE_RESYNCHING: return "Resynch";
                case MQC.MQCHSSTATE_HEARTBEATING: return "Heartbeat";
                case MQC.MQCHSSTATE_IN_SCYEXIT: return "Scyexit";
                case MQC.MQCHSSTATE_IN_RCVEXIT: return "Rcvexit";
                case MQC.MQCHSSTATE_IN_SENDEXIT: return "Sendexit";
                case MQC.MQCHSSTATE_IN_MSGEXIT: return "Msgexit";
                case MQC.MQCHSSTATE_IN_CHADEXIT: return "Chadexit";
                case MQC.MQCHSSTATE_NET_CONNECTING: return "Netconnect";
                case MQC.MQCHSSTATE_SSL_HANDSHAKING: return "Sslhandshk";
                case MQC.MQCHSSTATE_NAME_SERVER: return "Nameserver";
                case MQC.MQCHSSTATE_IN_MQPUT: return "Mqput";
                case MQC.MQCHSSTATE_IN_MQGET: return "Mqget";
                case MQC.MQCHSSTATE_IN_MQI_CALL: return "Mqicall";
                case MQC.MQCHSSTATE_COMPRESSING: return "Compress";
                default: return string.Empty;
            }
        }
        #endregion
    }
}