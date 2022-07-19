<template>
    <b-container fluid>
        <br/>

        <b-row>
            <b-col cols="11" sm="11" md="11" lg="11" xl="10" offset="1">

                <b-row>
                    <b-col cols="12" sm="12" md="12" lg="12" xl="9" style="margin-top:15px;margin-bottom:15px;">
                        <b-form-select :options="pageOptions" size="sm" style="max-width:80px;" v-model="perPage" />

                        <b-btn v-if="dataLoading" class="ml-3" style="min-width:36px;" size="sm" variant="secondary" disabled>
                            <font-awesome-icon v-if="dataLoading" icon="cog" spin />
                        </b-btn>
                        <b-btn v-else class="ml-3" style="min-width:36px;" size="sm" variant="primary" @click="tableLoad" v-b-tooltip="{title: 'Обновить данные', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}">
                            <font-awesome-icon icon="retweet" />
                        </b-btn>
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

                    <template slot="webbr_register_datetime" slot-scope="row">
                        <abbr :title="$dt.fromISO(row.value)">{{ $dt.fromISO(row.value).toRelative() }}</abbr>
                    </template>

                    <template slot="webbr_auth_datetime" slot-scope="row">
                        <abbr :title="$dt.fromISO(row.value)">{{ $dt.fromISO(row.value).toRelative() }}</abbr>
                    </template>
                </b-table>

                <b-row>
                    <b-col md="6" class="d-flex align-self-center">
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
        props: ['apiUrl'],
        data() {
            return {
                items: [],
                fields: [
                    { key: 'ad_name', label: 'ФИО', sortable: true, 'thStyle': 'width:150px;', 'class': 'text-nowrap' },
                    { key: 'ad_login', label: 'Логин', sortable: true, 'thStyle': 'width:150px;', 'class': 'text-nowrap' },
                    { key: 'ad_department', label: 'Отдел', sortable: true, 'thStyle': 'width:70px;', 'class': 'text-nowrap' },
                    { key: 'webbr_role', label: 'Роль', sortable: true, 'thStyle': 'width:70px;', 'class': 'text-nowrap' },
                    { key: 'ad_place', label: 'Площадка', sortable: true, 'thStyle': 'width:60px;', 'class': 'text-nowrap' },
                    { key: 'webbr_register_datetime', label: 'Регистрация', sortable: true, 'thStyle': 'width:50px;', 'class': 'text-nowrap' },
                    { key: 'webbr_auth_datetime', label: 'Авторизация', sortable: true, 'thStyle': 'width:50px;', 'class': 'text-nowrap' }
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
            }
        },
        mounted() {
            this.$nextTick(() => this.$refs.searchInput.focus())
        },

        activated() {
            this.$nextTick(() => this.$refs.searchInput.focus())
        },

        computed: {
            filterTable() {
                let filterArray = this.filter.split(' ');
                let array = '^' + filterArray.map(x => x ? '(?=.*' + x + ')' : '').join('');
                return new RegExp(array, 'ig');
            }
        },
        
        methods: {
            onFiltered(filteredItems) {
                this.totalRows = filteredItems.length;
                // this.currentPage = 1
            },

            async tableLoad() {
                this.$nextTick(() => this.$refs.searchInput.focus());
                this.dataLoading = true;
                let response = await this.$store.dispatch(USER_GET, '/api/configuration/getusers');
                this.items = response.data;
                this.dataLoading = false;
                this.totalRows = this.items.length;
            }
        },
        async created() {
            await this.tableLoad();
        }
    }
</script>

<style scoped>

</style>