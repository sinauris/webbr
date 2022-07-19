<template>
    <b-card no-body :header-class="{'card-header-progress': true, 'cardAlarm': alarmDetected }" :class="{'btnHidden':true, 'borderAlarm': alarmDetected }">
        <div slot="header" class="text-center font-weight-bold">
            <div>
                <span @click="alarmEnable = !alarmEnable">
                    <span v-if="alarmEnable" style="cursor: pointer;" ><font-awesome-icon icon="car-battery"/> ИБП</span>
                    <span v-else style="cursor: pointer; color: darkgrey"><font-awesome-icon icon="car-battery"/> ИБП</span>
                </span>
            </div>
        </div>

        <div v-if="exception" class="text-center">
            <p class="m-0 font-weight-bold" style="font-size: 40px"><kbd style="background-color:#fff;color:#000">{{ this.exception.status }}</kbd></p>
            <h6 class="text-muted mt-2">Возникла ошибка</h6>
            <h5>{{ this.exception.data }}</h5>
        </div>

        <template v-else>
            <b-card-body class="text-center p-0 m-0">
                <div flush>
                    <b-table class="mb-0" hover  :fields="fields" style="cursor: default;">
                        <template slot="HEAD_name" slot-scope="row">
                            <div v-b-tooltip="{title: 'Ссылка и описание', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">#</div>
                        </template>
                        <template slot="HEAD_input_vol" slot-scope="row">
                            <div v-b-tooltip="{title: 'Входное напряжение, V', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">Вх</div>
                        </template>
                        <template slot="HEAD_output_vol" slot-scope="row">
                            <div v-b-tooltip="{title: 'Выходное напряжение, V', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">Вых</div>
                        </template>
                        <template slot="HEAD_ups_load" slot-scope="row">
                            <div v-b-tooltip="{title: 'Нагрузка, %', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">Нагр</div>
                        </template>
                        <template slot="HEAD_battery_temp" slot-scope="row">
                            <div v-b-tooltip="{title: 'Температура батареи, °C', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">Темп</div>
                        </template>
                        <template slot="HEAD_battery_capacity" slot-scope="row">
                            <div v-b-tooltip="{title: 'Ёмкость батареи, %', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">Ёмк</div>
                        </template>
                    </b-table>

                    <template v-for="p in getPlace">
                        <div v-if="upsCaclulate(p.placeid).length !== 0" style="width: 100%; padding: 4px 10px 4px 10px; font-size: 13px; font-weight: bold; cursor: default; background-color: rgba(0, 0, 0, 0.03)">{{p.place_description}}</div>

                        <b-table thead-class="hidden_header" hover fixed bordered :fields="fields" :items=upsCaclulate(p.placeid) class="mb-0" style="cursor: default">
                            <template slot="name" slot-scope="row">
                                <span v-b-tooltip="{title: row.item.tooltip, trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"><a :href="'http://'+row.item.ip" target="_blank"><font-awesome-icon icon="globe-europe"/></a></span>
                            </template>

                            <template slot="ups_load" slot-scope="row">
                                <span v-if="row.value > 90" style="color: red">{{row.value}}</span>
                                <span v-else>{{row.value}}</span>
                            </template>

                            <template slot="battery_temp" slot-scope="row">
                                <span v-if="row.value > 40" style="color: red">{{row.value}}</span>
                                <span v-else>{{row.value}}</span>
                            </template>

                            <template slot="row-details" slot-scope="row">
                                <b-row class="m-0 p-0">
                                    <b-col cols="3">
                                        <font-awesome-icon icon="car-battery" size="2x" style="color: red"/>
                                    </b-col>
                                    <b-col cols="6">
                                        <p class="m-0 p-0" style="line-height: 1; font-size: 12px;">Работает от батареи</p>
                                        <p class="m-0 p-0" style="line-height: 1; font-size: 12px;">Осталось от батареи</p>
                                    </b-col>
                                    <b-col cols="3">
                                        <p class="m-0 p-0 font-weight-bold" style="line-height: 1">{{ Math.floor(row.item.battery_second / 60) }} мин</p>
                                        <p class="m-0 p-0 font-weight-bold" style="line-height: 1">{{ row.item.battery_min_remaining }} мин</p>
                                    </b-col>
                                </b-row>
                            </template>
                        </b-table>
                    </template>
                </div>
            </b-card-body>
        </template>
    </b-card>
</template>

<script>
    import { mapGetters } from 'vuex'
    export default {
        props: ["exception", "items"],
        data() {
            return {
                fields: [
                    { key: "name", label: "#", 'thStyle': 'width:52px;' },
                    { key: "input_vol", 'thStyle': 'width:52px;' },
                    { key: "output_vol", 'thStyle': 'width:51px;' },
                    { key: "ups_load", 'thStyle': 'width:52px;' },
                    { key: "battery_temp", 'thStyle': 'width:51px;' },
                    { key: "battery_capacity", 'thStyle': 'width:52px;' }
                ],
                alarmEnable: true
            }
        },
        methods: {
            upsCaclulate(placeid) {
                let arr = this.items.filter(obj => obj.placeid === placeid);
                return arr.map(x => {
                    if(x.battery_second != 0) {
                        x._rowVariant = 'danger';
                        x._showDetails = true;
                    }
                    return x;
                });
            }
        },
        computed: {
            ...mapGetters(['getPlace']),
            alarmDetected() {
                if(!this.alarmEnable) return false;
                if(this.items) {
                    let result = false;
                    this.items.forEach(x => { if(x.battery_second != 0) result = true });
                    return result;
                }
            }
        }
    }
</script>

<style scoped>

</style>