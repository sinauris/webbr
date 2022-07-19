<template>
    <b-container fluid>
        <br/>

        <b-row>
            <b-col cols="12" sm="12" md="12" lg="12" xl="10" offset-xl="1">

                <b-row>
                    <b-col cols="12" sm="12" md="12" lg="12" xl="9" style="margin-top:15px;margin-bottom:15px;">
                        <b-form-select :options="pageOptions" size="sm" style="max-width:80px;" v-model="perPage" />

                        <b-btn v-if="dataLoading" class="ml-3" style="min-width:36px;" size="sm" variant="secondary" disabled>
                            <font-awesome-icon v-if="dataLoading" icon="cog" spin />
                        </b-btn>
                        <b-btn v-else class="ml-3" style="min-width:36px;" size="sm" variant="primary" @click="tableLoad" v-b-tooltip="{title: 'Обновить данные', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}">
                            <font-awesome-icon icon="retweet" />
                        </b-btn>
                        
                        <b-form-radio-group buttons size="sm" class="mx-3" button-variant="outline-success" v-model="radioServerStateSelected">
                            <b-form-radio value="" v-b-tooltip="{title: 'Показать все', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Все</b-form-radio>
                            <b-form-radio value="poweredOn" v-b-tooltip="{title: 'Показать включенные', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Включены</b-form-radio>
                            <b-form-radio value="suspended" v-b-tooltip="{title: 'Показать приостановленные', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Приостановлены</b-form-radio>
                            <b-form-radio value="poweredOff" v-b-tooltip="{title: 'Показать выключенные', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Выключены</b-form-radio>
                        </b-form-radio-group>

<!--                        <b-form-select :options="groupOptions" size="sm" style="max-width:80px;" v-model="groupSelect" />-->
                    </b-col>

                    <b-col cols="12" sm="12" md="12" lg="12" xl="3" style="margin-top:15px;margin-bottom:15px;">
                        <b-input-group size="sm">
                            <b-input-group-text slot="prepend">
                                <font-awesome-icon icon="search" />
                            </b-input-group-text>
                            <b-form-input
                                ref="searchInput"
                                autofocus
                                v-model="filter"
                                placeholder="Поиск" />
                        </b-input-group>
                    </b-col>

                </b-row>

                <b-table
                    :busy="dataLoading"
                    empty-filtered-text="Ничего не найдено"
                    show-empty
                    responsive
                    :bordered="true"
                    :hover="true"
                    :items="items"
                    :fields="fields"
                    :current-page="currentPage"
                    :per-page="perPage"
                    :filter="filterTable"
                    :sort-by.sync="sortBy"
                    :sort-desc.sync="sortDesc"
                    @filtered="onFiltered"
                >
                    <div slot="table-busy" class="text-center">
                        <b-spinner style="width: 32rem; height: 32rem" type="grow" />
                    </div>
                    
                    <template slot="HEAD_abtn" slot-scope="data">
                        <font-awesome-icon class="text-muted" icon="bolt" size="lg"/>
                    </template>

                    <template slot="abtn" slot-scope="row">
                        <b-btn v-if="row.item.vmState === 'poweredOn'" class="action-button" style="width: 100%" @click="sshsCommand(row.item.vmIpaddress)"><font-awesome-icon icon="terminal" style="color:#28a745"/></b-btn>
                        <b-btn v-if="row.item.vmState === 'suspended'" class="action-button" style="width: 100%" disabled><font-awesome-icon icon="terminal" style="color:#ffc107"/></b-btn>
                        <b-btn v-else-if="row.item.vmState === 'poweredOff'" class="action-button" style="width: 100%" disabled><font-awesome-icon icon="terminal" style="color:#6c757d"/></b-btn>
                    </template>

                    <template slot="hypervisorIp" slot-scope="row">
                        <a v-if="row.value.includes('10.254.21.37')" :href="'https://'+row.value+':8006'" target="_blank">{{ row.value }}</a>
                        <a v-else :href="'http://'+row.value+''" target="_blank">{{ row.value }}</a>
                    </template>

                    <template slot="vmCluster" slot-scope="row">
                        <span @click="copyToClipboard(row.value.toUpperCase())" style="cursor: pointer">{{row.value.toUpperCase()}}</span>
                    </template>

                    <template slot="vmHost" slot-scope="row">
                        <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{row.value}}</span>
                    </template>

                    <template slot="vmGroup" slot-scope="row">
                        <div style="cursor:default;">
                            <b>{{ row.value }}</b>
                        </div>
                    </template>
                    
                    <template slot="vmName" slot-scope="row">
                        <template v-if="row.value === 'None'"/>
                        <template v-else>
                            <span @click="copyToClipboard(row.value.toLowerCase())" style="cursor: pointer">{{row.value.toLowerCase()}}</span>
                        </template>
                    </template>

                    <template slot="vmIpaddress" slot-scope="row">
                        <template v-if="row.value === 'None'"/>
                        <template v-else>
                            <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{row.value}}</span>
                        </template>
                    </template>

                    <template slot="vmDnsname" slot-scope="row">
                        <template v-if="row.value === 'None'"/>
                        <template v-else>
                            <span @click="copyToClipboard(row.value.toLowerCase())" style="cursor: pointer">
                                {{ dnsNameReplace(row.value.toLowerCase()) }}
                            </span>
                        </template>
                    </template>

                    <template slot="vmGuestos" slot-scope="row">
                        <div v-if="row.value !== 'None' && row.value.indexOf('Microsoft')">
                            <span @click="copyToClipboard(row.value)" style="cursor: pointer"><font-awesome-icon :icon="['fab', 'linux']"/> {{ row.value }}</span>
                        </div>
                        <template v-else-if="row.value === 'None'"/>
                        <div v-else>
                            <span @click="copyToClipboard(row.value)" style="cursor: pointer"><font-awesome-icon :icon="['fab', 'windows']"/> {{ row.value }}</span>
                        </div>
                    </template>

                    <template slot="vmCpu" slot-scope="row">
                        <div style="cursor:default;">{{ row.value }}</div>
                    </template>
                    
                    <template slot="vmMemory" slot-scope="row">
                        <div style="cursor:default;">{{ formatBytes(row.value) }}</div>
                    </template>

                    <template slot="vmAnnotation" slot-scope="row">
                        <template v-if="row.value === 'None'"/>
                        <template v-else>
                            <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{row.value}}</span>
                        </template>
                    </template>
                    
                </b-table>
                
                <b-row style="margin-top:10px;margin-bottom:10px;">
                    <b-col md="5" class="d-flex align-self-center">
                        <div>Всего на странице <b>{{ totalRows }}</b></div>
                    </b-col>

                    <b-col md="2" v-if="this.totalRows > perPage" class="ml-auto">
                        <b-pagination align="right" :total-rows="totalRows" :per-page="perPage" v-model="currentPage" class="my-0" ></b-pagination>
                    </b-col>
                </b-row>
            </b-col>
        </b-row>
    </b-container>
</template>

<script>
    import { USER_GET } from '../store/actions/user'

    const items = [];

    export default {
        data() {
            return {
                items: [],
                radioServerStateSelected: 'poweredOn',
                fields: [
                    { key: 'abtn', 'tdClass': 'betterButton', 'class': 'text-center text-nowrap' },
                    { key: 'hypervisorIp', label: 'Гипервизор', sortable: true, 'class': 'text-nowrap' },
                    { key: 'vmCluster', label: 'Кластер', sortable: true, 'class': 'text-nowrap' },
                    { key: 'vmHost', label: 'Хост', sortable: true, 'class': 'text-nowrap' },
                    { key: 'vmGroup', label: 'Группа', sortable: true, 'class': 'text-nowrap' },
                    { key: 'vmName', label: 'Имя', sortable: true, 'class': 'text-nowrap' },
                    { key: 'vmDnsname', label: 'DNS', sortable: true, 'class': 'text-nowrap' },
                    { key: 'vmIpaddress', label: 'IP-адрес', sortable: true, 'class': 'text-nowrap' },
                    { key: 'vmGuestos', label: 'Гостевая ОС', sortable: true, 'class': 'text-nowrap' },
                    { key: 'vmCpu', label: 'Ядер', sortable: true, 'class': 'text-nowrap' },
                    { key: 'vmMemory', label: 'Память', sortable: true, 'class': 'text-nowrap' },
                    { key: 'vmAnnotation', label: 'Описание', sortable: true, 'class': 'text-nowrap' }
                ],
                currentPage: 1,
                perPage: 20,
                totalRows: 0,
                pageOptions: [
                    { value: 20, text: '20' },
                    { value: 50, text: '50' },
                    { value: 100, text: '100' }
                ],
                
                groupSelect: 'Linux',
                groupOptions: [
                    { value: 'MSC OIT', text: 'MSC OIT' },
                    { value: 'Linux', text: 'Linux' },
                    { value: 'Net', text: '100' }
                ],
                sortBy: null,
                sortDesc: false,
                filter: '',
                dataLoading: false
            }
        },

        mounted() {
            this.$nextTick(() => this.$refs.searchInput.focus())
        },
        activated() {
            this.$nextTick(() => this.$refs.searchInput.focus())
        },
        methods: {
            onFiltered(filteredItems) {
                this.totalRows = filteredItems.length;
            },

            sshsCommand: async function(ip) {
                window.location.href = "ssh://" + ip;
            },
            
            formatBytes(a,b) {
                if(0==a)return"-";a=a.replace(/[^0-9]/g,'');let c=1024,d=b||2,e=["МБ","ГБ","ТБ","ПБ","EB","ZB","YB"],f=Math.floor(Math.log(a)/Math.log(c));return parseFloat((a/Math.pow(c,f)).toFixed(d))+" "+e[f]
            },
            
            copyToClipboard(item) {
                const el = document.createElement('textarea');
                el.value = item;
                document.body.appendChild(el);
                el.select();
                document.execCommand('copy');
                document.body.removeChild(el);
            },
            
            dnsNameReplace(dnsname) {
                if(dnsname) {
                    return dnsname.replace(/.call-center.newcontact.su|.newcontact.su/gi,'');
                }
            },
            
            async tableLoad() {
                this.$nextTick(() => this.$refs.searchInput.focus());
                this.dataLoading = true;
                let response = await this.$store.dispatch(USER_GET, '/api/server/getvirtualservers');
                this.items = response.data;
                this.dataLoading = false;
                this.totalRows = this.items.length;
            }
        },
        computed: {
            filterTable() {
                let filterArray = this.filter.split(' ');
                filterArray = filterArray.concat(this.radioServerStateSelected.split(' '));
                let array = '^' + filterArray.map(x => x ? '(?=.*' + x + ')' : '').join('');
                return new RegExp(array, 'ig');
            }
        },
        async created() {
            await this.tableLoad();
        }
    }
</script>

<style scoped>

</style>