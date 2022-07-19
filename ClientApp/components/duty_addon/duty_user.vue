<template>
    <div>

        <br/>
        <br/>
        <br/>

        <template v-if="userTable.length !== 0">
            <b-row>
                <b-col cols="8" offset="2" sm="4" offset-sm="4" md="4" lg="4" offset-lg="4" xl="2" offset-xl="1">
                    <b-form v-if="lastUserName" @submit.stop.prevent="createDuty">
                        <h4 class="text-center">{{stateTitleDuty}}</h4>
                        <h6 class="text-center text-muted">{{this.$store.getters.getTokenPlace}}</h6>
                        <br/>

                        <b-form-group label-for="formName">
                            <template slot="label">
                                <b>ФИО</b>
                            </template>
                            <b-form-input size="sm" id="formName" type="text" required :value="this.$store.getters.getTokenName" disabled></b-form-input>
                        </b-form-group>

                        <b-form-group :description="errorFormBalanceMessage" label-for="formBalance">
                            <template slot="label">
                                <b>Баланс</b> <span style="color: red">*</span>
                            </template>
                            <b-input-group size="sm" append="руб." style="cursor: default">
                                <b-form-input :state="errorFormBalanceState" size="sm" id="formBalance" type="text" required :disabled="disableFormBalance" v-model="form.balance"/>
                            </b-input-group>
                        </b-form-group>

                        <b-btn v-if="!balance2collapseVisible" block variant="light" size="sm" class="pt-0 pb-0 mb-3" v-b-toggle.balance2collapse><font-awesome-icon icon="plus" size="sm"/> Баланс второй сим-карты</b-btn>
                        <b-collapse id="balance2collapse" class="mt-2" v-model="balance2collapseVisible">
                            <b-form-group label-for="formBalance2">
                                <template slot="label">
                                    <b>Баланс второй сим-карты</b>
                                </template>
                                <b-input-group size="sm" append="руб." style="cursor: default">
                                    <b-form-input size="sm" id="formBalance2" type="text" :disabled="disableFormBalance2" v-model="form.balance2"/>
                                </b-input-group>
                            </b-form-group>
                        </b-collapse>

                        <b-form-group :description="errorFormStpincMessage" label-for="formStpinc">
                            <template slot="label">
                                <b>Инциденты</b>
                            </template>
                            <b-form-input size="sm" id="formStpinc" type="text" :disabled="disableFormStpinc" v-model="form.stpinc"/>
                        </b-form-group>

                        <b-form-group label-for="formComment" :description="errorFormCommentMessage">
                            <template slot="label">
                                <b>Комментарий</b>
                            </template>
                            <b-form-textarea id="formComment"
                                             v-model="form.comment"
                                             size="sm"
                                             placeholder="Оставить заметку для следующей смены"
                                             :disabled="disableFormComment"
                                             :rows="2"
                                             :max-rows="12">
                            </b-form-textarea>
                        </b-form-group>

                        <b-alert v-if="stateButtonDuty === 'Warning'" show variant="warning" class="text-center">
                            <h6 class="alert-heading font-weight-bold">Внимание</h6>
                            <b>{{ lastUserName }}</b> не закрыл смену!
                        </b-alert>

                        <b-btn v-if="stateButtonDuty === 'Close'" :disabled="disableSubmitButton" type="submit" block variant="primary">Открыть смену</b-btn>
                        <b-btn v-else-if="stateButtonDuty === 'Open'" :disabled="disableSubmitButton" type="submit" block variant="success">Сдать смену</b-btn>
                        <b-btn v-else-if="stateButtonDuty === 'Warning'" :disabled="disableSubmitButton" type="submit" block variant="warning">Сдать чужую смену</b-btn>

                    </b-form>
                </b-col>

                <b-col cols="10" sm="10" md="10" lg="10" xl="7" offset="1">
                    <duty_user_table ref="dutytable" :userTable="userTable"/>
                </b-col>
            </b-row>
        </template>
        <div v-else class="d-flex justify-content-center">
            <template v-if="dataLoad">
                <b-form @submit.stop.prevent="createDuty" style="min-width: 350px">
                    <h4 class="text-center">Открыть первую смену</h4>
                    <h6 class="text-center text-muted">{{this.$store.getters.getTokenPlace}}</h6>
                    <br/>

                    <b-form-group label-for="formName">
                        <template slot="label">
                            <b>ФИО</b>
                        </template>
                        <b-form-input size="sm" id="formName" type="text" required :value="this.$store.getters.getTokenName" disabled></b-form-input>
                    </b-form-group>

                    <b-form-group :description="errorFormBalanceMessage" label-for="formBalance">
                        <template slot="label">
                            <b>Баланс</b> <span style="color: red">*</span>
                        </template>
                        <b-input-group size="sm" append="руб." style="cursor: default">
                            <b-form-input :state="errorFormBalanceState" size="sm" id="formBalance" type="text" required :disabled="disableFormBalance" v-model="form.balance"/>
                        </b-input-group>
                    </b-form-group>

                    <b-btn v-if="!balance2collapseVisible" block variant="light" size="sm" class="pt-0 pb-0 mb-3" v-b-toggle.balance2collapse><font-awesome-icon icon="plus" size="sm"/> Баланс второй сим-карты</b-btn>
                    <b-collapse id="balance2collapse" class="mt-2" v-model="balance2collapseVisible">
                        <b-form-group label-for="formBalance2">
                            <template slot="label">
                                <b>Баланс второй сим-карты</b>
                            </template>
                            <b-input-group size="sm" append="руб." style="cursor: default">
                                <b-form-input size="sm" id="formBalance2" type="text" :disabled="disableFormBalance2" v-model="form.balance2"/>
                            </b-input-group>
                        </b-form-group>
                    </b-collapse>

                    <b-form-group :description="errorFormStpincMessage" label-for="formStpinc">
                        <template slot="label">
                            <b>Инциденты</b>
                        </template>
                        <b-form-input size="sm" id="formStpinc" type="text" :disabled="disableFormStpinc" v-model="form.stpinc"/>
                    </b-form-group>

                    <b-form-group label-for="formComment" :description="errorFormCommentMessage">
                        <template slot="label">
                            <b>Комментарий</b>
                        </template>
                        <b-form-textarea id="formComment"
                                         v-model="form.comment"
                                         size="sm"
                                         placeholder="Оставить заметку для следующей смены"
                                         :disabled="disableFormComment"
                                         :rows="2"
                                         :max-rows="12">
                        </b-form-textarea>
                    </b-form-group>

                    <b-btn type="submit" block variant="primary">Открыть смену</b-btn>
                </b-form>
            </template>
            <template v-else>
                <div class="text-center">
                    <b-spinner style="width: 20rem; height: 20rem" type="grow" />
                </div>
            </template>
        </div>
    </div>
</template>

<script>
    import { USER_GET, USER_POST } from '../../store/actions/user'
    import duty_user_table from './../duty_addon/duty_user_table'

    export default {
        data() {
            return {
                dataLoad: false,
                lastUserName: '',
                lastUserStatus: '',

                balance2collapseVisible: false,

                disableFormBalance: false,
                errorFormBalanceState: null,
                errorFormBalanceMessage: '',

                disableFormBalance2: false,

                disableSubmitButton: false,

                disableFormStpinc: false,
                errorFormStpincState: null,
                errorFormStpincMessage: '',

                disableFormComment: false,
                errorFormCommentState: null,
                errorFormCommentMessage: '',

                form: {
                    balance: '',
                    balance2: '',
                    stpinc: '',
                    comment: '',
                },
                userTable: []
            }
        },
        components: { duty_user_table },
        methods: {
            async createDuty() {
                this.disableSubmitButton = true;
                this.disableFormBalance = true;
                this.disableFormBalance2 = true;
                this.disableFormStpinc = true;
                this.disableFormComment = true;

                await this.$store.dispatch(USER_POST, {
                    url: '/api/duty/createduty',
                    stpinc: this.form.stpinc,
                    balance: this.form.balance,
                    balance2: this.form.balance2,
                    //status: this.tableLastValue.status === 'Принял' ? 'Сдал' : 'Принял',
                    comment: this.form.comment
                })
                .then(async () => {
                    this.form.balance = '';
                    this.form.balance2 = '';
                    this.form.stpinc = '';
                    this.form.comment = '';
                    this.disableSubmitButton = false;
                    this.disableFormBalance = false;
                    this.disableFormBalance2 = false;
                    this.disableFormStpinc = false;
                    this.disableFormComment = false;

                    await this.loadTable();
                })
                .catch((err) => {
                    this.disableSubmitButton = false;
                    this.disableFormBalance = false;
                    this.disableFormBalance2 = false;
                    this.disableFormStpinc = false;
                    this.disableFormComment = false;

                    try{
                        this.errorFormBalanceMessage = Object.values(err.response.data.Balance).toString();
                        this.errorFormStpincMessage = Object.values(err.response.data.Stpinc).toString();
                        this.errorFormCommentMessage = Object.values(err.response.data.Comment).toString();
                    } catch (e) {}

                });
            },

            async loadTable() {
                await this.$store.dispatch(USER_GET, '/api/duty/dutyuser')
                    .then((resp) => {
                        this.userTable = resp.data;
                        if(resp.data.length !== 0) {
                            this.lastUserName = resp.data[0].name;
                            this.lastUserStatus = resp.data[0].status;
                        }
                        this.dataLoad = true;
                });
            }
        },
        computed: {
            stateButtonDuty() {
                if(this.lastUserName) {
                    if (this.lastUserName !== this.$store.getters.getTokenName && this.lastUserStatus.includes('Принял')) return 'Warning';
                    else if (this.lastUserStatus.includes('Принял')) return 'Open';
                    else if (this.lastUserStatus.includes('Сдал')) return 'Close';
                }
            },
            stateTitleDuty() {
                if(this.lastUserName) {
                    if (this.lastUserName !== this.$store.getters.getTokenName && this.lastUserStatus.includes('Принял')) return 'Закрытие чужой смены';
                    else if (this.lastUserStatus.includes('Принял')) return 'Закрытие смены';
                    else if (this.lastUserStatus.includes('Сдал')) return 'Открытие смены';
                }
            }
        },
        async created() {
            await this.loadTable();
        }
    }
</script>

<style scoped>

</style>
