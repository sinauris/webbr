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
                    
                    <template slot="group_name" slot-scope="row">
                        <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{row.value}}</span>
                    </template>

                    <template slot="group_name" slot-scope="row">
                        <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{row.value}}</span>
                    </template>
                    
                    <template slot="group_description" slot-scope="row">
                        <div v-if="row.value  && row.value.length > 110">
                            <span @click="copyToClipboard(row.value)" style="cursor:pointer;" v-b-tooltip.top.hover :title="row.value">{{row.value | truncate(110, '...')}}</span> 
                        </div>
                        <template v-else>
                            <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{row.value}}</span>
                        </template>
                    </template>

                    <template slot="group_member" slot-scope="row">
                        <span v-if="row.value.length === 0"><font-awesome-icon size="sm" icon="layer-group"/> Пользователей: <b>0</b></span>

                        <div v-else>
                            <span :id="'popoverMember' + row.item.group_guid" style="cursor: pointer;"><font-awesome-icon size="sm" icon="layer-group"/> Пользователей: <b>{{ row.value.split(';').length }}</b></span>
                            <b-popover :target="'popoverMember' + row.item.group_guid"
                                       placement="left"
                                       triggers="click hover blur focus">

                                <div class="popover-body-overflow">
                                    <p class="m-0 p-0" v-for='(item, index) in row.value.split(";")'><b>{{ index + 1}}.</b> {{ item }}</p>
                                </div>
                            </b-popover>
                        </div>
                    </template>

                    <template slot="group_memberOf" slot-scope="row">
                        <span v-if="row.value.length === 0"><font-awesome-icon size="sm" icon="layer-group"/> Групп: <b>0</b></span>

                        <div v-else>
                            <span :id="'popoverMemberOf' + row.item.group_guid" style="cursor: pointer;"><font-awesome-icon size="sm" icon="layer-group"/> Групп: <b>{{ row.value.split(';').length }}</b></span>
                            <b-popover :target="'popoverMemberOf' + row.item.group_guid"
                                       placement="left"
                                       triggers="click hover blur focus">

                                <div class="popover-body-overflow">
                                    <p class="m-0 p-0" v-for='(item, index) in row.value.split(";")'><b>{{ index + 1}}.</b> {{ item }}</p>
                                </div>
                            </b-popover>
                        </div>
                    </template>

                    <template slot="group_change_datetime" slot-scope="row">
                        <abbr :title="$dt.fromISO(row.value)">{{ $dt.fromISO(row.value).toRelative() }}</abbr>
                    </template>
                    <template slot="group_create_datetime" slot-scope="row">
                        <abbr :title="$dt.fromISO(row.value)">{{ $dt.fromISO(row.value).toRelative() }}</abbr>
                    </template>

                </b-table>

                <b-row>
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
    import { USER_GET } from '../../store/actions/user'
    const items = [];
    export default {
        data() {
            return {
                items: [],
                fields: [
                    { key: 'group_name', label: 'Имя группы', sortable: true, 'class': 'text-nowrap' },
                    { key: 'group_description', label: 'Описание', sortable: true, 'class': 'text-nowrap' },
                    { key: 'group_member', label: 'Пользователи', sortable: true, 'class': 'text-nowrap' },
                    { key: 'group_memberOf', label: 'Группы', sortable: true, 'class': 'text-nowrap' },
                    { key: 'group_change_datetime', label: 'Время изменения', sortable: true, 'class': 'text-nowrap' },
                    { key: 'group_create_datetime', label: 'Время создания', sortable: true, 'class': 'text-nowrap' }
                ],
                currentPage: 1,
                perPage: 20,
                totalRows: items.length,
                pageOptions: [
                    { value: 20, text: '20' },
                    { value: 50, text: '50' },
                    { value: 100, text: '100' }
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
                // this.currentPage = 1
            },

            copyToClipboard(item) {
                const el = document.createElement('textarea');
                el.value = item;
                document.body.appendChild(el);
                el.select();
                document.execCommand('copy');
                document.body.removeChild(el);
            },
            
            async tableLoad() {
                this.$nextTick(() => this.$refs.searchInput.focus());
                this.dataLoading = true;
                let response = await this.$store.dispatch(USER_GET, '/api/domain/getdomaingroups');
                this.items = response.data;
                this.dataLoading = false;
                this.totalRows = this.items.length;
            }
        },
        computed: {
            filterTable() {
                let filterArray = this.filter.split(' ');
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
    .popover-body {
        max-height: 20rem;
        overflow-y: auto;
        white-space:pre-wrap;
    }
</style>


