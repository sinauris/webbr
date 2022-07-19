<template>
    <div class="m-2 text-center" style="cursor: default">
        <b-card no-body>
            <span slot="header" class="font-weight-bold">
                <b-link href="http://mtsbank.newcontact.su/mgw_admin/" target="_blank">MGW</b-link>
            </span>

            <b-list-group flush class="m-0 p-0">

                <b-list-group-item v-for="v in mtsMgwAgentData" :key="v.agent_name" class="m- p-0">
                    <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default;">

                        <b-col lg="1">
                            <font-awesome-icon v-if="v.agent_status === 'RUNNING' && mgwJobsResult(v.agent_name)" icon="circle" style="color: limegreen"/>
                            <font-awesome-icon v-else-if="v.agent_status === 'RUNNING' && !mgwJobsResult(v.agent_name)" icon="circle" class="text-warning"/>
                            <font-awesome-icon v-else icon="circle" style="color: red"/>
                        </b-col>

                        <b-col lg="10">
                            <div v-if="v.agent_status === 'RUNNING' && mgwJobsResult(v.agent_name)">{{ v.agent_name }}</div>
                            <div v-else-if="v.agent_status === 'RUNNING' && !mgwJobsResult(v.agent_name)" class="text-warning font-weight-bold">{{ v.agent_name }}</div>
                            <div v-else class="text-danger font-weight-bold">{{ v.agent_name }}</div>
                            <div class="text-muted" style="font-size: 10px;">Дата запуска: {{ v.agent_start_time }}</div>
                        </b-col>

                        <b-col lg="1">
                            <div style="cursor: pointer" v-b-toggle="'mtsMgwAgents' + v.agent_name">
                                <font-awesome-icon class="when-opened" icon="chevron-up"/>
                                <font-awesome-icon class="when-closed" icon="chevron-down"/>
                            </div>
                        </b-col>
                    </b-row>

                    <b-collapse :id="'mtsMgwAgents' + v.agent_name">
                        <hr class="m-0 p-0"/>
                        <b-list-group flush>
                            <b-list-group-item class="m-0 p-0 border-bottom-0">
                                <div class="m-1">
                                    <p class="m-0 p-0 font-weight-bold">Состояние агента</p> {{ v.agent_status }}
                                </div>
                            </b-list-group-item>
                            <b-list-group-item class="m-0 p-0 border-bottom-0">
                                <div class="m-1">
                                    <p class="m-0 p-0 font-weight-bold">Пинг агента</p> {{ v.agent_ping }}
                                </div>
                            </b-list-group-item>
                            <b-list-group-item class="m-0 p-0 border-bottom-0">
                                <div class="m-1">
                                    <p class="m-0 p-0 font-weight-bold">Юзер агента</p> {{ v.agent_user }}
                                </div>
                            </b-list-group-item>
                            <b-list-group-item class="m-0 p-0 border-bottom-0">
                                <div class="m-1">
                                    <p class="m-0 p-0 font-weight-bold">БД агента</p> {{ v.agent_database }}
                                </div>
                            </b-list-group-item>
                            <b-list-group-item class="m-0 p-0 border-bottom-0">
                                <div class="m-1">
                                    <p class="m-0 p-0 font-weight-bold">Макс. кол-во соединений</p> {{ v.max_connections }}
                                </div>
                            </b-list-group-item>
                            <b-list-group-item class="m-0 p-0 border-bottom-0">
                                <div class="m-1">
                                    <p class="m-0 p-0 font-weight-bold">Макс. кол-во памяти</p> {{ v.max_memory }}
                                </div>
                            </b-list-group-item>
                            <b-list-group-item class="m-0 p-0 border-bottom-0">
                                <div class="m-1">
                                    <p class="m-0 p-0 font-weight-bold">Макс. кол-во потоков</p> {{ v.max_threads }}
                                </div>
                            </b-list-group-item>

                            <div class="text-center" style="width: 100%; padding: 1px 10px; font-size: 13px; font-weight: bold; border-top: 1px solid rgba(0, 0, 0, 0.125); cursor: default; background-color: rgba(0, 0, 0, 0.03)">Задания агента</div>

                            <template v-for="vv in mtsMgwJobData" v-if="v.agent_name === vv.agent_name">

                                <b-list-group-item v-if="vv.status === 'READY' && vv.enabled === 'TRUE'" class="m-0 p-0 border-bottom-0" style="cursor: default">
                                    <b-row align-v="center" class="m-1" style="line-height: 1">
                                        <b-col lg="1">
                                            <font-awesome-icon icon="circle" style="color: limegreen"/>
                                        </b-col>
                                        <b-col lg="10">
                                            <span class="mb-1 p-0 text-success font-weight-bold" style="font-size: 12px;">{{ vv.job_name }}</span>
                                        </b-col>
                                        <b-col lg="1">
                                            <span style="cursor: pointer" v-b-toggle="'mtsMgwJob' + vv.job_name">
                                                <font-awesome-icon class="when-opened" icon="chevron-up"/>
                                                <font-awesome-icon class="when-closed" icon="chevron-down"/>
                                            </span>
                                        </b-col>
                                    </b-row>
                                </b-list-group-item>
                                <b-list-group-item v-else-if="vv.enabled !== 'TRUE'" class="m-0 p-0 border-bottom-0" style="cursor: default">
                                    <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default">
                                        <b-col lg="1">
                                            <font-awesome-icon icon="circle" style="color: #95a5a6"/>
                                        </b-col>
                                        <b-col lg="10">
                                            <span class="mb-1 p-0 font-weight-bold" style="color: #95a5a6">{{ vv.job_name }}</span>
                                        </b-col>
                                        <b-col lg="1">
                                        <span style="cursor: pointer" v-b-toggle="'mtsMgwJob' + vv.job_name">
                                            <font-awesome-icon class="when-opened" icon="chevron-up"/>
                                            <font-awesome-icon class="when-closed" icon="chevron-down"/>
                                        </span>
                                        </b-col>
                                    </b-row>
                                </b-list-group-item>
                                <b-list-group-item v-else class="m-0 p-0 border-bottom-0" style="cursor: default; background-color: red">
                                    <b-row align-v="center" class="m-1 text-white font-weight-bold" style="line-height: 1">
                                        <b-col lg="1">
                                            <font-awesome-icon icon="circle"/>
                                        </b-col>
                                        <b-col lg="10">
                                            <span class="mb-1 p-0" style="font-size: 13px">{{ vv.job_name }}</span>
                                        </b-col>
                                        <b-col lg="1">
                                            <span style="cursor: pointer" v-b-toggle="'mtsMgwJob' + vv.job_name">
                                                <font-awesome-icon class="when-opened" icon="chevron-up"/>
                                                <font-awesome-icon class="when-closed" icon="chevron-down"/>
                                            </span>
                                        </b-col>
                                    </b-row>
                                </b-list-group-item>
                                <b-collapse :id="'mtsMgwJob' + vv.job_name">
                                    <b-list-group class="m-0 p-0" style="font-size: 12px">
                                        <b-list-group-item class="m-0 p-0 border-bottom-0">
                                            <div class="m-1">
                                                <p class="m-0 p-0 font-weight-bold">Работоспособность</p> {{ vv.enabled }}
                                            </div>
                                        </b-list-group-item>

                                        <b-list-group-item class="m-0 p-0 border-bottom-0">
                                            <div class="m-1">
                                                <p class="m-0 p-0 font-weight-bold">Состояние канала</p> {{ vv.status }}
                                            </div>
                                        </b-list-group-item>

                                        <b-list-group-item class="m-0 p-0 border-bottom-0">
                                            <div class="m-1">
                                                <p class="m-0 p-0 font-weight-bold">Имя задания</p> {{ vv.job_name }}
                                            </div>
                                        </b-list-group-item>
                                        <b-list-group-item class="m-0 p-0 border-bottom-0">
                                            <div class="m-1">
                                                <p class="m-0 p-0 font-weight-bold">Источник</p> {{ vv.source }}
                                            </div>
                                        </b-list-group-item>
                                        <b-list-group-item class="m-0 p-0 border-bottom-0">
                                            <div class="m-1">
                                                <p class="m-0 p-0 font-weight-bold">Принято сообщений</p> {{ vv.propagated_msgs }}
                                            </div>
                                        </b-list-group-item>
                                        <b-list-group-item class="m-0 p-0 border-bottom-0">
                                            <div class="m-1">
                                                <p class="m-0 p-0 font-weight-bold">Направление</p>
                                                <template v-if="vv.propagation_type === 'INBOUND'">Входящий</template>
                                                <template v-else-if="vv.propagation_type === 'OUTBOUND'">Исходящий</template>
                                            </div>
                                        </b-list-group-item>
                                        <b-list-group-item class="m-0 p-0 border-bottom-0">
                                            <div class="m-1">
                                                <p class="m-0 p-0 font-weight-bold">Имя очереди и канала MQ</p> {{ vv.destination }}
                                            </div>
                                        </b-list-group-item>

                                        <b-list-group-item class="m-0 p-0 border-bottom-0" v-if="vv.last_error_msg">
                                            <div class="m-1">
                                                <p class="m-0 p-0 font-weight-bold ">Последняя ошибка</p>
                                                <div class="text-danger mt-1 mb-1">
                                                    {{ vv.last_error_msg }}
                                                </div>
                                                <p class="m-0 p-0 font-weight-bold">{{ vv.last_error_date }}</p>
                                            </div>
                                        </b-list-group-item>
                                    </b-list-group>
                                </b-collapse>
                            </template>

                        </b-list-group>
                        <b-card-footer class="text-center p-0">
                            <div style="font-size:11px">
                                <abbr :title="$dt.fromISO(v.updated)">{{ $dt.fromISO(v.updated).toRelative() }}</abbr>
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
        props: ["mtsMgwAgentData", "mtsMgwJobData"],
        methods: {
            mgwJobsResult(agent) {
                if(this.mtsMgwJobData) {
                    let result = true;
                    let arrAgent = this.mtsMgwJobData.filter((j) => {return j.agent_name === agent});
                    arrAgent.forEach(x => { if(x.enabled !== 'TRUE' || x.status !== 'READY') {
                        result = false;
                    }});
                    return result;
                }
            }
        },
        computed: {
            mgwAgentsResult() {
                if(this.mtsMgwAgentData) {
                    let result = true;
                    this.mtsMgwAgentData.forEach(x => { if(x.agent_status !== 'RUNNING') {
                        result = false;
                    }});
                    return result;
                }
            }
        }
    }
</script>

<style scoped>

</style>