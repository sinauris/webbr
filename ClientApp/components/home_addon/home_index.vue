<template>
    <div v-if="records != null" class="addLink">
        <div v-for="i in records.sections" :key="i.id">
            <div>
                <div class="accordionBtn d-flex">
<!--                    <b-btn block size="sm" class="text-left expand-button" v-b-toggle="'accordion-'+i.id">
                        <h5 class="mb-1">{{i.section_name}}</h5>
                    </b-btn>-->
                    <h5 class="mb-1" style="cursor: pointer" v-b-toggle="'accordion-'+i.id">{{i.section_name}}</h5>
                    <div class="accordionBtnHoverEdit ml-2" v-if="isRgPolicy">
                        <font-awesome-icon icon="plus" v-b-tooltip="{title: 'Добавить ссылку', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" @click="createLink(i.id)" style="cursor: hand"/>
                        <font-awesome-icon icon="pencil-alt" size="sm" v-b-tooltip="{title: 'Изменить раздел', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" @click="editColumn(i.id, i.section_name)" style="cursor: hand; margin-left: 10px"/>
                        <font-awesome-icon icon="trash-alt" size="sm" v-b-tooltip="{title: 'Удалить раздел', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" @click="deleteColumn(i.id, i.section_name)" style="cursor: hand; margin-left: 10px"/>
                    </div>
                </div>
                <b-collapse :visible="i.id == 1" :id="'accordion-'+i.id" accordion="linksAccordion">
                    <ul>
                        <li v-for="j in records.links" :key="j.id" v-if="i.id == j.column">
                            <a :href="j.link" target="_blank"> {{j.description}}</a>
                            <div class="hoverEdit ml-2" v-if="isRgPolicy">
                                <font-awesome-icon icon="pencil-alt" size="sm" style="cursor: hand" v-b-tooltip="{title: 'Изменить ссылку', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" @click="updateLink(j.id, j.link, j.description, j.column)"/>
                                <font-awesome-icon icon="trash-alt" size="sm" style="cursor: hand; margin-left: 10px" v-b-tooltip="{title: 'Удалить ссылку', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" @click="deleteLink(j.id, j.description)"/>
                            </div>
                        </li>
                    </ul>
                </b-collapse>
            </div>
        </div>
        <div class="addLinkBtnHover mt-2" v-if="isRgPolicy">
            <b-btn block variant="success" size="sm" @click="createColumn()"><font-awesome-icon icon="plus" size="sm" style="cursor: hand"/> Добавить раздел</b-btn>
        </div>

        <div v-if="isRgPolicy">
            <b-modal
                title="Создание ссылки"
                ref="linkCreateModal"
                @show="linkCreateModalShowClear"
                hide-footer>
                <b-form @submit.stop.prevent="submitCreateLink">
                    <b-form-group label-for="LinkDescription">
                        <template slot="label">
                            <b>Описание</b>
                        </template>
                        <b-form-input size="sm" id="LinkDescription" type="text" v-model="linkCreate.description" required/>
                    </b-form-group>
                    <b-form-group label-for="LinkHttp">
                        <template slot="label">
                            <b>Ссылка</b>
                        </template>
                        <b-form-input size="sm" id="LinkHttp" type="text" v-model="linkCreate.link" required/>
                    </b-form-group>
                    <b-form-group>
                        <template slot="label">
                            <b>Раздел</b>
                        </template>
                        <b-form-select
                            size="sm"
                            v-model="linkCreate.column"
                            :options="recordColumn"
                            required
                        />
                    </b-form-group>
                    <hr/>
                    <b-btn v-if="!linkCreate.linkCreateModalButtonLoading" class="float-right" type="submit" variant="success">Создать</b-btn>
                    <b-btn v-else class="float-right" type="submit" variant="success" disabled><font-awesome-icon icon="cog" size="sm" spin/> Создание</b-btn>
                </b-form>
            </b-modal>

            <b-modal
                title="Изменение ссылки"
                ref="linkUpdateModal"
                @show="linkEditModalShowClear"
                hide-footer>
                <b-form @submit.stop.prevent="submitUpdateLink">
                    <b-form-group label-for="LinkDescription">
                        <template slot="label">
                            <b>Описание</b>
                        </template>
                        <b-form-input size="sm" id="LinkDescription" type="text" v-model="linkEdit.description" required/>
                    </b-form-group>
                    <b-form-group label-for="LinkHttp">
                        <template slot="label">
                            <b>Ссылка</b>
                        </template>
                        <b-form-input size="sm" id="LinkHttp" type="text" v-model="linkEdit.link" required/>
                    </b-form-group>
                    <b-form-group>
                        <template slot="label">
                            <b>Раздел</b>
                        </template>
                        <b-form-select
                            size="sm"
                            v-model="linkEdit.column"
                            :options="recordColumn"
                            required
                        />
                    </b-form-group>
                    <hr/>
                    <b-btn v-if="!linkEdit.linkEditModalButtonLoading" class="float-right" type="submit" variant="success">Изменить</b-btn>
                    <b-btn v-else class="float-right" type="submit" variant="success" disabled><font-awesome-icon icon="cog" size="sm" spin/> Изменение</b-btn>
                </b-form>
            </b-modal>

            <b-modal
                title="Создание раздела"
                ref="columnCreateModal"
                @show="columnCreateModalShowClear"
                hide-footer>
                <b-form @submit.stop.prevent="submitCreateColumn">
                    <b-form-group label-for="ColumnName">
                        <template slot="label">
                            <b>Название раздела</b>
                        </template>
                        <b-form-input size="sm" id="ColumnName" type="text" v-model="columnCreate.name" required/>
                    </b-form-group>
                    <hr/>
                    <b-btn v-if="!columnCreate.columnCreateModalButtonLoading" class="float-right" type="submit" variant="success">Создать</b-btn>
                    <b-btn v-else class="float-right" type="submit" variant="success" disabled><font-awesome-icon icon="cog" size="sm" spin/> Создание</b-btn>
                </b-form>
            </b-modal>

            <b-modal
                title="Переименование раздела"
                ref="columnEditModal"
                @show="columnEditModalShowClear"
                hide-footer>
                <b-form @submit.stop.prevent="submitEditColumn">
                    <b-form-group label-for="ColumnName">
                        <template slot="label">
                            <b>Название раздела</b>
                        </template>
                        <b-form-input size="sm" id="ColumnName" type="text" v-model="columnEdit.name" required/>
                    </b-form-group>
                    <hr/>
                    <b-btn v-if="!columnEdit.columnEditModalButtonLoading" class="float-right" type="submit" variant="success">Изменить</b-btn>
                    <b-btn v-else class="float-right" type="submit" variant="success" disabled><font-awesome-icon icon="cog" size="sm" spin/> Изменение</b-btn>
                </b-form>
            </b-modal>
        </div>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex'
    import { USER_GET, USER_POST } from '../../store/actions/user'

    export default {
        data() {
            return {
                records: null,

                columnCreate: {
                    name: '',
                    columnCreateModalButtonLoading: false
                },
                columnEdit: {
                    id: '',
                    name: '',
                    columnEditModalButtonLoading: false
                },

                linkCreate: {
                    description: '',
                    link: '',
                    column: '',

                    linkCreateModalButtonLoading: false
                },
                linkEdit: {
                    id: '',
                    description: '',
                    link: '',
                    column: '',

                    linkEditModalButtonLoading: false
                }
            }
        },

        methods: {
            columnCreateModalShowClear() {
                this.columnCreate.name = '';
                this.columnCreate.columnCreateModalButtonLoading = false;
            },
            columnEditModalShowClear() {
                this.columnEdit.columnCreateModalButtonLoading = false;
            },
            linkCreateModalShowClear() {
                this.linkCreate.description = '';
                this.linkCreate.link = '';
                this.linkCreate.column = '';
                this.linkCreate.linkCreateModalButtonLoading = false;
            },
            linkEditModalShowClear() {
                this.linkEdit.id = '';
                this.linkEdit.description = '';
                this.linkEdit.link = '';
                this.linkEdit.column = '';
                this.linkEdit.linkEditModalButtonLoading = false;
            },

            createColumn() {
                this.$refs.columnCreateModal.show();
            },
            async submitCreateColumn() {
                this.columnCreate.columnCreateModalButtonLoading = true;
                await this.$store.dispatch(USER_POST, { url: '/api/index/createcolumn', section_name: this.columnCreate.name }).then(async () => await this.refreshLinks());
                this.columnCreate.columnCreateModalButtonLoading = false;
                this.$refs.columnCreateModal.hide()
            },

            editColumn(id, name) {
                this.columnEdit.id = id;
                this.columnEdit.name = name;
                this.$refs.columnEditModal.show();
            },
            async submitEditColumn() {
                this.columnEdit.columnEditModalButtonLoading = true;
                await this.$store.dispatch(USER_POST, { url: '/api/index/updatecolumn', id: this.columnEdit.id, section_name: this.columnEdit.name }).then(async () => await this.refreshLinks());
                this.columnEdit.columnEditModalButtonLoading = false;
                this.$refs.columnEditModal.hide()
            },

            async deleteColumn(id, name) {
                let acceptDelete = confirm('Подтверждаете удаление раздела ' + name + '?');
                if(acceptDelete) await this.$store.dispatch(USER_POST, { url: '/api/index/deletecolumn', id: id }).then(async () => await this.refreshLinks())
            },

            createLink(column) {
                this.$refs.linkCreateModal.show();
                this.linkCreate.column = column;
            },
            async submitCreateLink() {
                this.linkCreate.linkCreateModalButtonLoading = true;
                await this.$store.dispatch(USER_POST, {
                    url: '/api/index/createlink',
                    link: this.linkCreate.link,
                    description: this.linkCreate.description,
                    column: this.linkCreate.column
                })
                    .then(async () => await this.refreshLinks());
                this.linkCreate.linkCreateModalButtonLoading = false;
                this.$refs.linkCreateModal.hide()
            },
            updateLink(id, link, description, column) {
                this.$refs.linkUpdateModal.show();

                this.linkEdit.id = id;
                this.linkEdit.description = description;
                this.linkEdit.link = link;
                this.linkEdit.column = column;
            },
            async submitUpdateLink() {
                this.linkEdit.linkEditModalButtonLoading = true;
                await this.$store.dispatch(USER_POST, {
                    url: '/api/index/updatelink',
                    id: this.linkEdit.id,
                    link: this.linkEdit.link,
                    description: this.linkEdit.description,
                    column: this.linkEdit.column
                })
                    .then(async () => await this.refreshLinks());

                this.linkEdit.linkEditModalButtonLoading = false;
                this.$refs.linkUpdateModal.hide()
            },


            async deleteLink(id, description) {
                let acceptDelete = confirm('Подтверждаете удаление ссылки на ' + description + '?');
                if(acceptDelete) await this.$store.dispatch(USER_POST, { url: '/api/index/deletelink', id: id}).then(async () => await this.refreshLinks())
            },


            async refreshLinks() {
                await this.$store.dispatch(USER_GET, '/api/index/getlinks').then((resp) => this.records = resp.data);
            }
        },

        computed: {
            ...mapGetters(['isRgPolicy']),
            recordColumn() {
                let arr = [];
                this.records.sections.forEach(i => { arr.push({ value: i.id, text: i.section_name }); });
                return arr;
            }
        },

        mounted () {
            this.$signalR.on('linksUpdate', () => this.refreshLinks());
        },

        async created() {
            await this.refreshLinks();
        }
    }
</script>

<style>
    .addLinkBtnHover {
        display: none;
        align-items: center;
        position: relative;
    }
    .addLink:hover > .addLinkBtnHover {
        display: block;
    }

    .accordionBtnHoverEdit {
        display: none;
        align-items: center;
        position: relative;
    }
    .accordionBtn:hover > .accordionBtnHoverEdit {
        display: inline-flex;
    }

    .hoverEdit {
        display: none;
        align-items: center;
        position: relative;
    }

    li:hover > .hoverEdit {
        display: inline-flex;
    }
</style>
