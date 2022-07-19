<!--<template>-->
    <!--<b-card no-body :header-class="{'card-header-progress': true, 'cardAlarm': alarmDetected }" :class="{'btnHidden':true, 'borderAlarm': alarmDetected }">-->
        <!--<div slot="header" class="text-center font-weight-bold">-->
            <!--<div class="ml-2 btnHover float-left">-->
                <!--<span class="mr-2" id="zabbixPopover" style="cursor: help"><font-awesome-icon :icon="['far', 'question-circle']"/></span>-->
            <!--</div>-->

            <!--<b-popover target="zabbixPopover" placement="left" triggers="hover">-->
                <!--<template slot="title">-->
                    <!--<span style="font-size: 14px">Zabbix</span>-->
                    <!--<p class="p-0 m-0 text-muted" style="font-size: 12px">Описание виджета</p>-->

                    <!--<hr class="mt-3 mb-2 p-0"/>-->
                    <!--<div style="font-size: 12px"><font-awesome-icon :icon="['far', 'clock']"/> Время обновления: 10 секунд</div>-->
                <!--</template>-->
                <!--<p class="m-0" style="font-size: 12px">Виджет отображает текущие алерты Zabbix.</p>-->
            <!--</b-popover>-->

            <!--<div class="float-right" style="margin-right: 20px">-->
                <!--<div class="btnHover" style="cursor: pointer" v-b-toggle="'zabbixCollapse'">-->
                    <!--<font-awesome-icon class="when-opened" icon="chevron-up"/>-->
                    <!--<font-awesome-icon class="when-closed" icon="chevron-down"/>-->
                <!--</div>-->
            <!--</div>-->

            <!--<div>-->
                <!--<span style="cursor: default;"><font-awesome-icon icon="bell"/> Zabbix • {{ items.length }}</span>-->
            <!--</div>-->
        <!--</div>-->

        <!--<b-collapse id="zabbixCollapse" visible>-->
            <!--<div v-if="exception" class="text-center">-->
                <!--<p class="m-0 font-weight-bold" style="font-size: 40px"><kbd style="background-color:#fff;color:#000">{{ this.exception.status }}</kbd></p>-->
                <!--<h6 class="text-muted mt-2">Возникла ошибка</h6>-->
                <!--<h5>{{ this.exception.data }}</h5>-->
            <!--</div>-->

            <!--<template v-else>-->
                <!--<b-card-->
                    <!--v-for="card in items.slice(0,10)"-->
                    <!--:key="card.id"-->
                    <!--no-body-->
                    <!--class="m-2 text-center"-->
                    <!--style="cursor: default"-->
                    <!--:header-bg-variant="card.priority == 1 ? 'zabbix-easy' : card.priority == 2 ? 'zabbix-warning' : card.priority == 3 ? 'zabbix-average' : card.priority == 4 ? 'zabbix-high' : card.priority == 5 ? 'zabbix-disaster' : ''"-->
                    <!--header-text-variant="dark"-->
                    <!--header-class="zabbix-card-header"-->
                <!--&gt;-->

                    <!--<div slot="header" style="padding: 1px; line-height: 1; font-size: 11px; font-weight: 700">{{ card.hostname.toUpperCase() }}</div>-->
                    <!--<b-card-body style="padding: 4px; line-height: 1; font-size: 11px">{{ card.description }}</b-card-body>-->
                    <!--<b-card-footer style="min-height: 16px; padding: 2px; line-height: 1; font-size: 10px">-->
                        <!--<abbr :title="$dt.fromISO(card.lastchange)">{{ $dt.fromISO(card.lastchange).toRelative() }}</abbr>-->
                    <!--</b-card-footer>-->

                <!--</b-card>-->
                <!---->
                <!--<b-btn v-if="!zabbixExpand && items.length > 10" block v-b-toggle.expandZabbix variant="light" size="sm" class="m-0 p-0" style="font-size: 0.7rem; background-color: #f8f9fa00; border-color: #f8f9fa00;">-->
                    <!--<font-awesome-icon icon="chevron-down"/> Развернуть-->
                <!--</b-btn>-->
                <!--<b-collapse id="expandZabbix" v-model="zabbixExpand" style="width: 100%">-->
                <!--<b-card-->
                    <!--v-for="card in items.slice(10)"-->
                    <!--:key="card.id"-->
                    <!--no-body-->
                    <!--class="m-2 text-center"-->
                    <!--style="cursor: default"-->
                    <!--:header-bg-variant="card.priority == 1 ? 'zabbix-easy' : card.priority == 2 ? 'zabbix-warning' : card.priority == 3 ? 'zabbix-average' : card.priority == 4 ? 'zabbix-high' : card.priority == 5 ? 'zabbix-disaster' : ''"-->
                    <!--header-text-variant="dark"-->
                    <!--header-class="zabbix-card-header"-->
                <!--&gt;-->
                    <!--<div slot="header" style="padding: 1px; line-height: 1; font-size: 11px; font-weight: 700">{{ card.hostname.toUpperCase() }}</div>-->
                    <!--<b-card-body style="padding: 4px; line-height: 1; font-size: 11px">{{ card.description }}</b-card-body>-->
                    <!--<b-card-footer style="min-height: 16px; padding: 2px; line-height: 1; font-size: 10px">-->
                        <!--<abbr :title="$dt.fromISO(card.lastchange)">{{ $dt.fromISO(card.lastchange).toRelative() }}</abbr>-->
                    <!--</b-card-footer>-->
                <!--</b-card>-->
                <!--<b-btn block v-b-toggle.expandZabbix variant="light" size="sm" class="mt-0 p-0" style="font-size: 0.7rem; background-color: #f8f9fa00; border-color: #f8f9fa00;">-->
                    <!--<font-awesome-icon icon="chevron-up"/>-->
                    <!--Свернуть-->
                <!--</b-btn>-->
            <!--</b-collapse>-->
            <!--</template>-->
        <!--</b-collapse>-->
    <!--</b-card>-->
<!--</template>-->

<!--<script>-->
    <!--export default {-->
        <!--props: ["exception", "items"],-->
        <!--data() {-->
            <!--return {-->
                <!--zabbixExpand: false-->
            <!--};-->
        <!--},-->
        <!--computed: {-->
            <!--alarmDetected() {-->
                <!--if(this.items) {-->
                    <!--let result = false;-->
                    <!--this.items.forEach(x => { if(x.priority >= 4) result = true });-->
                    <!--return result;-->
                <!--}-->
            <!--}-->
        <!--}-->
    <!--};-->
<!--</script>-->

<!--<style scoped>-->
    <!--.zabbix-card-header { padding: 1px 0 1px 0 !important }-->

    <!--.bg-zabbix-easy { background-color: #e8fff6; }-->
    <!--.bg-zabbix-warning { background-color: #fff59d; }-->
    <!--.bg-zabbix-average { background-color: #ff9d00; }-->
    <!--.bg-zabbix-high { background-color: #ff0000; color: #ffffff !important; }-->
    <!--.bg-zabbix-disaster { background-color: #ff0000; color: #ffffff !important; }-->

    <!--.btnHover {-->
        <!--display: none;-->
    <!--}-->
    <!--.btnHidden:hover >>> .btnHover {-->
        <!--display: block;-->
        <!--position: absolute;-->
    <!--}-->
<!--</style>-->


<template>
    <b-card no-body :header-class="{'card-header-progress': true, 'cardAlarm': alarmDetected }" :class="{'btnHidden':true, 'borderAlarm': alarmDetected }">
        <div slot="header" class="text-center font-weight-bold">
            <div>
                <span @click="alarmEnable = !alarmEnable">
                    <span v-if="alarmEnable" style="cursor: pointer;" ><font-awesome-icon icon="bell"/> Zabbix • {{ items.length }}</span>
                    <span v-else style="cursor: pointer; color: darkgrey"><font-awesome-icon icon="bell"/> Zabbix • {{ items.length }}</span>
                </span>
            </div>
        </div>

        
        <div v-if="exception" class="text-center">
            <p class="m-0 font-weight-bold" style="font-size: 40px"><kbd style="background-color:#fff;color:#000">{{ this.exception.status }}</kbd></p>
            <h6 class="text-muted mt-2">Возникла ошибка</h6>
            <h5>{{ this.exception.data }}</h5>
        </div>

        <template v-else>
            <b-row>
                <b-col v-for="(card, index) in items" :key="card.id" cols="4" sm="3" md="2" lg="2" xl="4">
                    <b-card
                        no-body
                        class="text-center m-1"
                        style="cursor: default;"
                        :header-bg-variant="card.priority == 1 ? 'zabbix-easy' : card.priority == 2 ? 'zabbix-warning' : card.priority == 3 ? 'zabbix-average' : card.priority == 4 ? 'zabbix-high' : card.priority == 5 ? 'zabbix-disaster' : ''"
                        :border-variant="card.priority == 1 ? 'zabbix-easy' : card.priority == 2 ? 'zabbix-warning' : card.priority == 3 ? 'zabbix-average' : card.priority == 4 ? 'zabbix-high' : card.priority == 5 ? 'zabbix-disaster' : ''"
                        header-text-variant="dark"
                        header-class="zabbix-card-header"
                    >
                        <div slot="header" style="cursor:pointer; padding: 1px; line-height: 1; font-size: 10px; font-weight: 700; min-height: 22px;" v-b-toggle="'zab-' + index">
                            <template v-if="!card.hostname.includes('(')">{{ card.hostname.toUpperCase() }}</template>
                            <template v-else-if="card.hostname.includes('(')">
                                <div>{{ card.hostname.split('(')[0].replace(' ', '') }}</div>
                                <div>{{ card.hostname.split('(')[1].replace(')', '') }}</div>
                            </template>
                            <template v-else>{{ card.hostname.toUpperCase() }}</template>
                        </div>
                        
                        <b-collapse v-if="card.priority < 4" :id="'zab-' + index">
                            <b-card-body style="padding: 4px; line-height: 1; font-size: 11px">{{ card.description }}</b-card-body>
                        </b-collapse>
                        <b-collapse v-else visible :id="'zab-' + index">
                            <b-card-body style="padding: 4px; line-height: 1; font-size: 11px">{{ card.description }}</b-card-body>
                        </b-collapse>
                        
                        <b-card-footer footer-text-variant="dark" :footer-bg-variant="card.priority == 1 ? 'zabbix-easy' : card.priority == 2 ? 'zabbix-warning' : card.priority == 3 ? 'zabbix-average' : card.priority == 4 ? 'zabbix-high' : card.priority == 5 ? 'zabbix-disaster' : ''" style="min-height: 16px; padding: 2px; line-height: 1; font-size: 10px">
                            {{ $dt.fromISO(card.lastchange).toRelative() }}
                        </b-card-footer>
                    </b-card>
                </b-col>
            </b-row>
        </template>
    </b-card>
</template>

<script>
    export default {
        props: ["exception", "items"],
        data() {
            return {
                zabbixExpand: false,
                alarmEnable: true
            }
        },
        computed: {
            alarmDetected() {
                if(!this.alarmEnable) return false;
                if(this.items) {
                    let result = false;
                    this.items.forEach(x => { if(x.priority >= 4) result = true });
                    return result;
                }
            }
        }
    }
</script>

<style scoped>
    .zabbix-card-header { padding: 1px 0 1px 0 !important; }

    .bg-zabbix-easy { background-color: #e8fff6 }
    .bg-zabbix-warning { background-color: #fff59d }
    .bg-zabbix-average { background-color: #ff9d00 }
    .bg-zabbix-high { background-color: #ff0000; color: #ffffff !important }
    .bg-zabbix-disaster { background-color: #ff0000; color: #ffffff !important }

    .border-zabbix-easy { border-color: #e8fff6 }
    .border-zabbix-warning { border-color: #ffe763 }
    .border-zabbix-average { border-color: #ff9d00 }
    .border-zabbix-high { border-color: #ff0000; background-color: #ff0000; color: #ffffff !important }
    .border-zabbix-disaster { border-color: #ff0000; background-color: #ff0000; color: #ffffff !important }
</style>