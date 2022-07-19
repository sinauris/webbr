<template>
    <b-card no-body :header-class="{'card-header-progress': true, 'cardAlarm': alarmDetected }" :class="{'btnHidden':true, 'borderAlarm': alarmDetected }">
        <div slot="header" class="text-center font-weight-bold">
            <div style="font-size:14px">
                <span @click="alarmEnable = !alarmEnable">
                    <span v-if="alarmEnable" style="cursor: pointer;" ><font-awesome-icon icon="cloud"/> Grafana</span>
                    <span v-else style="cursor: pointer; color: darkgrey"><font-awesome-icon icon="cloud"/> Grafana</span>
                </span>
            </div>
        </div>
        <div v-if="exception" class="text-center">
            <p class="m-0 font-weight-bold" style="font-size: 40px"><kbd style="background-color:#fff;color:#000">{{ this.exception.status }}</kbd></p>
            <h6 class="text-muted mt-2">Возникла ошибка</h6>
            <h5>{{ this.exception.data }}</h5>
        </div>
        <template v-else>
            <template v-for="s in sections">
                <h6 class="m-2 p-0 text-center" style="cursor: default">{{ s.section_name }}</h6>
                <b-row>
                    <b-col v-for="card in servers" v-if="s.id === card.section" :key="card.id" cols="4" sm="3" md="3" lg="2" xl="4">
                        <b-card
                                no-body
                                :border-variant="!card.load ? 'danger' : Math.floor(card.load) >= card.load_treshold ? 'danger' : 'success'"
                                :bg-variant="!card.load ? 'danger' : Math.floor(card.load) >= card.load_treshold ? 'danger' : ''"
                                header-text-variant="white"
                                class="showed m-1"
                        >
                            <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default;">
                                <b-col cols="2" sm="2" md="2" lg="2" xl="2">
                                    <div class="text-center">
                                        <span v-if="!card.load" v-b-tooltip.hover title="Не поступает информация">
                                            <font-awesome-icon :icon="['far', 'times-circle']" size="lg" style="color: #fff; cursor: help;"/>
                                        </span>
                                        
                                        <font-awesome-icon v-else-if="Math.floor(card.load) >= card.load_treshold" icon="circle" style="color: #fff"/>
                                        <font-awesome-icon v-else icon="circle" style="color: limegreen"/>
                                    </div>
                                </b-col>
                                <b-col cols="6" sm="6" md="6" lg="6" xl="6">
                                    <div v-if="!card.load || Math.floor(card.load) >= card.load_treshold">
                                        <div style="font-size: 12px; color: #fff;">{{ card.name }}</div>
                                        <div style="font-size: 10px; color: #fff;">{{ card.ip }}</div>
                                    </div>
                                    <div v-else>
                                        <div style="font-size: 12px;">{{ card.name }}</div>
                                        <div style="font-size: 10px;">{{ card.ip }}</div>
                                    </div>
                                </b-col>
                                <b-col cols="4" sm="4" md="4" lg="4" xl="4">
                                    <div class="showFirst">
                                        <div v-if="!card.load" class="text-center font-weight-bold" style="font-size: 20px; color: #fff">##</div>
                                        <div v-else-if="Math.floor(card.load) >= card.load_treshold" style="font-size: 20px; color: #fff;" class="text-center font-weight-bold">{{ card.load }}</div>
                                        <div v-else style="font-size: 20px;" class="text-center font-weight-bold">{{ card.load }}</div>
                                    </div>
                                    <div class="showSecond">
                                        <b-btn variant="warning" size="sm" style="font-size: 0.5rem; width: 100%; padding: 6px; line-height: 0" @click="sshsCommand(card.ip)" v-b-tooltip="{title: 'Подключиться по SSH', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"><font-awesome-icon icon="terminal"/></b-btn>
                                    </div>
                                </b-col>
                            </b-row>
                        </b-card>
                    </b-col>
                </b-row>
            </template>
        </template>
    </b-card>
</template>

<script>
    export default {
        props: ["exception", "sections", "servers"],
        data() {
            return {
                collapseShow: true,
                search: '',
                alarmEnable: true
            }
        },
        methods: {
            sshsCommand(ip) {
                window.location.href = "ssh://" + ip;
            }
        },
        computed: {
            filteredSections() {
                return [...new Set(this.servers.map(item => item.section))];
            },
            alarmDetected() {
                if(!this.alarmEnable) return false;
                if(this.servers) {
                    let result = false;
                    this.servers.forEach(x => { if(!x.load || Math.floor(x.load) >= x.load_treshold) result = true });
                    return result;
                }
            }
        }
    }
</script>

<style scoped>
    .showSecond { display: none }
    .showed:hover .showSecond { display: block }
    .showed:hover .showFirst { display : none }
</style>