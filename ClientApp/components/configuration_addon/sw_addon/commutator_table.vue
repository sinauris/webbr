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
                        <b-btn v-if="row.item.enable === 1" class="action-button" style="width: 100%" size="sm" @click="changeCommutatorState(row.item)" v-b-tooltip="{title: 'Исключить из опроса', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"><font-awesome-icon icon="power-off" size="lg" style="color: #28a745"/></b-btn>
                        <b-btn v-else class="action-button" style="width: 100%" size="sm" @click="changeCommutatorState(row.item)" v-b-tooltip="{title: 'Добавить в опрос', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"><font-awesome-icon icon="power-off" size="lg" style="color: #9e9e9e"/></b-btn>
                    </template>

                    <template slot="HEAD_id" slot-scope="row">
                        <font-awesome-icon icon="plus-circle" size="lg" style="cursor:pointer;" v-b-modal.createCommutatorModal v-b-tooltip="{title: 'Добавить коммутатор', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"/>
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
                            <b-btn class="action-button" style="width: 100%" size="sm" @click.stop="changeForm(row.item)" v-b-modal.changeCommutatorModal v-b-tooltip="{title: 'Изменить коммутатор', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"><font-awesome-icon icon="pencil-alt"/></b-btn>
                            <b-btn class="action-button" style="width: 100%" size="sm" @click.stop="deleteCommutator(row.item)" v-b-tooltip="{title: 'Удалить коммутатор', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}"><font-awesome-icon icon="trash-alt"/></b-btn>
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
            ref="createModalRef"
            id="createCommutatorModal"
            title="Добавление коммутатора"
            @shown="clearForm"
            hide-footer>
            <b-form @submit.stop.prevent="createSubmit">
                <b-form-group label-for="CreateCommutatorIpAddress">
                    <template slot="label">
                        <b>IP-адрес коммутатора</b>
                    </template>
                    <b-form-input size="sm" id="CreateCommutatorIpAddress" type="text" v-model="form.ip" required placeholder="XXX_XXX_XXX_XXX"/>
                </b-form-group>

                <b-row>
                    <b-col cols="8">
                        <b-form-group label-for="CreateCommutatorPort">
                            <template slot="label">
                                <b>Транковые порты</b> <span v-b-tooltip.hover title="Транковые порты разделяются пробелом."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                            </template>
                            <b-form-input size="sm" id="CreateCommutatorPort" type="text" v-model="form.port" required placeholder="23 24"/>
                        </b-form-group>
                    </b-col>
                    <b-col cols="3" offset="1">
                        <b-form-group label-for="CreateCommutatorOffsetPort">
                            <template slot="label">
                                <b>Смещение</b> <span v-b-tooltip.hover title="Под смещением портов подразумевается отступ от привычной нумерации 1-24 производителем оборудования. Например, коммутаторы Linksys вместо портов 1-24 выдают 49-72."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                            </template>
                            <b-form-input size="sm" id="CreateCommutatorOffsetPort" type="text" v-model="form.port_offset" value="0"/>
                        </b-form-group>
                    </b-col>
                </b-row>
                <b-form-group label-for="CreateCommutatorSnmpPublicString">
                    <template slot="label">
                        <b>SNMP Публичная строка</b> <span v-b-tooltip.hover title="Публичная строка используется для подключения к устройству по протоколу SNMP и получению информации."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                    </template>
                    <b-form-input size="sm" id="CreateCommutatorSnmpPublicString" type="text" v-model="form.snmp_public_string" required placeholder="NCC_snmp"/>
                </b-form-group>
                <b-form-group label-for="CreateCommutatorSnmpOid">
                    <template slot="label">
                        <b>SNMP OID</b> <span v-b-tooltip.hover title="OID используется для точного указание места, где будет произведён поиск требуемых характеристик устройства."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                    </template>
                    <b-form-input size="sm" id="CreateCommutatorSnmpOid" type="text" v-model="form.snmp_oid" required placeholder="1.3.6.1.2.1.17.7.1.2.2.1.2"/>
                </b-form-group>
                <b-form-group label-for="CreateCommutatorComment">
                    <template slot="label">
                        <b>Комментарий</b>
                    </template>
                    <b-form-input size="sm" id="CreateCommutatorComment" type="text" v-model="form.comment" required placeholder="Коммутатор № , стойка № , серверная №"/>
                </b-form-group>
                <hr/>
                <b-btn class="float-right" type="submit" variant="success">Добавить</b-btn>
            </b-form>
        </b-modal>

        <b-modal
            ref="changeModalRef"
            id="changeCommutatorModal"
            title="Изменение коммутатора"
            hide-footer>
            <b-form @submit.stop.prevent="changeSubmit">
                <b-form-group label-for="ChangeCommutatorIpAddress">
                    <template slot="label">
                        <b>IP-адрес коммутатора</b>
                    </template>
                    <b-form-input size="sm" id="ChangeCommutatorIpAddress" type="text" v-model="form.ip" required placeholder="XXX_XXX_XXX_XXX"/>
                </b-form-group>

                <b-row>
                    <b-col cols="8">
                        <b-form-group label-for="ChangeCommutatorPort">
                            <template slot="label">
                                <b>Транковые порты</b> <span v-b-tooltip.hover title="Транковые порты разделяются пробелом."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                            </template>
                            <b-form-input size="sm" id="ChangeCommutatorPort" type="text" v-model="form.port" required placeholder="23 24"/>
                        </b-form-group>
                    </b-col>
                    <b-col cols="3" offset="1">
                        <b-form-group label-for="ChangeCommutatorOffsetPort">
                            <template slot="label">
                                <b>Смещение</b> <span v-b-tooltip.hover title="Под смещением портов подразумевается отступ от привычной нумерации 1-24 производителем оборудования. Например, коммутаторы Linksys вместо портов 1-24 выдают 49-72."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                            </template>
                            <b-form-input size="sm" id="ChangeCommutatorOffsetPort" type="text" v-model="form.port_offset"/>
                        </b-form-group>
                    </b-col>
                </b-row>

                <b-form-group label-for="ChangeCommutatorSnmpPublicString">
                    <template slot="label">
                        <b>SNMP Публичная строка</b> <span v-b-tooltip.hover title="Публичная строка используется для подключения к устройству по протоколу SNMP и получению информации."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                    </template>
                    <b-form-input size="sm" id="ChangeCommutatorSnmpPublicString" type="text" v-model="form.snmp_public_string" required placeholder="NCC_snmp"/>
                </b-form-group>
                <b-form-group label-for="ChangeCommutatorSnmpOid">
                    <template slot="label">
                        <b>SNMP OID</b> <span v-b-tooltip.hover title="OID используется для точного указание места, где будет произведён поиск требуемых характеристик устройства."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help;"/></span>
                    </template>
                    <b-form-input size="sm" id="ChangeCommutatorSnmpOid" type="text" v-model="form.snmp_oid" required placeholder="1.3.6.1.2.1.17.7.1.2.2.1.2"/>
                </b-form-group>
                <b-form-group label-for="ChangeCommutatorComment">
                    <template slot="label">
                        <b>Комментарий</b>
                    </template>
                    <b-form-input size="sm" id="ChangeCommutatorComment" type="text" v-model="form.comment" required placeholder="Коммутатор № , стойка № , серверная №"/>
                </b-form-group>
                <hr/>
                <b-btn class="float-right" type="submit" variant="primary">Сохранить</b-btn>
            </b-form>
        </b-modal>

    </b-container>
</template>

<script>
    import { mapGetters } from 'vuex'
    import { USER_GET, USER_POST } from '../../../store/actions/user'

    export default {
        data() {
            return {
                form: {
                    id: '',
                    ip: '',
                    port: '',
                    port_offset: '',
                    comment: '',
                    snmp_public_string: '',
                    snmp_oid: '',
                    enable: ''
                },
                items: [],
                fields: [
                    { key: 'id', 'class': 'text-center text-nowrap', 'thStyle': 'width:60px;', 'tdClass': 'betterButton' },
                    { key: 'ip', label: 'IP', sortable: true, 'thStyle': 'width:150px;', 'class': 'text-nowrap' },
                    { key: 'port', label: 'Порты', sortable: true, 'thStyle': 'width:80px;', 'class': 'text-nowrap' },
                    { key: 'port_offset', label: 'Смещение', sortable: true, 'thStyle': 'width:80px;', 'class': 'text-nowrap' },
                    { key: 'snmp_public_string', label: 'SNMP Строка', sortable: true, 'thStyle': 'width:150px;', 'class': 'text-nowrap' },
                    { key: 'snmp_oid', label: 'SNMP OID', sortable: true, 'thStyle': 'width:200px;', 'class': 'text-nowrap' },
                    { key: 'comment', label: 'Комментарий', sortable: true, 'thStyle': 'width:525px;', 'class': 'text-nowrap' },
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
        watch: {
            async '$route'() { if (this.$route.name === 'commutatorid' && this.$route.params.id) await this.tableLoad(); }
        },
        methods: {
            onFiltered(filteredItems) {
                this.totalRows = filteredItems.length;
                // this.currentPage = 1
            },

            clearForm() {
                this.form.id = '';
                this.form.ip = '';
                this.form.port = '';
                this.form.port_offset = '';
                this.form.snmp_public_string = '';
                this.form.snmp_oid = '';
                this.form.comment = '';
            },

            changeForm(item) {
                this.form.id = item.id;
                this.form.ip = item.ip;
                this.form.port = item.port;
                this.form.port_offset = item.port_offset;
                this.form.snmp_public_string = item.snmp_public_string;
                this.form.snmp_oid = item.snmp_oid;
                this.form.comment = item.comment;
            },

            async createSubmit() {
                await this.$store.dispatch(USER_POST, {
                    url: '/api/configuration/createcommutator',
                    ip: this.form.ip,
                    port: this.form.port,
                    port_offset: this.form.port_offset,
                    snmp_public_string: this.form.snmp_public_string,
                    snmp_oid: this.form.snmp_oid,
                    comment: this.form.comment,
                    placeid: this.placeId
                })
                .then(async () => {
                    this.$refs.createModalRef.hide();
                    await this.tableLoad();
                });
                this.$refs.createModalRef.hide();
            },

            async changeSubmit() {
                await this.$store.dispatch(USER_POST, {
                    url: '/api/configuration/updatecommutator',
                    id: this.form.id,
                    ip: this.form.ip,
                    port: this.form.port,
                    port_offset: this.form.port_offset,
                    snmp_public_string: this.form.snmp_public_string,
                    snmp_oid: this.form.snmp_oid,
                    comment: this.form.comment,
                    placeid: this.placeId
                })
                .then(async () => {
                    this.$refs.changeModalRef.hide();
                    await this.tableLoad();
                });
                this.$refs.changeModalRef.hide();
            },

            async deleteCommutator(item) {
                let acceptDelete = confirm('Подтверждаете удаление коммутатора ' + item.ip + '?');
                if(acceptDelete){
                    await this.$store.dispatch(USER_POST, {
                        url: '/api/configuration/deletecommutator',
                        id: item.id
                    })
                        .then(async () => {
                            await this.tableLoad();
                        });
                }
            },

            async changeCommutatorState(i) {
                await this.$store.dispatch(USER_POST, { url: '/api/configuration/switchstatecommutator', id: i.id, enable: i.enable })
                    .then(async () => {
                        await this.tableLoad();
                    });
            },

            async tableLoad() {
                this.$nextTick(() => this.$refs.searchInput.focus());
                this.currentPage = 1;
                this.dataLoading = true;
                let response = await this.$store.dispatch(USER_GET, '/api/configuration/getswitch' + this.$route.params.id);
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
            },
            
            placeId() {
                let place_short_description = this.$route.params.id;

                let result = this.getPlace.filter(obj => {
                    return obj.place_short_description === place_short_description
                });

                return result[0].placeid;
            }
        },
        async created() {
            await this.tableLoad();
        }
    }
</script>

<style scoped>

</style>