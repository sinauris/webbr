<template>
    <b-card no-body :header-class="{'card-header-progress': true, 'cardAlarm': alarmComputed }" :class="{'btnHidden':true, 'borderAlarm': alarmComputed }">

        <div slot="header" class="text-center font-weight-bold">
            <div class="ml-2 btnHover float-left">
                <span class="mr-2" id="mtsPopover" style="cursor: help"><font-awesome-icon :icon="['far', 'question-circle']"/></span>
            </div>

            <b-popover target="mtsPopover" placement="left" triggers="hover">
                <template slot="title">
                    <div style="font-size: 12px"><font-awesome-icon :icon="['far', 'clock']"/> Время обновления: 10-15 секунд</div>
                </template>
                <p class="m-0" style="font-size: 12px">Виджет отображает текущие состояние проекта.</p>
            </b-popover>

            <div class="float-right" style="margin-right: 20px">
                <div class="btnHover" style="cursor: pointer" v-b-toggle="'mtsCollapse'">
                    <font-awesome-icon class="when-opened" icon="chevron-up"/>
                    <font-awesome-icon class="when-closed" icon="chevron-down"/>
                </div>
            </div>

            <div style="font-size:14px">
                <span @click="alarmEnable = !alarmEnable">
                    <span v-if="alarmEnable" style="cursor: pointer;" ><font-awesome-icon icon="university"/> МТС Банк</span>
                    <span v-else style="cursor: pointer; color: darkgrey"><font-awesome-icon icon="university"/> МТС Банк</span>
                </span>
            </div>
        </div>

        <b-collapse id="mtsCollapse" visible>
            <div class="m-2 text-center" style="cursor: default">
                <b-card no-body class="mb-3">
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

                                            <div class="text-center" style="width: 100%; padding: 1px 10px; font-size: 13px; font-weight: bold; border-bottom: 1px solid #dee2e6; cursor: default; background-color: rgba(0, 0, 0, 0.03)">{{ v.ip }}</div>
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
                                                        <b-row align-v="center" class="m-1 text-success" style="line-height: 1; cursor: default">
                                                            <b-col lg="1">
                                                                <font-awesome-icon icon="circle" style="color: limegreen"/>
                                                            </b-col>
                                                            <b-col lg="7">
                                                                <span class="font-weight-bold" style="font-size: 13px">Доступен</span>
                                                            </b-col>
                                                            <b-col lg="4">
                                                                <span style="font-size: 13px; color: #000">{{ mqCutIpResult(res) }}</span>
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
                                        <div class="text-center" style="width: 100%; padding: 1px 10px; font-size: 13px; font-weight: bold; cursor: default; background-color: rgba(0, 0, 0, 0.03)">{{ ch }}</div>
                                            <hr class="m-0 p-0"/>
                                            <b-list-group flush>
                                                <template v-for="v in mtsMqChannelData"  v-if="v.mq_ip === ch">
                                                    <b-list-group-item v-if="v.channel_status ==='Running'" class="m-0 p-0 border-bottom-0" style="cursor: default">
                                                        <b-row align-v="center" class="m-1 text-success font-weight-bold" style="line-height: 1">
                                                            <b-col lg="1">
                                                                <font-awesome-icon icon="circle" style="color: limegreen"/>
                                                            </b-col>
                                                            <b-col lg="10">
                                                                <span style="font-size: 12px">{{ v.channel_name }}</span>
                                                            </b-col>
                                                            <b-col lg="1">
                                                                <span style="cursor: pointer; color: #000" v-b-toggle="'mtsChannel-' + v.mq_ip + v.channel_name + v.queue_manager_name + v.channel_connection_name">
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

                                        <div class="text-center" style="width: 100%; padding: 1px 10px; font-size: 13px; font-weight: bold; border-bottom: 1px solid #dee2e6; cursor: default; background-color: rgba(0, 0, 0, 0.03)">{{ qu }}</div>
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

                <b-card no-body class="mb-3">
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
<!--                                    <div class="text-muted" style="font-size: 10px;">Дата запуска: {{ v.agent_start_time }}</div>-->
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

                <b-card no-body>
                    <span slot="header" class="font-weight-bold">
                        <b-link href="http://37.221.187.183/Техническая_дирекция/Проекты/МТС_Банк/МТС_Банк._Скрипт_импорта._Проверка_работоспособности_и_перезапуск" target="_blank">Импорт</b-link>
                    </span>
                    <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default">
                        <b-col lg="1">
                            <div>
                                <font-awesome-icon v-if="!importSshResult || !importScriptResult" icon="circle" style="color: red"/>
                                <font-awesome-icon v-else icon="circle" style="color: limegreen"/>
                            </div>
                        </b-col>
                        <b-col lg="10">
                            <div v-if="!importSshResult || !importScriptResult" class="text-danger font-weight-bold">Скрипты импорта</div>
                            <div v-else>Скрипты импорта</div>
                        </b-col>
                        <b-col lg="1">
                            <div style="cursor: pointer" @click="mqImportCollapseShow = !mqImportCollapseShow">
                                <font-awesome-icon v-if="mqImportCollapseShow" icon="chevron-up"/>
                                <font-awesome-icon v-else icon="chevron-down"/>
                            </div>
                        </b-col>
                    </b-row>

                    <b-collapse v-model="mqImportCollapseShow" id="importCollapseShow">
                        <hr class="m-0 p-0"/>
                        <b-list-group flush>

                            <template v-for="v in mtsImportData" v-if="v.ssh_result === 'OK'">
                                <b-list-group-item v-if="v.import_result === 'RUNNING'" class="m-0 p-0 border-top-0">
                                    <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default">
                                        <b-col lg="1">
                                            <font-awesome-icon icon="circle" style="color: limegreen"/>
                                        </b-col>
                                        <b-col lg="7">
                                            <span class="text-success font-weight-bold" style="font-size: 13px">Запущен</span>
                                        </b-col>
                                        <b-col lg="4">
                                            <div style="font-size: 13px">{{ v.description }}</div>
                                            <div class="text-muted" style="font-size: 11px">{{ v.ip }}</div>
                                        </b-col>
                                    </b-row>
                                </b-list-group-item>

                                <b-list-group-item v-else class="m-0 p-0 border-top-0" style="background: red">
                                    <b-row align-v="center" class="m-1 text-white font-weight-bold" style="line-height: 1; cursor: default">
                                        <b-col lg="1">
                                            <font-awesome-icon icon="circle"/>
                                        </b-col>
                                        <b-col lg="7">
                                            <span>Остановлен</span>
                                        </b-col>
                                        <b-col lg="4">
                                            <div style="font-size: 13px">{{ v.description }}</div>
                                            <div style="font-size: 11px">{{ v.ip }}</div>
                                        </b-col>
                                    </b-row>
                                </b-list-group-item>
                            </template>

                            <template v-else>
                                <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default;">
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
                                <b-alert v-if="v.ssh_exception" show variant="danger" class="mt-3 mb-0 p-0">
                                    <h6 class="mt-1 alert-heading">Ошибка</h6>
                                    <p class="m-1 p-0" style="font-size: 12px;">{{ v.ssh_exception }}</p>
                                </b-alert>
                            </template>

                        </b-list-group>
                        <b-card-footer v-if="mtsImportData.length !== 0" class="text-center p-0 border-top-0">
                            <div style="font-size:11px">
                                <abbr :title="$dt.fromISO(mtsImportData[0].updated)">{{ $dt.fromISO(mtsImportData[0].updated).toRelative() }}</abbr>
                            </div>
                        </b-card-footer>
                    </b-collapse>
                </b-card>
            </div>
        </b-collapse>
    </b-card>
</template>

<script>
    export default {
        props: ["mtsMqTunnelData", "mtsMqChannelData", "mtsMqQueueData", "mtsImportData", "mtsMgwAgentData", "mtsMgwJobData"],
        data() {
            return {
                mqTunnelCollapseShow: false,
                mqChannelCollapseShow: false,
                mqQueueCollapseShow: false,
                mqImportCollapseShow: false,

                alarmDetected: false,
                alarmEnable: true
            };
        },
        methods: {
            mqCutIpResult(ip) {
                if(ip.includes('OK')) return ip.replace(' - OK', '');
                else if(ip.includes('FAIL')) return ip.replace(' - FAIL', '');
                else return '';
            },

            mgwJobsResult(agent) {
                if(this.mtsMgwJobData) {
                    let result = true;
                    // let arrAgents = this.mtsMgwJobData.filter((item) => { return item.naumenlogin !== '';});
                    let arrAgent = this.mtsMgwJobData.filter((j) => {return j.agent_name === agent});
                    arrAgent.forEach(x => { if(x.enabled === 'TRUE' && x.status !== 'READY') {
                        result = false;
                    }});
                    return result;
                }
            },
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
            // mgwJobsResult() {
            //     if(this.mtsMgwJobData) {
            //         let result = true;
            //         this.mtsMgwJobData.forEach(x => { if(x.enabled === 'TRUE' && x.status !== 'READY') {
            //             result = false;
            //         }});
            //         return result;
            //     }
            // },

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
        },
    }
</script>

<style scoped>
    .card-header {
        padding: 1px;
    }

    .card {
        line-height: 1.5;
    }

    .btnHover {
        display: none;
    }
    .btnHidden:hover >>> .btnHover {
        display: block;
        position: absolute;
    }

    .showSecond{
        display: none;
    }
    .showed:hover .showSecond {
        display: block;
    }
    .showed:hover .showFirst {
        display : none;
    }
</style>
