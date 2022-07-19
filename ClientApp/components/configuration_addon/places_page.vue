<template>
    <b-container fluid>
        <br/>
        <b-row>
            <b-col cols="11" sm="11" md="11" lg="11" xl="10" offset="1">
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

                    <template slot="id" slot-scope="row">
                        <b-btn v-if="row.item.place_enabled === 1" class="action-button" style="width: 100%" size="sm" @click="changePlaceState(row.item)" v-b-tooltip="{title: 'Исключить из опроса', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"><font-awesome-icon icon="power-off" size="lg" style="color: #28a745"/></b-btn>
                        <b-btn v-else class="action-button" style="width: 100%" size="sm" @click="changePlaceState(row.item)" v-b-tooltip="{title: 'Добавить в опрос', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"><font-awesome-icon icon="power-off" size="lg" style="color: #9e9e9e"/></b-btn>
                    </template>

                    <template slot="HEAD_id" slot-scope="row">
                        <font-awesome-icon icon="plus-circle" size="lg" style="cursor:pointer;" v-b-modal.createPlaceModal v-b-tooltip="{title: 'Добавить площадку', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"/>
                    </template>

                    <template slot="ip" slot-scope="row">
                        <a :href="'http://'+row.value+''" target="_blank">{{ row.value }}</a>
                    </template>
                    <template slot="port" slot-scope="row">
                        {{row.value}}
                    </template>
                    <template slot="comment" slot-scope="row">
                        {{ row.value }}
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
                                @click="tableLoad"
                                icon="retweet"
                                size="lg"
                                style="cursor:pointer;"
                                v-b-tooltip="{title: 'Обновить данные', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"/>
                    </template>

                    <template slot="actions" slot-scope="row">
                        <b-btn-group style="width: 100%">
                            <b-btn class="action-button" style="width: 100%" size="sm" @click.stop="changeForm(row.item)" v-b-modal.changePlaceModal v-b-tooltip="{title: 'Изменить площадку', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"><font-awesome-icon icon="pencil-alt"/></b-btn>
                            <b-btn class="action-button" style="width: 100%" size="sm" @click.stop="deletePlace(row.item)" v-b-tooltip="{title: 'Удалить площадку', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"><font-awesome-icon icon="trash-alt"/></b-btn>
                        </b-btn-group>
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

        <b-modal
                ref="createPlaceModalRef"
                id="createPlaceModal"
                title="Добавление площадки"
                @shown="clearForm"
                hide-footer>
            <b-form @submit.stop.prevent="createSubmit">
                <b-alert show variant="warning">
                    <p class="m-0" style="font-size: 13px">
                        В ходе опроса подключение к серверам происходит по SSH, с авторизацией по ключу <b v-b-toggle.sshkey style="cursor:pointer; font-size: 18px; color: #007bff">Technical Key</b>.
                        В случае проблем проверьте, добавлен ли пользователю <b>root</b> данный ключ, а так же разрешена ли под ним авторизация. 
                    </p>
                    <b-collapse id="sshkey">
                        <h4 class="alert-heading">Как добавить ключ</h4>
                        <ul>
                            <li>Зайти по SSH на нужный сервер</li>
                            <li>Проследовать в папку <kbd>$HOME/.ssh/</kbd> </li>
                            <li>Открыть файл <kbd>authorized_keys</kbd></li>
                            <li>Добавить ключ ниже</li>
                        </ul>
                        <code style="font-size: 11px;">ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDwBcxgnxN9vJglXGrNn03TAyMOiVwO3abHEraFe8C5/siDnRPe38sAcD1zC16L2q/KI4PZjHofLyA3kvURN3XSXilUyMHZboPCySfV2lN+B3GbJ/kt5mDp4f3nLW80zC6Gmx7nX2uluiIEBEXYmO3nqOYPX8J2Gf7QQRKOqoKh7/ZGBts5NU7MBHaAmgtpQJcGm7P1JD1Ho9PtMvnTnbJTumg4J6VcHyXo8dn2R4YI0UTmw/gPmDdT50OjBX6sYYtZXxB0gRSwE9JcP7z2OQuXv09V8darxZ8thQ+xLHdqgMssLKtgbLX2kKLOixSYmQlTlUZwNRI/4f0sJ13c+CTddVNQGmLEoEQ4ca29IoKl8kMHcnsCCsYpgXAknjH1d3uG3ygv9i7AP47Jvk/qKHQ2Jyi1/ALff7S+uGmOen/nbgNcgT8PgKBiyLe8mTxDgydtPLyzd3h1W15OHRd6bChaefMc+sgIus4w2oONvby5FpPuTOvQ+HifyV/AkYshW4WPTQwSa1yr9WvkgLSZT3BQj1USUeNJzGdz+INAW7UgSwlrKlM7lkQmsTKnCgUDKgP2O+mocmb/R/vYtvHaTBa+pAP5BrMunBm9+lotoiwTki0E4BQcj7tTqc/vyeXfzmM7BQiHvR0+TnBHTy2Z8NToAaH8ZpqhTjFtDFpkXEv97Q== technicalkey</code>
                    </b-collapse>
                </b-alert>
                <b-row>
                    <b-col cols="5">
                        <b-form-group label-for="CreatePlaceDescription">
                            <template slot="label">
                                <b>Название площадки</b>
                            </template>
                            <b-form-input size="sm" id="CreatePlaceDescription" type="text" v-model="form.place_description" required placeholder="Киров"/>
                        </b-form-group>
                    </b-col>
                    <b-col cols="6" offset="1">
                        <b-form-group label-for="CreatePlaceShortDescription">
                            <template slot="label">
                                <b>Аббревиатура</b> <span v-b-tooltip title="Сокращённое название площадки на английском языке."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                            </template>
                            <b-form-input size="sm" id="CreatePlaceShortDescription" type="text" v-model="form.place_short_description" required placeholder="krv"/>
                        </b-form-group>
                    </b-col>
                </b-row>
                <b-row>
                    <b-col cols="5">
                        <b-form-group label-for="CreatePlaceDhcpServer">
                            <template slot="label">
                                <b>Сервер DHCP</b> <span v-b-tooltip title="Сервер DHCP данной площадки. Указывает, где выполнять команду для последующего сбора информации."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                            </template>
                            <b-form-input size="sm" id="CreatePlaceDhcpServer" type="text" v-model="form.dhcp_server" placeholder="XXX_XXX_XXX_XXX"/>
                        </b-form-group>
                    </b-col>
                    <b-col cols="6" offset="1">
                        <b-form-group label-for="CreatePlaceDhcpServerCommand">
                            <template slot="label">
                                <b>Команда сервера DHCP</b> <span v-b-tooltip title="Команда для сбора информации о выданных в аренду адресах на DHCP сервере. При опросе площадки идёт парсинг данного вывода."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                            </template>
                            <b-form-input size="sm" id="CreatePlaceDhcpServerCommand" type="text" v-model="form.dhcp_server_command" placeholder="cat /var/lib/dhcp/dhcpd.leases"/>
                        </b-form-group>
                    </b-col>
                </b-row>
                <b-form-group label-for="CreatePlaceNaumenServer">
                    <template slot="label">
                        <b>Сервер Naumen</b> <span v-b-tooltip title="Сервер локальной шины Naumen данной площадки. При опросе идёт сбор информации с сервера и последующий парсинг."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                    </template>
                    <b-form-input size="sm" id="CreatePlaceNaumenServer" type="text" v-model="form.naumen_server" placeholder="XXX_XXX_XXX_XXX"/>
                </b-form-group>
                <b-form-group label-for="CreatePlaceComment">
                    <template slot="label">
                        <b>Комментарий</b>
                    </template>
                    <b-form-input size="sm" id="CreatePlaceComment" type="text" v-model="form.comment" placeholder="Описание площадки и других нюансов"/>
                </b-form-group>
                <hr/>
                <b-btn class="float-right" type="submit" variant="success">Добавить</b-btn>
            </b-form>
        </b-modal>

        <b-modal
                ref="changePlaceModalRef"
                id="changePlaceModal"
                title="Изменение площадки"
                hide-footer>
            <b-form @submit.stop.prevent="changeSubmit">
                <b-row>
                    <b-col cols="5">
                        <b-form-group label-for="ChangePlaceDescription">
                            <template slot="label">
                                <b>Название площадки</b>
                            </template>
                            <b-form-input size="sm" id="ChangePlaceDescription" type="text" v-model="form.place_description" required placeholder="Киров"/>
                        </b-form-group>
                    </b-col>
                    <b-col cols="6" offset="1">
                        <b-form-group label-for="ChangePlaceShortDescription">
                            <template slot="label">
                                <b>Аббревиатура</b> <span v-b-tooltip title="Сокращённое название площадки на английском языке."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                            </template>
                            <b-form-input size="sm" id="ChangePlaceShortDescription" type="text" v-model="form.place_short_description" required placeholder="krv"/>
                        </b-form-group>
                    </b-col>
                </b-row>
                <b-row>
                    <b-col cols="5">
                        <b-form-group label-for="ChangePlaceDhcpServer">
                            <template slot="label">
                                <b>Сервер DHCP</b> <span v-b-tooltip title="Сервер DHCP данной площадки. Указывает, где выполнять команду для последующего сбора информации."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                            </template>
                            <b-form-input size="sm" id="ChangePlaceDhcpServer" type="text" v-model="form.dhcp_server" placeholder="XXX_XXX_XXX_XXX"/>
                        </b-form-group>
                    </b-col>
                    <b-col cols="6" offset="1">
                        <b-form-group label-for="ChangePlaceDhcpServerCommand">
                            <template slot="label">
                                <b>Команда сервера DHCP</b> <span v-b-tooltip title="Команда для сбора информации о выданных в аренду адресах на DHCP сервере. При опросе площадки идёт парсинг данного вывода."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                            </template>
                            <b-form-input size="sm" id="ChangePlaceDhcpServerCommand" type="text" v-model="form.dhcp_server_command" placeholder="cat /var/lib/dhcp/dhcpd.leases"/>
                        </b-form-group>
                    </b-col>
                </b-row>
                <b-form-group label-for="ChangePlaceNaumenServer">
                    <template slot="label">
                        <b>Сервер Naumen</b> <span v-b-tooltip title="Сервер локальной шины Naumen данной площадки. При опросе идёт сбор информации с сервера и последующий парсинг."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                    </template>
                    <b-form-input size="sm" id="ChangePlaceNaumenServer" type="text" v-model="form.naumen_server" placeholder="XXX_XXX_XXX_XXX"/>
                </b-form-group>
                <b-form-group label-for="ChangePlaceComment">
                    <template slot="label">
                        <b>Комментарий</b>
                    </template>
                    <b-form-input size="sm" id="ChangePlaceComment" type="text" v-model="form.comment" placeholder="Описание площадки и других нюансов"/>
                </b-form-group>
                <hr/>
                <b-btn class="float-right" type="submit" variant="success">Сохранить</b-btn>
            </b-form>
        </b-modal>
    </b-container>
</template>

<script>
    import { mapGetters } from 'vuex'
    import { USER_GET, USER_POST } from '../../store/actions/user'

    export default {
        data() {
            return {
                items: [],
                form: {
                    place_description: '',
                    place_short_description: '',
                    dhcp_server: '',
                    dhcp_server_command: '',
                    naumen_server: '',
                    comment: '',
                    placeid: ''
                },
                fields: [
                    { key: 'id', 'class': 'text-center text-nowrap', 'thStyle': 'width:60px;', 'tdClass': 'betterButton' },
                    { key: 'place_description', label: 'Название', sortable: true, 'class': 'text-nowrap', 'thStyle': 'width:80px;' },
                    { key: 'place_short_description', label: 'Аббревиатура', sortable: true, 'class': 'text-nowrap', 'thStyle': 'width:150px;' },
                    { key: 'dhcp_server', label: 'Сервер DHCP', sortable: true, 'class': 'text-nowrap', 'thStyle': 'width:80px;' },
                    { key: 'dhcp_server_command', label: 'Команда сервера DHCP', sortable: true, 'class': 'text-nowrap', 'thStyle': 'width:150px;' },
                    { key: 'naumen_server', label: 'Сервер Naumen', sortable: true, 'class': 'text-nowrap', 'thStyle': 'width:200px;' },
                    { key: 'comment', label: 'Примечание', sortable: true, 'class': 'text-nowrap', 'thStyle': 'width:525px;' },
                    { key: 'actions', 'class': 'text-center text-nowrap', 'thStyle': 'width:60px;', 'tdClass': 'betterButton' }
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

            clearForm() {
                this.form.place_description = '';
                this.form.place_short_description = '';
                this.form.dhcp_server = '';
                this.form.dhcp_server_command = '';
                this.form.naumen_server = '';
                this.form.comment = '';
                this.form.placeid = '';
            },

            changeForm(item) {
                console.log(item);
                this.form.place_description = item.place_description;
                this.form.place_short_description = item.place_short_description;
                this.form.dhcp_server = item.dhcp_server;
                this.form.dhcp_server_command = item.dhcp_server_command;
                this.form.naumen_server = item.naumen_server;
                this.form.comment = item.comment;
                this.form.placeid = item.placeid;
            },

            async createSubmit() {
                await this.$store.dispatch(USER_POST, {
                    url: '/api/configuration/createplace',
                    place_description: this.form.place_description,
                    place_short_description: this.form.place_short_description,
                    dhcp_server: this.form.dhcp_server,
                    dhcp_server_command: this.form.dhcp_server_command,
                    naumen_server: this.form.naumen_server,
                    comment: this.form.comment
                })
                    .then(async () => {
                        document.location.reload();
                        // this.$refs.createPlaceModalRef.hide();
                        // await this.tableLoad();
                    });
                this.$refs.createPlaceModalRef.hide();
            },

            async changeSubmit() {
                await this.$store.dispatch(USER_POST, {
                    url: '/api/configuration/updateplace',
                    place_description: this.form.place_description,
                    place_short_description: this.form.place_short_description,
                    dhcp_server: this.form.dhcp_server,
                    dhcp_server_command: this.form.dhcp_server_command,
                    naumen_server: this.form.naumen_server,
                    comment: this.form.comment,
                    placeid: this.form.placeid
                })
                    .then(async () => {
                        document.location.reload();
                        // this.$refs.changePlaceModalRef.hide();
                        // await this.tableLoad();
                    });
                this.$refs.changePlaceModalRef.hide();
            },

            async deletePlace(item) {
                let acceptDelete = confirm('Подтверждаете удаление площадки ' + item.place_description + '?');
                if(acceptDelete){
                    await this.$store.dispatch(USER_POST, {
                        url: '/api/configuration/deleteplace',
                        placeid: item.placeid
                    })
                        .then(async () => {
                            document.location.reload();
                            // await this.tableLoad();
                        });
                }
            },

            async changePlaceState(i) {
                await this.$store.dispatch(USER_POST, { url: '/api/configuration/switchstateplace', placeid: i.placeid, place_enabled: i.place_enabled })
                    .then(async () => {
                        document.location.reload();
                    });
            },
            
            async tableLoad() {
                this.$nextTick(() => this.$refs.searchInput.focus());
                this.currentPage = 1;
                this.dataLoading = true;
                let response = await this.$store.dispatch(USER_GET, '/api/configuration/getplaces');
                this.items = response.data;
                this.dataLoading = false;
            }
        },
        computed: {
            ...mapGetters(['getPlace']),

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