<template>
    <b-card no-body header-class="card-header-progress" class="btnHidden">
        <div slot="header" class="text-center font-weight-bold">
            <div style="font-size:14px; cursor: default;">
                <font-awesome-icon icon="user"/> Пользователи
            </div>
        </div>
        <b-card-body class="p-0 m-0">
            <b-table class="mb-0" hover small :foot-clone="true" :fields="fields" :items="items">
                <template slot="HEAD_place_description" slot-scope="row">
                    <span v-b-tooltip="{title: 'Название площадки', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">Площадка</span>
                </template>
                <template slot="HEAD_naumen" slot-scope="row">
                    <span v-b-tooltip="{title: 'Зарегистрировано в Naumen', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">Онл.</span>
                </template>
                <template slot="HEAD_hosts_count" slot-scope="row">
                    <span v-b-tooltip="{title: 'Работающие устройства', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">Хосты</span>
                </template>

                <template slot="FOOT_place_description" slot-scope="row">
                    <span style="cursor: default">Всего</span>
                </template>
                <template slot="FOOT_naumen" slot-scope="row">
                    <span style="cursor: default">{{ hostsActiveCaclulate }}</span>
                </template>
                <template slot="FOOT_hosts_count" slot-scope="row">
                    <span style="cursor: default">{{ naumenActiveCaclulate }}</span>
                </template>
            </b-table>
        </b-card-body>
    </b-card>
</template>

<script>
    export default {
        props: ["items"],
        data() {
            return {
                fields: [
                    { key: 'place_description', label: 'Площадка', 'thStyle': 'width:200px; cursor: default', 'class': 'defaultCursor' },
                    { key: 'naumen', label: 'Онл.', 'class': 'text-center defaultCursor', 'thStyle': 'cursor: default' },
                    { key: 'hosts_count', label: 'Всего', 'class': 'text-center defaultCursor', 'thStyle': 'cursor: default' }
                ]
            }
        },
        computed: {
            hostsActiveCaclulate() {
                if(this.items.length !== 0) return this.items.reduce((a, b) => +a + +b.naumen, 0)
            },
            naumenActiveCaclulate() {
                if(this.items.length !== 0) return this.items.reduce((a, b) => +a + +b.hosts_count, 0)
            },
        }
    }
</script>

<style scoped>

</style>