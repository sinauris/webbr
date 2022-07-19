<template>
    <div>
        <b-container fluid>
            <br/>
            <b-row>
                <b-col cols="12" sm="12" md="12" lg="12" xl="4" class="pr-2">
                    <dashboard_grafana :exception="grafana_error" :sections="grafana_data.sections" :servers='grafana_data.servers'/>
                </b-col>

                <b-col cols="12" sm="12" md="4" lg="3" xl="2" class="pr-2">
                    <dashboard_mts
                        :mtsMqTunnelData="mts_mq_tunnel_data"
                        :mtsMqChannelData="mts_mq_channel_data"
                        :mtsMqQueueData="mts_mq_queue_data"
                        :mtsImportData="mts_import_data"
                        :mtsMgwAgentData="mts_mgw_agent_data"
                        :mtsMgwJobData="mts_mgw_job_data"
                    />
                </b-col>

                <b-col cols="12" sm="12" md="8" lg="9" xl="2" class="pr-2">
                    <dashboard_zabbix :exception="zabbix_error" :items="zabbix_data"/>
                </b-col>

                <b-col cols="12" sm="12" md="9" lg="12" xl="4">
                    <b-row>
                        <b-col cols="12" sm="6" md="6" lg="6" xl="6" class="pr-2">
                            <dashboard_hosts class="mb-2" :items="hosts_data"/>
                            <dashboard_naumen :items="naumen_data"/>
                        </b-col>
                        <b-col cols="12" sm="6" md="6" lg="6" xl="6">
                            <dashboard_ups :exception="ups_error" :items="ups_data"/>
                        </b-col>
                    </b-row>
                </b-col>
            </b-row>
        </b-container>
    </div>
</template>

<script>
    import { USER_GET } from '../store/actions/user'
    import Dashboard_grafana from "./dashboard_addon/dashboard_grafana";
    import Dashboard_mts from "./dashboard_addon/dashboard_mts";
    import Dashboard_zabbix from "./dashboard_addon/dashboard_zabbix";
    import Dashboard_naumen from "./dashboard_addon/dashboard_naumen";
    import Dashboard_ups from "./dashboard_addon/dashboard_ups";
    import Dashboard_hosts from "./dashboard_addon/dashboard_hosts";

    export default {
        components: {
            Dashboard_grafana,
            Dashboard_mts,
            Dashboard_zabbix,
            Dashboard_naumen,
            Dashboard_ups,
            Dashboard_hosts
        },
        data() {
            return {
                zabbixExpand: false,

                grafana_data: [],
                grafana_error: '',

                zabbix_data: [],
                zabbix_error: '',

                ups_data: [],
                ups_error: '',

                telephony_data: [],
                telephony_error: '',

                hosts_data: [],
                naumen_data: [],

                mts_mq_tunnel_data: [],
                mts_mq_channel_data: [],
                mts_mq_queue_data: [],
                mts_mgw_agent_data: [],
                mts_mgw_job_data: [],
                mts_import_data: []
            };
        },
        methods: {
            async grafanaLoad() {
                if(this.$store.getters.getSignalrState) await this.$signalR.invoke('GetValues', 'dashboard_grafana');
                else {
                    await this.$store.dispatch(USER_GET, '/api/dashboard/getgrafanadata')
                        .then((resp) => this.grafana_data = resp.data)
                        .catch((err) => this.grafana_error = err.response);
                }
            },

            async zabbixLoad() {
                if(this.$store.getters.getSignalrState) await this.$signalR.invoke('GetValues', 'dashboard_zabbix');
                else {
                    await this.$store.dispatch(USER_GET, '/api/dashboard/getzabbixdata')
                        .then((resp) => this.zabbix_data = resp.data)
                        .catch((err) => this.zabbix_error = err.response);
                }
            },

            async upsLoad() {
                if(this.$store.getters.getSignalrState) await this.$signalR.invoke('GetValues', 'dashboard_ups');
                else await this.$store.dispatch(USER_GET, '/api/dashboard/getupsdata').then((resp) => this.ups_data = resp.data);
            },

            async naumenLoad() {
                if(this.$store.getters.getSignalrState) await this.$signalR.invoke('GetValues', 'dashboard_naumen');
                else await this.$store.dispatch(USER_GET, '/api/dashboard/getnaumendata').then((resp) => {
                    this.hosts_data = resp.data.naumenHostsList;
                    this.naumen_data = resp.data.naumenLicenseList;
                });
            },

            async mtsMqTunnelLoad() {
                if(this.$store.getters.getSignalrState) await this.$signalR.invoke('GetValues', 'dashboard_mts_tunnel');
                else await this.$store.dispatch(USER_GET, '/api/dashboard/getmtsmqtunneldata').then((resp) => this.mts_mq_tunnel_data = resp.data);
            },
            async mtsMqChannelLoad() {
                if(this.$store.getters.getSignalrState) await this.$signalR.invoke('GetValues', 'dashboard_mts_channel');
                else await this.$store.dispatch(USER_GET, '/api/dashboard/getmtsmqchanneldata').then((resp) => this.mts_mq_channel_data = resp.data);
            },
            async mtsMqQueueLoad() {
                if(this.$store.getters.getSignalrState) await this.$signalR.invoke('GetValues', 'dashboard_mts_queue');
                else await this.$store.dispatch(USER_GET, '/api/dashboard/getmtsmqqueuedata').then((resp) => this.mts_mq_queue_data = resp.data);
            },
            async mtsMgwAgentLoad() {
                if(this.$store.getters.getSignalrState) await this.$signalR.invoke('GetValues', 'dashboard_mts_agent');
                else await this.$store.dispatch(USER_GET, '/api/dashboard/getmtsmgwagentdata').then((resp) => this.mts_mgw_agent_data = resp.data);
            },
            
            async mtsMgwJobLoad() {
                if(this.$store.getters.getSignalrState) await this.$signalR.invoke('GetValues', 'dashboard_mts_jobs');
                else await this.$store.dispatch(USER_GET, '/api/dashboard/getmtsmgwjobdata').then((resp) => this.mts_mgw_job_data = resp.data);
            },
            async mtsImportLoad() {
                if(this.$store.getters.getSignalrState) await this.$signalR.invoke('GetValues', 'dashboard_mts_import');
                else await this.$store.dispatch(USER_GET, '/api/dashboard/getmtsimportdata').then((resp) => this.mts_import_data = resp.data);
            }
        },

        async mounted () {
            this.$signalR.on('dashboard_grafana', async (data) => { if(data) this.grafana_data = data });
            this.$signalR.on('dashboard_zabbix', async (data) => { if(data) this.zabbix_data = data });
            this.$signalR.on('dashboard_ups', async (data) => { if(data) this.ups_data = data });
            this.$signalR.on('dashboard_naumen', async (data) => { if(data) this.hosts_data = data.naumenHostsList; this.naumen_data = data.naumenLicenseList ? data.naumenLicenseList : [] });
            
            this.$signalR.on('dashboard_mts_tunnel', async (data) => { if(data) this.mts_mq_tunnel_data = data });
            this.$signalR.on('dashboard_mts_channel', async (data) => { if(data) this.mts_mq_channel_data = data });
            this.$signalR.on('dashboard_mts_queue', async (data) => { if(data) this.mts_mq_queue_data = data });
            this.$signalR.on('dashboard_mts_agent', async (data) => { if(data) this.mts_mgw_agent_data = data });
            this.$signalR.on('dashboard_mts_jobs', async (data) => { if(data) this.mts_mgw_job_data = data });
            this.$signalR.on('dashboard_mts_import', async (data) => { if(data) this.mts_import_data = data });
        },

        async created() {
            await Promise.all([
                this.grafanaLoad(),
                this.zabbixLoad(),
                this.naumenLoad(),
                this.upsLoad(),
                
                this.mtsMqTunnelLoad(),
                this.mtsMqChannelLoad(),
                this.mtsMqQueueLoad(),
                this.mtsMgwAgentLoad(),
                this.mtsMgwJobLoad(),
                this.mtsImportLoad()
            ]);

            
            // await this.grafanaLoad();
            // await this.mtsMqTunnelLoad();
            // await this.mtsMqChannelLoad();
            // await this.mtsMqQueueLoad();
            // await this.mtsMgwAgentLoad();
            // await this.mtsMgwJobLoad();
            // await this.mtsImportLoad();
            // await this.zabbixLoad();
            // await this.naumenLoad();
            // await this.upsLoad();
        }
    };
</script>

<style scoped>

</style>