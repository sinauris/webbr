<template>
    <b-card no-body :header-class="{'card-header-progress': true, 'cardAlarm': alarmComputed }" :class="{'btnHidden':true, 'borderAlarm': alarmComputed }">
        <div slot="header" class="text-center font-weight-bold">
            <div style="font-size:14px">
                <span @click="alarmEnable = !alarmEnable">
                    <span v-if="alarmEnable" style="cursor: pointer;" ><font-awesome-icon icon="university"/> МТС Банк</span>
                    <span v-else style="cursor: pointer; color: darkgrey"><font-awesome-icon icon="university"/> МТС Банк</span>
                </span>
            </div>
        </div>

        <div class="text-center" style="cursor: default">
            <dashboard_mts_mq :mtsMqTunnelData="mtsMqTunnelData" :mtsMqChannelData="mtsMqChannelData" :mtsMqQueueData="mtsMqQueueData" />
            <dashboard_mts_mgw :mtsMgwAgentData="mtsMgwAgentData" :mtsMgwJobData="mtsMgwJobData" />
            <dashboard_mts_import :mtsImportData="mtsImportData" />
        </div>
    </b-card>
</template>

<script>
    import Dashboard_mts_mq from "../dashboard_addon/dashboard_mts_addon/dashboard_mts_mq";
    import Dashboard_mts_mgw from "../dashboard_addon/dashboard_mts_addon/dashboard_mts_mgw";
    import Dashboard_mts_import from "../dashboard_addon/dashboard_mts_addon/dashboard_mts_import";
    
    export default {
        props: ["mtsMqTunnelData", "mtsMqChannelData", "mtsMqQueueData", "mtsImportData", "mtsMgwAgentData", "mtsMgwJobData"],
        components: {
            Dashboard_mts_mq,
            Dashboard_mts_mgw,
            Dashboard_mts_import
        },
        data() {
            return {
                mqTunnelCollapseShow: false,
                mqChannelCollapseShow: false,
                mqQueueCollapseShow: false,
                mqImportCollapseShow: false,

                alarmDetected: false,
                alarmEnable: true
            }
        },
        computed: {
            alarmComputed() {
                if(!this.alarmEnable) return false;
                if(this.mqSshResult === false || this.mqPingResult === false || this.importSshResult === false || this.importScriptResult === false || this.mgwAgentsResult === false || this.mqChannelResult === false || this.mqQueueResult === false) return true;
            },

            mqSshResult() {
                if(this.mtsMqTunnelData) {
                    let result = true;
                    this.mtsMqTunnelData.forEach(x => { if(x.ssh_result.includes('FAIL')) {
                        result = false;
                        this.mqTunnelCollapseShow = true;
                    }});
                    return result;
                }
            },
            mqPingResult() {
                if(this.mtsMqTunnelData) {
                    let result = true;
                    this.mtsMqTunnelData.forEach(x => { if(x.ping_result.includes('FAIL')) {
                        result = false;
                        this.mqTunnelCollapseShow = true;
                    }});
                    return result;
                }
            },
            importSshResult() {
                 if(this.mtsImportData) {
                     let result = true;
                     this.mtsImportData.forEach(x => { if(x.ssh_result === 'FAIL') {
                         result = false;
                         this.mqImportCollapseShow = true;
                     }});
                     return result;
                 }
            },
            importScriptResult() {
                if(this.mtsImportData) {
                    let result = true;
                    this.mtsImportData.forEach(x => { if(x.import_result === 'STOPPED') {
                        result = false;
                        this.mqImportCollapseShow = true;
                    }});
                    return result;
                }
            },
            mgwAgentsResult() {
                if(this.mtsMgwAgentData) {
                    let result = true;
                    this.mtsMgwAgentData.forEach(x => { if(x.agent_status !== 'RUNNING') {
                        result = false;
                    }});
                    return result;
                }
            },
            mqChannelResultFiltered() {
                if(this.mtsMqChannelData) return [...new Set(this.mtsMqChannelData.map(item => item.mq_ip))];
            },
            mqChannelResult() {
                if(this.mtsMqChannelData) {
                    let result = true;
                    this.mtsMqChannelData.forEach(x => { if(x.channel_status !== 'Running') {
                        result = false;
                        this.mqChannelCollapseShow = true;
                    }});
                    return result;
                }
            },
            mqQueueResultFiltered() {
                if(this.mtsMqQueueData) return [...new Set(this.mtsMqQueueData.map(item => item.mq_ip))];
            },
            mqQueueResult() {
                if(this.mtsMqQueueData) {
                    let result = true;
                    this.mtsMqQueueData.forEach(x => { if(x.queue_depth > 20) {
                        result = false;
                        this.mqQueueCollapseShow = true;
                    }});
                    return result;
                }
            }
        }
    }
</script>

<style scoped>

</style>