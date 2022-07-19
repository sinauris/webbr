<template>
    <b-container fluid>
        <b-row>
            <b-col cols="12" sm="12" md="12" lg="12" xl="10" offset-xl="1">

                <b-row>
                    <b-col cols="12" sm="12" md="12" lg="12" xl="9" style="margin-top:15px;margin-bottom:15px;">
                        <b-form-select
                            :options="pageOptions"
                            size="sm"
                            style="max-width:80px;"
                            v-model="perPage"/>

                        <b-btn v-if="dataLoading" class="ml-3" style="min-width:36px;" size="sm" variant="secondary" disabled>
                            <font-awesome-icon v-if="dataLoading" icon="cog" spin />
                        </b-btn>
                        <b-btn v-else class="ml-3" style="min-width:36px;" size="sm" variant="primary" @click="load" v-b-tooltip="{title: 'Обновить данные', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}">
                            <font-awesome-icon icon="retweet" />
                        </b-btn>

                        <b-form-radio-group class="ml-3"  buttons button-variant="outline-success" size="sm" v-model="radioOSSelected">
                            <b-form-radio value="" v-b-tooltip="{title: 'Показать все', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Все</b-form-radio>
                            <b-form-radio value="linux" v-b-tooltip="{title: 'Показать Linix', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Linux</b-form-radio>
                            <b-form-radio value="windows" v-b-tooltip="{title: 'Показать Windows', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Windows</b-form-radio>
                        </b-form-radio-group>
                    </b-col>

                    <b-col cols="12" sm="12" md="12" lg="12" xl="3" style="margin-top:15px;margin-bottom:15px;">
                        <b-input-group size="sm">
                            <b-input-group-text slot="prepend">
                                <font-awesome-icon icon="search"/>
                            </b-input-group-text>
                            <b-form-input
                                ref="searchInput"
                                autofocus
                                v-model="filter"
                                placeholder="Поиск"/>
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
                        <b-spinner style="width: 27rem; height: 27rem" type="grow" />
                    </div>

                    <template slot="HEAD_typeos" slot-scope="data">
                        <font-awesome-icon icon="bolt" size="lg" class="text-muted" style="min-width: 22px;" />
                    </template>

                    <template slot="typeos" slot-scope="row">
                        <div v-if="row.value === 'Linux'"><font-awesome-icon :icon="['fab', 'linux']"/></div>
                        <div v-else-if="row.value === 'Windows'"><font-awesome-icon :icon="['fab', 'windows']"/></div>
                        <div v-else></div>
                    </template>

                    <template slot="rm" slot-scope="row">
                        <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{ row.value }}</span>
                    </template>

                    <template slot="mac" slot-scope="row">
                        <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{ row.value }}</span>
                    </template>

                    <template slot="motherboard" slot-scope="row">
                        <template v-if="row.value === 'None'"/>
                        <template v-else>
                            <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{ row.value }}</span>
                        </template>
                    </template>

                    <template slot="cpu" slot-scope="row">
                        <template v-if="row.value === 'None'"/>
                        <template v-else>
                            <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{ row.value }}</span>
                        </template>
                    </template>

                    <template slot="ram" slot-scope="row">
                        <template v-if="row.value === 'None' || !row.value"/>
                        <template v-else>
                            <span :id="'popoverMem' + row.item.id" style="cursor: pointer;">Планок: <b>{{ row.value.split(';').length }}</b> • Ёмкость: <b>{{ row.item.ram_size }} ГБ</b></span>
                            <b-popover :target="'popoverMem' + row.item.id"
                                       placement="left"
                                       triggers="click focus blur hover">
                                <div class="popover-body-overflow">
                                    <p class="m-0 p-0" v-for="(item, index) in row.value.split(';')"><b>{{ index + 1}}.</b> {{ item }}</p>
                                </div>
                            </b-popover>
                        </template>
                    </template>

                    <template slot="hdd" slot-scope="row">
                        <template v-if="row.value === 'None' || !row.value"/>
                        <template v-else>
                            <span :id="'popoverHdd' + row.item.id" style="cursor: pointer;">Дисков: <b>{{ row.value.split(';').length }}</b> • Ёмкость: <b>{{ row.item.hdd_size }} ГБ</b></span>
                            <b-popover :target="'popoverHdd' + row.item.id"
                                       placement="left"
                                       triggers="click focus blur hover">

                                <div class="popover-body-overflow">
                                    <p class="m-0 p-0" v-for="(item, index) in row.value.split(';')"><b>{{ index + 1}}.</b> {{ item }}</p>
                                </div>
                            </b-popover>
                        </template>
                    </template>

                    <template slot="monitor" slot-scope="row">
                        <template v-if="row.value === 'None' || !row.value"/>
                        <template v-else>
                            <span :id="'popoverMonitor' + row.item.id" style="cursor: pointer;">Мониторов: <b>{{ row.value.split('; ').length }}</b></span>
                            <b-popover :target="'popoverMonitor' + row.item.id"
                                       placement="left"
                                       triggers="click focus blur hover">

                                <div class="popover-body-overflow">
                                    <p class="m-0 p-0" v-for="(item, index) in row.value.split('; ')"><b>{{ index + 1}}.</b> {{ item.replace(';', '') }}</p>
                                </div>
                            </b-popover>
                        </template>
                    </template>
                </b-table>

                <b-row style="margin-top:10px;margin-bottom:10px;">
                    <b-col md="5" class="d-flex align-self-center">
                        <div>Всего на странице <b>{{ totalRows }}</b></div>
                    </b-col>

                    <b-col md="2" v-if="this.totalRows > perPage"  class="ml-auto">
                        <b-pagination align="right" :total-rows="totalRows" :per-page="perPage" v-model="currentPage" class="my-0" ></b-pagination>
                    </b-col>
                </b-row>
            </b-col>
        </b-row>
    </b-container>

</template>

<script>
    import { USER_GET } from '../../store/actions/user'
    
    export default {
        data() {
            return {
                items: [],
                
                fields: [
                    { key: 'typeos', 'class': 'text-center text-nowrap', 'thClass': 'headcol', 'tdClass': 'headcol', 'thStyle': 'width:38px;' },
                    { key: 'rm', label: 'РМ', sortable: true, 'class': 'text-nowrap' },
                    { key: 'mac', label: 'MAC', sortable: true, 'class': 'text-nowrap' },
                    { key: 'motherboard', label: 'Мат. плата', sortable: true, 'class': 'text-nowrap' },
                    { key: 'cpu', label: 'Процессор', sortable: true, 'class': 'text-nowrap' },
                    { key: 'ram', label: 'Память', sortable: true, 'class': 'text-nowrap' },
                    { key: 'hdd', label: 'Диски', sortable: true, 'class': 'text-nowrap' },
                    { key: 'monitor', label: 'Мониторы', sortable: true, 'class': 'text-nowrap' },
                    { key: 'updated', label: 'Обновлено', sortable: true, 'class': 'text-nowrap' }
                ],
                currentPage: 1,
                perPage: 20,
                totalRows: 0,
                pageOptions: [
                    { value: 20, text: '20' },
                    { value: 50, text: '50' },
                    { value: 100, text: '100' }
                ],
                sortBy: null,
                sortDesc: false,
                filter: '',
                dataLoading: false,
                radioOSSelected: '',
                navigateVariable: ''
            }
        },

        mounted() {
            this.$nextTick(() => this.$refs.searchInput.focus())
        },

        activated() {
            this.$nextTick(() => this.$refs.searchInput.focus())
        },

        watch: {
            async '$route'() { if (this.$route.name === 'inventoryid' && this.$route.params.id) await this.tableLoad() }
        },

        methods: {
            onFiltered(filteredItems) {
                this.totalRows = filteredItems.length;
                // this.currentPage = 1
            },

            changeModal(item) {
                this.modal.rm = item.rm;
                this.modal.invnumber = item.invnumber;
                this.modal.mac = item.mac;
            },
            
            copyToClipboard(item) {
                const el = document.createElement('textarea');
                el.value = item;
                document.body.appendChild(el);
                el.select();
                document.execCommand('copy');
                document.body.removeChild(el);
            },

            async load() {
                this.$nextTick(() => this.$refs.searchInput.focus());
                this.currentPage = 1;
                this.dataLoading = true;
                let response = await this.$store.dispatch(USER_GET, '/api/inventory/getinventory' + this.$route.params.id);
                this.items = response.data;
                this.dataLoading = false;
                this.totalRows = this.items.length;
            },
            async tableLoad() {
                if (this.items.length === 0) {
                    this.navigateVariable = this.$route.fullPath;
                    await this.load();
                }
                else {
                    if (this.navigateVariable !== this.$route.fullPath) {
                        this.navigateVariable = this.$route.fullPath;
                        await this.load();
                    }
                }
            }
        },
        computed: {
            filterTable() {
                let filterArray = this.filter.split(' ');
                filterArray = filterArray.concat(this.radioOSSelected.split(' '));
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