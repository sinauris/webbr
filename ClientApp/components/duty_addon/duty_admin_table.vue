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
                        <b-spinner style="width: 32rem; height: 32rem" type="grow" />
                    </div>

                    <template slot="HEAD_onlinestatus" slot-scope="data">
                        <font-awesome-icon icon="bolt" size="lg" class="text-muted"/>
                    </template>

                    <template slot="balance" slot-scope="row">
                        {{row.value}} руб.
                    </template>

                    <template slot="balance2" slot-scope="row" v-if="row.value">
                        {{row.value}} руб.
                    </template>

                    <template slot="comment" slot-scope="row">
                        <div>
                            <span v-if="row.value.length > 70" style="cursor:pointer;" :id="'commentPop-'+row.item.id">
                                {{row.value | truncate(70, '...')}}
                            </span>
                            <b-popover v-if="row.value.length > 55" :target="'commentPop-'+row.item.id" placement="top" triggers="click blur hover">
                                {{row.item.comment}}
                            </b-popover>
                            <template v-else>{{row.value}}</template>
                        </div>
                    </template>

                    <template slot="date" slot-scope="row">
                        <template v-if="timeConstruct(row.value)">
                            <abbr :title="$dt.fromISO(row.value)">{{ $dt.fromISO(row.value).toRelative() }}</abbr>
                        </template>
                        <template v-else>
                            {{ row.value }}
                        </template>
                    </template>

                    <template slot="HEAD_actions" slot-scope="data">
                        <font-awesome-icon
                                v-if="dataLoading"
                                icon="cog"
                                size="lg"
                                spin
                                disabled />

                        <font-awesome-icon
                                v-else
                                @click="load"
                                icon="retweet"
                                size="lg"
                                style="cursor:pointer;"
                                v-b-tooltip="{title: 'Обновить данные', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"/>
                    </template>

                    <template slot="actions" slot-scope="row">
                        <b-btn-group style="width: 100%">
                            <b-btn
                                class="action-button"
                                size="sm" style="width:100%"
                                @click="updateModalDuty(row.item)"
                                v-b-modal.changeDuty v-b-tooltip="{title: 'Изменить дежурство', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}">
                                <font-awesome-icon icon="pencil-alt"/>
                            </b-btn>
                            <b-btn
                                class="action-button"
                                size="sm" style="width:100%"
                                @click="deleteDuty(row.item)" v-b-tooltip="{title: 'Удалить дежурство', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}">
                                <font-awesome-icon icon="trash-alt"/>
                            </b-btn>
                        </b-btn-group>
                    </template>
                </b-table>

                <b-row>
                    <b-col v-if="this.allFilteredItems.length > perPage" md="2" class="ml-auto">
                        <b-pagination align="right" :total-rows="totalRows" :per-page="perPage" v-model="currentPage" class="my-0" />
                    </b-col>
                </b-row>

            </b-col>
        </b-row>

        <b-modal
            id="changeDuty"
            ref="changeDutyRef"
            title="Изменение дежурства"
            hide-footer>
            <b-form @submit.stop.prevent="updateDuty">
                <b-form-group label-for="modalName">
                    <template slot="label">
                        <b>Имя дежурного</b>
                    </template>
                    <b-form-input size="sm" id="modalName" type="text" v-model="modal.name" disabled/>
                </b-form-group>
                <b-form-group label-for="modalStpinc">
                    <template slot="label">
                        <b>Инциденты</b>
                    </template>
                    <b-form-input size="sm" id="modalStpinc" type="text" v-model="modal.stpinc"/>
                </b-form-group>
                <b-form-group label-for="modalBalance">
                    <template slot="label">
                        <b>Баланс</b> <span style="color: red">*</span>
                    </template>
                    <b-input-group size="sm" append="руб." style="cursor: default">
                        <b-form-input size="sm" id="modalBalance" type="text" required v-model="modal.balance"/>
                    </b-input-group>
                </b-form-group>
                <b-btn v-if="!balance2collapseVisible" block variant="light" size="sm" class="pt-0 pb-0 mb-3" v-b-toggle.balance2collapse><font-awesome-icon icon="plus" size="sm"/> Баланс второй сим-карты</b-btn>
                <b-collapse id="balance2collapse" class="mt-2" v-model="balance2collapseVisible">
                    <b-form-group label-for="modalBalance2">
                        <template slot="label">
                            <b>Баланс второй сим-карты</b>
                        </template>
                        <b-input-group size="sm" append="руб." style="cursor: default">
                            <b-form-input size="sm" id="modalBalance2" type="text" v-model="modal.balance2"/>
                        </b-input-group>
                    </b-form-group>
                </b-collapse>

                <b-form-group label-for="modalComment">
                    <template slot="label">
                        <b>Комментарий</b>
                    </template>
                    <b-form-textarea id="modalComment"
                                     v-model="modal.comment"
                                     size="sm"
                                     placeholder="Оставить заметку для следующей смены"
                                     :rows="1"
                                     :max-rows="12">
                    </b-form-textarea>

                </b-form-group>
                <b-form-group label-for="modalStatus">
                    <template slot="label">
                        <b>Статус</b>
                    </template>
                    <b-form-input size="sm" id="modalStatus" type="text" v-model="modal.status" placeholder="Сдали\приняли смену"/>
                </b-form-group>
                <hr/>
                <b-btn class="float-right" type="submit" variant="primary">Сохранить</b-btn>
            </b-form>
        </b-modal>
    </b-container>
</template>

<script>
    import { USER_GET, USER_POST } from '../../store/actions/user'

    const items = [];

    export default {
        data() {
            return {
                items: [],
                modal: {
                    id: '',
                    name: '',
                    stpinc: '',
                    balance: '',
                    balance2: '',
                    comment: '',
                    status: '',
                },
                balance2collapseVisible: false,
                fields: [
                    { key: 'name', label: 'ФИО', sortable: true, 'thStyle': 'width:160px;', 'class': 'text-nowrap' },
                    { key: 'stpinc', label: 'Инциденты', sortable: true, 'thStyle': 'width:250px;', 'class': 'text-nowrap' },
                    { key: 'balance', label: 'Баланс', sortable: true, 'thStyle': 'width:100px;', 'class': 'text-nowrap' },
                    { key: 'balance2', label: 'Баланс 2', sortable: true, 'thStyle': 'width:100px;', 'class': 'text-nowrap' },

                    { key: 'comment', label: 'Комментарий', sortable: true, 'thStyle': 'width:430px;', 'class': 'text-nowrap' },
                    { key: 'status', label: 'Статус', sortable: true, 'thStyle': 'width:180px;', 'class': 'text-nowrap' },
                    { key: 'date', label: 'Дата', sortable: true, 'thStyle': 'width:130px;', 'class': 'text-nowrap' },
                    { key: 'actions', label: 'Действия', 'class': 'text-center text-nowrap', 'thStyle': 'width:20px;', 'tdClass': 'betterButton' }
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
                allFilteredItems: [],
                dataLoading: false,
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
            async '$route'() { if (this.$route.name === 'dutyid' && this.$route.params.id) await this.tableLoad(); }
        },

        methods: {
            onFiltered(filteredItems) {
                this.allFilteredItems = filteredItems;
                this.totalRows = filteredItems.length;
                // this.currentPage = 1
            },

            timeConstruct(time) {
                if(this.$dt.fromISO(time).isValid) return true
            },

            async load() {
                this.$nextTick(() => this.$refs.searchInput.focus());
                this.currentPage = 1;
                this.dataLoading = true;
                let response = await this.$store.dispatch(USER_GET, '/api/duty/getduty' + this.$route.params.id);
                this.items = response.data;
                this.dataLoading = false;
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
            },

            async deleteDuty(item) {
                let acceptDelete = confirm('Подтверждаете удаление дежурства за ' + item.date + '?');
                if(acceptDelete) {
                    await this.$store.dispatch(USER_POST, {
                        url: '/api/duty/deleteduty',
                        id: item.id
                    })
                        .then(async () => {
                            await this.load();
                        });
                }
            },

            updateModalDuty(item) {
                this.modal.id = item.id;
                this.modal.name = item.name;
                this.modal.stpinc = item.stpinc;
                this.modal.balance = item.balance;
                this.modal.balance2 = item.balance2;
                this.modal.comment = item.comment;
                this.modal.status = item.status;
            },

            async updateDuty() {
                await this.$store.dispatch(USER_POST, {
                    url: '/api/duty/updateduty',
                    id: this.modal.id,
                    stpinc: this.modal.stpinc,
                    balance: this.modal.balance,
                    balance2: this.modal.balance2,
                    comment: this.modal.comment,
                    status: this.modal.status
                })
                    .then(async () => {
                        await this.load();
                    });
                this.$refs.changeDutyRef.hide();
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

</style>
