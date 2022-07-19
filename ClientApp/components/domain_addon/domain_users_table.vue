<template>
    <b-container fluid>
        <b-row>
            <b-col cols="12" sm="12" md="12" lg="12" xl="10" offset-xl="1">
                <b-row>
                    <b-col cols="12" sm="12" md="12" lg="12" xl="9" style="margin-top:15px; margin-bottom:15px;">
                        <b-form-select :options="pageOptions" size="sm" style="max-width:80px;" v-model="perPage" />
                        
                        <b-btn v-if="dataLoading" class="ml-3" style="min-width:36px;" size="sm" variant="secondary" disabled>
                            <font-awesome-icon v-if="dataLoading" icon="cog" spin />
                        </b-btn>
                        <b-btn v-else class="ml-3" style="min-width:36px;" size="sm" variant="primary" @click="tableLoad" v-b-tooltip="{title: 'Обновить данные', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}">
                            <font-awesome-icon icon="retweet" />
                        </b-btn>

                        <b-form-radio-group class="ml-3" buttons button-variant="outline-success" size="sm" v-model="radioUserAccountSelected">
                            <b-form-radio value="" v-b-tooltip="{title: 'Показать все', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Все</b-form-radio>
                            <b-form-radio value="normal_account" v-b-tooltip="{title: 'Активные пользователи', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Активные</b-form-radio>
                            <b-form-radio value="disabled_account" v-b-tooltip="{title: 'Заблокированные пользователи', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Заблокированные</b-form-radio>
                        </b-form-radio-group>
                    </b-col>

                    <b-col cols="12" sm="12" md="12" lg="12" xl="3" style="margin-top:15px; margin-bottom:15px;">
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
                    
                    <template slot="user_username" slot-scope="row">
                        <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{row.value}}</span>
                    </template>

                    <template slot="user_login" slot-scope="row">
                        <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{row.value}}</span>
                    </template>

                    <template slot="user_mail" slot-scope="row">
                        <a :href="'mailto:'+row.value+''">{{row.value}}</a>
                    </template>

                    <template slot="user_department" slot-scope="row">
                        <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{row.value}}</span>
                    </template>
                    
                    <template slot="user_description" slot-scope="row">
                        <div v-if="row.value && row.value.length > 25">
                            <span @click="copyToClipboard(row.value)" style="cursor: pointer" v-b-tooltip.top.hover :title="row.value">{{row.value | truncate(25, '...')}}</span>
                        </div>
                        <template v-else>
                            <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{row.value}}</span>
                        </template>
                    </template>

                    <template slot="user_city" slot-scope="row">
                        <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{row.value}}</span>
                    </template>

                    <template slot="user_mobile_phone" slot-scope="row">
                        <div v-if="row.value && row.item.user_naumen_phone">
                            <span v-b-tooltip="{title: 'Мобильный', placement: 'top', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help;">
                                {{row.value}}
                            </span>
                            •
                            <span v-b-tooltip="{title: 'Naumen', placement: 'top', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help;">
                                {{row.item.user_naumen_phone}}
                            </span>
                        </div>

                        <span v-else-if="row.value" v-b-tooltip="{title: 'Мобильный', placement: 'top', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help;">
                            {{row.value}}
                        </span>

                        <span v-else-if="row.item.user_naumen_phone" v-b-tooltip="{title: 'Naumen', placement: 'top', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help;">
                            {{row.item.user_naumen_phone}}
                        </span>

                        <template v-else/>
                    </template>

                    <template slot="user_groups" slot-scope="row">
                        <span v-if="row.value.length === 0"><font-awesome-icon size="sm" icon="layer-group"/> Групп: <b>0</b></span>

                        <div v-else>
                            <span :id="'popover2' + row.item.user_guid" style="cursor: pointer;"><font-awesome-icon size="sm" icon="layer-group"/> Групп: <b>{{ row.value.split(';').length }}</b></span>
                            <b-popover :target="'popover2' + row.item.user_guid"
                                       placement="left"
                                       triggers="click focus blur hover">

                                <div class="popover-body-overflow">
                                    <p class="m-0 p-0" v-for="(item, index) in row.value.split(';')"><b>{{ index + 1}}.</b> {{ item }}</p>
                                </div>
                            </b-popover>
                        </div>
                    </template>
                    
                    <template slot="user_logon_datetime" slot-scope="row">
                        <abbr :title="$dt.fromISO(row.value)">{{ $dt.fromISO(row.value).toRelative() }}</abbr>
                    </template>

                    <template slot="user_create_datetime" slot-scope="row">
                        <abbr :title="$dt.fromISO(row.value)">{{ $dt.fromISO(row.value).toRelative() }}</abbr>
                    </template>
                    
                </b-table>

                <b-row>
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
    import { mapGetters } from 'vuex'
    import { USER_GET } from '../../store/actions/user'
    
    export default {
        data() {
            return {
                items: [],
                fields: [
                    { key: 'user_computer', label: 'Имя компьютера', sortable: true, 'thStyle': 'width:150px;', 'class': 'text-nowrap' },
                    { key: 'user_username', label: 'Имя', sortable: true, 'thStyle': 'width:150px;', 'class': 'text-nowrap' },
                    { key: 'user_login', label: 'Логин', sortable: true, 'thStyle': 'width:100px;', 'class': 'text-nowrap' },
                    { key: 'user_mail', label: 'E-mail', sortable: true, 'thStyle': 'width:100px;', 'class': 'text-nowrap' },
                    { key: 'user_department', label: 'Отдел', sortable: true, 'thStyle': 'width:100px;', 'class': 'text-nowrap' },
                    { key: 'user_description', label: 'Должность', sortable: true, 'thStyle': 'width:100px;', 'class': 'text-nowrap' },
                    { key: 'user_city', label: 'Город', sortable: true, 'thStyle': 'width:100px;', 'class': 'text-nowrap' },
                    { key: 'user_mobile_phone', label: 'Телефон', sortable: true, 'thStyle': 'width:100px;', 'class': 'text-nowrap' },
                    { key: 'user_groups', label: 'Группы', sortable: true, 'thStyle': 'width:100px;', 'class': 'text-nowrap' },
                    { key: 'user_logon_datetime', label: 'Время входа', sortable: true, 'thStyle': 'width:70px;', 'class': 'text-nowrap' }
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
                radioUserAccountSelected: 'normal_account'
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
                // this.currentPage = 1
            },

            async tableLoad() {
                this.$nextTick(() => this.$refs.searchInput.focus());
                this.dataLoading = true;
                let response = await this.$store.dispatch(USER_GET, '/api/domain/getdomainusers');
                this.items = response.data.map(x => { if(x.user_disabled === "disabled_account") x._rowVariant = 'secondary'; return x; });
                this.dataLoading = false;
                this.totalRows = this.items.length;
            },
            
            copyToClipboard(item) {
                const el = document.createElement('textarea');
                el.value = item;
                document.body.appendChild(el);
                el.select();
                document.execCommand('copy');
                document.body.removeChild(el);
            }
        },
        computed: {
            ...mapGetters(['getPlace']),

            filterTable() {
                let filterArray = this.filter.split(' ');
                filterArray = filterArray.concat(this.radioUserAccountSelected.split(' '));
                let array = '^' + filterArray.map(x => x ? '(?=.*' + x + ')' : '').join('');
                return new RegExp(array, 'ig');
            }
        },
        async created() {
            await this.tableLoad();
        }
    }
</script>


