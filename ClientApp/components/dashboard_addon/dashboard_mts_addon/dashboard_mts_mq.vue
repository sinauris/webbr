<template>
    <div class="m-2 text-center" style="cursor: default">
        <b-card no-body>
            <span slot="header" class="font-weight-bold">
                <b-link href="http://37.221.187.183/Техническая_дирекция/Проекты/МТС_Банк/МТС_Банк._IBM_MQ_Explorer._Инструкция_по_проверке_работоспособности_очередей" target="_blank">MQ</b-link>
            </span>

            <b-list-group flush class="m-0 p-0 border-0">
                <b-list-group-item class="m-0 p-0">
                    <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default;">
                        <b-col lg="1">
                            <font-awesome-icon v-if="!mqSshResult || !mqPingResult" icon="circle" style="color: red"/>
                            <font-awesome-icon v-else icon="circle" style="color: limegreen"/>
                        </b-col>
                        <b-col lg="10">
                            <span v-if="!mqSshResult || !mqPingResult" class="text-danger font-weight-bold" style="font-size: 14px;">Туннели</span>
                            <span v-else style="font-size: 14px;">Туннели</span>
                        </b-col>
                        <b-col lg="1">
                            <div style="cursor: pointer" @click="mqTunnelCollapseShow = !mqTunnelCollapseShow">
                                <font-awesome-icon v-if="mqTunnelCollapseShow" icon="chevron-up"/>
                                <font-awesome-icon v-else icon="chevron-down"/>
                            </div>
                        </b-col>
                    </b-row>
                    <b-collapse v-model="mqTunnelCollapseShow" id="mqTunnelCollapseShow">
                        <hr class="m-0 p-0"/>
                        <b-list-group flush>
                            <b-list-group-item v-for="v in mtsMqTunnelData" :key="v.id" class="m-0 p-0 border-bottom-0">

                                <template v-if="v.ssh_result === 'OK'">

                                    <div class="text-center" style="width: 100%; padding: 4px 10px 1px 10px; font-size: 13px; font-weight: bold; cursor: default; background-color: rgba(0, 0, 0, 0.03)">{{ v.ip }}</div>
                                    <b-list-group class="m-0 p-0">
                                        <template v-for="res in v.ping_result.split(';').filter((el) => {return el})">

                                            <b-list-group-item v-if="res.includes('FAIL')" class="m-0 p-0 border-bottom-0" style="background-color: red">
                                                <b-row align-v="center" class="m-1 text-white font-weight-bold" style="line-height: 1; cursor: default">
                                                    <b-col lg="1">
                                                        <font-awesome-icon icon="circle"/>
                                                    </b-col>
                                                    <b-col lg="7">
                                                        <span class="font-weight-bold">Недоступен</span>
                                                    </b-col>
                                                    <b-col lg="4">
                                                        {{ mqCutIpResult(res) }}
                                                    </b-col>
                                                </b-row>
                                            </b-list-group-item>
                                            <b-list-group-item v-else class="m-0 p-0 border-bottom-0">
                                                <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default">
                                                    <b-col lg="1">
                                                        <font-awesome-icon icon="circle" style="color: limegreen"/>
                                                    </b-col>
                                                    <b-col lg="7">
                                                        <span class="font-weight-bold text-success" style="font-size: 13px">Доступен</span>
                                                    </b-col>
                                                    <b-col lg="4">
                                                        <span style="font-size: 13px">{{ mqCutIpResult(res) }}</span>
                                                    </b-col>
                                                </b-row>
                                            </b-list-group-item>

                                        </template>
                                    </b-list-group>
                                </template>

                                <template v-else>
                                    <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default">
                                        <b-col lg="1">
                                            <font-awesome-icon icon="circle" style="color: red"/>
                                        </b-col>
                                        <b-col lg="7">
                                            <span class="text-danger font-weight-bold">Ошибка подключения</span>
                                        </b-col>
                                        <b-col lg="4">
                                            <div style="font-size: 14px;">{{ v.description }}</div>
                                            <div class="text-muted" style="font-size: 10px;">{{ v.ip }}</div>
                                        </b-col>
                                    </b-row>
                                    <b-alert show variant="danger" class="mt-3 mb-0 p-0">
                                        <h6 class="mt-1 alert-heading">Ошибка</h6>
                                        <p class="m-1 p-0" style="font-size: 12px;">{{ v.ssh_exception }}</p>
                                    </b-alert>
                                </template>

                            </b-list-group-item>
                        </b-list-group>
                        <b-card-footer v-if="mtsMqTunnelData.length !== 0" class="text-center p-0">
                            <div style="font-size:11px">
                                <abbr :title="$dt.fromISO(mtsMqTunnelData[0].updated)">{{ $dt.fromISO(mtsMqTunnelData[0].updated).toRelative() }}</abbr>
                            </div>
                        </b-card-footer>
                    </b-collapse>
                </b-list-group-item>

                <b-list-group-item class="m-0 p-0 border-0">
                    <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default;">
                        <b-col lg="1">
                            <font-awesome-icon v-if="!mqChannelResult" icon="circle" style="color: red"/>
                            <font-awesome-icon v-else icon="circle" style="color: limegreen"/>
                        </b-col>
                        <b-col lg="10">
                            <div v-if="!mqChannelResult" class="text-danger font-weight-bold" style="font-size: 14px;">Каналы</div>
                            <span v-else style="font-size: 14px;">Каналы</span>
                        </b-col>
                        <b-col lg="1">
                            <div style="cursor: pointer" @click="mqChannelCollapseShow = !mqChannelCollapseShow">
                                <font-awesome-icon v-if="mqChannelCollapseShow" icon="chevron-up"/>
                                <font-awesome-icon v-else icon="chevron-down"/>
                            </div>
                        </b-col>
                    </b-row>
                    <b-collapse v-model="mqChannelCollapseShow" id="mqChannelCollapseShow">
                        <hr class="m-0 p-0"/>
                        <b-list-group flush>
                            <b-list-group-item v-for="ch in mqChannelResultFiltered" :key="ch" class="m-0 p-0 border-bottom-0">
                                <div class="text-center" style="width: 100%; padding: 4px 10px 1px 10px; font-size: 13px; font-weight: bold; cursor: default; background-color: rgba(0, 0, 0, 0.03)">{{ ch }}</div>
                                <hr class="m-0 p-0"/>
                                <b-list-group flush>
                                    <template v-for="v in mtsMqChannelData"  v-if="v.mq_ip === ch">
                                        <b-list-group-item v-if="v.channel_status ==='Running'" class="m-0 p-0 border-bottom-0" style="cursor: default">
                                            <b-row align-v="center" class="m-1 font-weight-bold" style="line-height: 1">
                                                <b-col lg="1">
                                                    <font-awesome-icon icon="circle" style="color: limegreen"/>
                                                </b-col>
                                                <b-col lg="10">
                                                    <span class="text-success" style="font-size: 12px">{{ v.channel_name }}</span>
                                                </b-col>
                                                <b-col lg="1">
                                                        <span style="cursor: pointer" v-b-toggle="'mtsChannel-' + v.mq_ip + v.channel_name + v.queue_manager_name + v.channel_connection_name">
                                                            <font-awesome-icon class="when-opened" icon="chevron-up"/>
                                                            <font-awesome-icon class="when-closed" icon="chevron-down"/>
                                                        </span>
                                                </b-col>
                                            </b-row>
                                        </b-list-group-item>
                                        <b-list-group-item v-else class="m-0 p-0 text-white border-bottom-0" style="cursor: default; background: red">
                                            <b-row align-v="center" class="m-1 text-white font-weight-bold" style="line-height: 1">
                                                <b-col lg="1">
                                                    <font-awesome-icon icon="circle"/>
                                                </b-col>
                                                <b-col lg="10">
                                                    <span>{{ v.channel_name }}</span>
                                                </b-col>
                                                <b-col lg="1">
                                                        <span style="cursor: pointer" v-b-toggle="'mtsChannel-' + v.mq_ip + v.channel_name + v.queue_manager_name + v.channel_connection_name">
                                                            <font-awesome-icon class="when-opened" icon="chevron-up"/>
                                                            <font-awesome-icon class="when-closed" icon="chevron-down"/>
                                                        </span>
                                                </b-col>
                                            </b-row>
                                        </b-list-group-item>

                                        <b-collapse :id="'mtsChannel-' + v.mq_ip + v.channel_name + v.queue_manager_name + v.channel_connection_name">
                                            <b-list-group class="m-0 p-0" style="font-size: 12px">
                                                <b-list-group-item class="m-0 p-0 border-bottom-0">
                                                    <div class="m-1">
                                                        <p class="m-0 p-0 font-weight-bold">Имя канала</p> {{ v.channel_name }}
                                                    </div>
                                                </b-list-group-item>
                                                <b-list-group-item class="m-0 p-0 border-bottom-0">
                                                    <div class="m-1">
                                                        <p class="m-0 p-0 font-weight-bold">Тип канала</p>
                                                        <template v-if="v.channel_state === 'Mqget'">Кластерный отправитель</template>
                                                        <template v-else-if="v.channel_state === 'Receive'">Кластерный получатель</template>
                                                        <template v-else>{{ v.channel_state }}</template>
                                                    </div>
                                                </b-list-group-item>
                                                <b-list-group-item class="m-0 p-0 border-bottom-0">
                                                    <div class="m-1">
                                                        <p class="m-0 p-0 font-weight-bold">Состояние канала</p>
                                                        <template v-if="v.channel_status === 'Running'">Выполняется</template>
                                                        <template v-else>{{ v.channel_status }}</template>
                                                    </div>
                                                </b-list-group-item>
                                                <b-list-group-item class="m-0 p-0 border-bottom-0">
                                                    <div class="m-1">
                                                        <p class="m-0 p-0 font-weight-bold">Имя соединения</p> {{ v.channel_connection_name }}
                                                    </div>
                                                </b-list-group-item>
                                                <b-list-group-item class="m-0 p-0 border-bottom-0">
                                                    <div class="m-1">
                                                        <p class="m-0 p-0 font-weight-bold">Менеджер очередей</p> {{ v.queue_manager_name }}
                                                    </div>
                                                </b-list-group-item>
                                            </b-list-group>
                                        </b-collapse>

                                    </template>
                                </b-list-group>
                            </b-list-group-item>
                        </b-list-group>
                        <b-card-footer v-if="mtsMqChannelData.length !== 0" class="text-center p-0">
                            <div style="font-size:11px">
                                <abbr :title="$dt.fromISO(mtsMqChannelData[0].updated)">{{ $dt.fromISO(mtsMqChannelData[0].updated).toRelative() }}</abbr>
                            </div>
                        </b-card-footer>
                    </b-collapse>
                </b-list-group-item>

                <b-list-group-item class="m-0 p-0">
                    <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default">
                        <b-col lg="1">
                            <font-awesome-icon v-if="!mqQueueResult" icon="circle" style="color: red"/>
                            <font-awesome-icon v-else icon="circle" style="color: limegreen"/>
                        </b-col>
                        <b-col lg="10">
                            <span v-if="!mqQueueResult" class="text-danger font-weight-bold" style="font-size: 14px">Очереди</span>
                            <span v-else style="font-size: 14px">Очереди</span>
                        </b-col>
                        <b-col lg="1">
                            <div style="cursor: pointer" @click="mqQueueCollapseShow = !mqQueueCollapseShow">
                                <font-awesome-icon v-if="mqQueueCollapseShow" icon="chevron-up"/>
                                <font-awesome-icon v-else icon="chevron-down"/>
                            </div>
                        </b-col>
                    </b-row>
                    <b-collapse v-model="mqQueueCollapseShow" id="mqQueueCollapseShow">
                        <hr class="m-0 p-0"/>
                        <b-list-group flush>
                            <b-list-group-item v-for="qu in mqQueueResultFiltered" :key="qu" class="m-0 p-0 border-bottom-0">

                                <div class="text-center" style="width: 100%; padding: 4px 10px 1px 10px; font-size: 13px; font-weight: bold; cursor: default; background-color: rgba(0, 0, 0, 0.03)">{{ qu }}</div>
                                <b-list-group class="m-0 p-0">
                                    <template v-for="v in mtsMqQueueData" v-if="v.mq_ip === qu">
                                        <b-list-group-item v-if="v.queue_depth > 20" class="m-0 p-0 border-bottom-0" style="background: red">
                                            <b-row align-v="center" class="m-1 text-white" style="line-height: 1; cursor: default;">
                                                <b-col lg="1">
                                                    <font-awesome-icon icon="circle"/>
                                                </b-col>
                                                <b-col lg="10">
                                                    <span class="font-weight-bold">{{ v.queue_name }}</span>
                                                </b-col>
                                                <b-col lg="1">
                                                    <span class="font-weight-bold" style="font-size: 13px">{{ v.queue_depth }}</span>
                                                </b-col>
                                            </b-row>
                                        </b-list-group-item>

                                        <b-list-group-item v-else class="m-0 p-0 border-bottom-0">
                                            <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default;">
                                                <b-col lg="1">
                                                    <font-awesome-icon icon="circle" style="color: limegreen"/>
                                                </b-col>
                                                <b-col lg="10">
                                                    <span class="font-weight-bold text-success" style="font-size: 12px">{{ v.queue_name }}</span>
                                                </b-col>
                                                <b-col lg="1">
                                                    <span style="font-size: 13px">{{ v.queue_depth }}</span>
                                                </b-col>
                                            </b-row>
                                        </b-list-group-item>

                                    </template>
                                </b-list-group>

                            </b-list-group-item>
                        </b-list-group>
                        <b-card-footer v-if="mtsMqQueueData.length !== 0" class="text-center p-0">
                            <div style="font-size:11px">
                                <abbr :title="$dt.fromISO(mtsMqQueueData[0].updated)">{{ $dt.fromISO(mtsMqQueueData[0].updated).toRelative() }}</abbr>
                            </div>
                        </b-card-footer>
                    </b-collapse>
                </b-list-group-item>
            </b-list-group>
        </b-card>
    </div>
</template>

<script>    
    export default {
        props: ["mtsMqTunnelData", "mtsMqChannelData", "mtsMqQueueData"],
        data() {
            return {
                mqTunnelCollapseShow: false,
                mqChannelCollapseShow: false,
                mqQueueCollapseShow: false
            };
        },
        methods: {
            mqCutIpResult(ip) {
                if(ip.includes('OK')) return ip.replace(' - OK', '');
                else if(ip.includes('FAIL')) return ip.replace(' - FAIL', '');
                else return '';
            }
        },
        computed: {            
            mqSshResult() {
                if(this.mtsMqTunnelData) {
                    let result = true;
                    this.mtsMqTunnelData.forEach(x => { 
                        if(x.ssh_result.includes('FAIL')) {
                            result = false;
                            this.mqTunnelCollapseShow = true;
                        }
                    });
                    return result;
                }
            },
            mqPingResult() {
                if(this.mtsMqTunnelData) {
                    let result = true;
                    this.mtsMqTunnelData.forEach(x => { 
                        if(x.ping_result.includes('FAIL')) {
                            result = false;
                            this.mqTunnelCollapseShow = true;
                        }
                    });
                    return result;
                }
            },
            mqChannelResultFiltered() {
                if(this.mtsMqChannelData) return [...new Set(this.mtsMqChannelData.map(item => item.mq_ip))];
            },
            mqChannelResult() {
                if(this.mtsMqChannelData) {
                    let result = true;
                    this.mtsMqChannelData.forEach(x => { 
                        if(x.channel_status !== 'Running') {
                            result = false;
                            this.mqChannelCollapseShow = true;
                        }
                    });
                    return result;
                }
            },
            mqQueueResult() {
                if(this.mtsMqQueueData) {
                    let result = true;
                    this.mtsMqQueueData.forEach(x => { 
                        if(x.queue_depth > 20) {
                            result = false;
                            this.mqQueueCollapseShow = true;
                        }
                    });
                    return result;
                }
            },
            mqQueueResultFiltered() {
                if(this.mtsMqQueueData) return [...new Set(this.mtsMqQueueData.map(item => item.mq_ip))];
            }
        }
    }
</script>

<style scoped>
    
</style>