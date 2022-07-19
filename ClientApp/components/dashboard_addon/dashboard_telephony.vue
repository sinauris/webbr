<!--
<template>
    <b-card no-body header-class="card-header-progress" class="btnHidden">

        <div slot="header" class="text-center font-weight-bold">
            <div class="ml-2 btnHover float-left">
                <span class="mr-2" id="telephonyPopover" style="cursor: help"><font-awesome-icon :icon="['far', 'question-circle']"/></span>
            </div>

            <b-popover target="telephonyPopover" placement="left" triggers="hover">
                <template slot="title">
                    <span style="font-size: 14px">Телефония</span>
                    <p class="p-0 m-0 text-muted" style="font-size: 12px">Описание виджета</p>

                    <hr class="mt-3 mb-2 p-0"/>
                    <div style="font-size: 12px"><font-awesome-icon :icon="['far', 'clock']"/> Время обновления: 2 минуты</div>
                </template>
                <p class="m-0" style="font-size: 12px">Виджет отображает проектно-ориентированное состояние телефонии с задержкой в 5-10 минут.</p>
            </b-popover>

            <div class="float-right" style="margin-right: 20px">
                <div class="btnHover" style="cursor: pointer" v-b-toggle="'telephonyCollapse'">
                    <font-awesome-icon class="when-opened" icon="chevron-up"/>
                    <font-awesome-icon class="when-closed" icon="chevron-down"/>
                </div>
            </div>

            <div>
                <span style="cursor: default;"><font-awesome-icon icon="microphone"/> Телефония</span>
            </div>
        </div>

        <b-collapse id="telephonyCollapse" visible>
            <div v-if="exception" class="text-center">
                <p class="m-0 font-weight-bold" style="font-size: 40px"><kbd style="background-color:#fff;color:#000">{{ this.exception.status }}</kbd></p>
                <h6 class="text-muted mt-2">Возникла ошибка</h6>
                <h5>{{ this.exception.data }}</h5>
            </div>

            <template v-else>
                <b-card-body class="p-0 m-0">
                    <b-card no-body class="text-center p-1 border-0" style="font-size: 12px; vertical-align: middle">
                        <div class="text-muted" style="cursor: default">
                            <font-awesome-icon class="mr-2" icon="arrow-left" style="font-size:13px;color: #007bff"/>Входящий проект •
                            Исходящий проект<font-awesome-icon class="ml-2" icon="arrow-right" style="font-size: 13px;color: red"/>
                        </div>
                    </b-card>
                    <b-table fixed hover small :fields="telephonyShortImportantList" :items="telephonyShortCaclulate(items.telephonyShort)" style="margin-bottom:0; cursor: default">

                        <template slot="HEAD_serv" slot-scope="data">
                            <div id="tableHeadservTooltip" style="cursor: help">Провайдер</div>

                            <b-popover target="tableHeadservTooltip" triggers="hover">
                                <template slot="title">Провайдер телефонии</template>
                                <div style="font-size: 12px">
                                    <p class="p-0 mb-2">Столбец <b>Провайдер</b> показывает, через какого провайдера телефонии идёт вызов на данном проекте.</p>
                                </div>
                            </b-popover>
                        </template>

                        <template slot="HEAD_typE_PROJECT" slot-scope="data">
                            <div id="tableHeadtypE_PROJECTTooltip" style="cursor: help">Тип</div>

                            <b-popover target="tableHeadtypE_PROJECTTooltip" triggers="hover">
                                <template slot="title">Тип проекта</template>
                                <div style="font-size: 12px">
                                    <p class="p-0 mb-2">Столбец <b>Тип</b> показывает, к какому типу, входящему или исходящему, принадлежит проект.</p>
                                </div>
                            </b-popover>
                        </template>

                        <template slot="HEAD_numS_ALL" slot-scope="data">
                            <div id="tableHeadnumS_ALLTooltip" style="cursor: help">Все</div>

                            <b-popover target="tableHeadnumS_ALLTooltip" triggers="hover">
                                <template slot="title">Все вызовы</template>
                                <div style="font-size: 12px">
                                    <p class="p-0 mb-2">Столбец <b>Все</b> показывает общее количество совершённых вызовов за данных промежуток времени.</p>
                                </div>
                            </b-popover>
                        </template>

                        <template slot="HEAD_ok" slot-scope="data">
                            <div id="tableHeadOKTooltip" style="cursor: help">OK</div>

                            <b-popover target="tableHeadOKTooltip" triggers="hover">
                                <template slot="title">Успешные вызовы</template>
                                <div style="font-size: 12px">
                                    <p class="p-0 mb-2">Столбец <b>OK</b> говорит об отсутствии каких-либо проблем с чьей-либо стороны.</p>
                                    Входят SIP-коды:
                                    <p class="p-0 m-0"><b>200</b> - Успешный звонок</p>
                                    <p class="p-0 m-0"><b>201</b> - Успешный звонок</p>
                                </div>
                            </b-popover>
                        </template>

                        <template slot="HEAD_bad" slot-scope="data">
                            <div id="tableHeadBADTooltip" style="cursor: help">BAD</div>

                            <b-popover target="tableHeadBADTooltip" triggers="hover">
                                <template slot="title">Возможные проблемы</template>
                                <div style="font-size: 12px">
                                    <p class="p-0 mb-2">Столбец <b>BAD</b> говорит о завершении звонка по вине звонящего или кому звонят.</p>
                                    Входят SIP-коды:
                                    <p class="p-0 m-0"><b>403</b> - Абонент не зарегистрирован</p>
                                    <p class="p-0 m-0"><b>404</b> - Вызываемый абонент не найден, нет такого SIP-номера</p>
                                    <p class="p-0 m-0"><b>406</b> - Пользователь не доступен</p>
                                    <p class="p-0 m-0"><b>408</b> - Время обработки запроса истекло: Абонента не удалось найти за отведенное время</p>
                                    <p class="p-0 m-0"><b>480</b> - Неправильный номер телефона, не соответствует количество цифр или неправильный код страны или города</p>
                                    <p class="p-0 m-0"><b>486</b> - Абонент занят</p>
                                    <p class="p-0 m-0"><b>487</b> - Запрос отменен, обычно приходит при отмене вызова</p>
                                    <p class="p-0 m-0"><b>603</b> - Вызываемый пользователь не желает принимать входящие вызовы, не указывая причину отказа</p>
                                </div>
                            </b-popover>
                        </template>

                        <template slot="HEAD_err" slot-scope="data">
                            <div id="tableHeadERRTooltip" style="cursor: help">ERR</div>

                            <b-popover target="tableHeadERRTooltip" triggers="hover">
                                <template slot="title">Критичные проблемы</template>
                                    <div style="font-size: 12px">
                                        <p class="p-0 mb-2">Столбец <b>ERR</b> говорит наличии критически важных проблем на каналах связи.</p>
                                        Входят SIP-коды:
                                        <p class="p-0 m-0"><b>401</b> - Нет доступа к ресурсу</p>
                                        <p class="p-0 m-0"><b>410</b> - Время регистрации истекло</p>
                                        <p class="p-0 m-0"><b>415</b> - Звонок совершается неподдерживаемым кодеком</p>
                                        <p class="p-0 m-0"><b>420</b> - Неизвестное расширение: Сервер не понял расширение протокола SIP</p>
                                        <p class="p-0 m-0"><b>481</b> - Действие не выполнено, нормальный ответ при поступлении дублирующего пакета</p>
                                        <p class="p-0 m-0"><b>488</b> - Нет шлюзов с поддержкой заказанного кодека</p>

                                        <p class="p-0 m-0"><b>500</b> - Нет ответа от базы данных</p>
                                        <p class="p-0 m-0"><b>501</b> - В сервере не реализованы какие-либо функции, необходимые для обслуживания запроса</p>
                                        <p class="p-0 m-0"><b>502</b> - Сервер, функционирующий в качестве шлюза или прокси-сервера, принимает некорректный ответ от сервера, к которому он направил запрос</p>
                                        <p class="p-0 m-0"><b>503</b> - Сервер не может в данный момент обслужить вызов вследствие перегрузки или проведения технического обслуживания</p>

                                        <p class="p-0 m-0"><b>0</b> - Сбой на канале связи</p>
                                        <p class="p-0 m-0"><b>1</b> - Сбой на канале связи</p>
                                    </div>
                            </b-popover>
                        </template>

                        <template slot="HEAD_proc" slot-scope="row">
                            <div id="tableHeadProcTooltip" style="cursor: help"><b>%</b></div>

                            <b-popover target="tableHeadProcTooltip" triggers="hover">
                                <template slot="title">Отношение</template>
                                <div style="font-size: 12px">
                                    <p class="p-0 mb-2">Столбец <b>%</b> считает отношение проблемных(столбец <b>BAD</b>) и содержащих критичные ошибки(столбец <b>ERR</b>) вызовов ко всем совершённым(столбец <b>Все</b>) вызовам.</p>

                                    <hr class="mt-3 mb-2 p-0"/>

                                    <div class="text-center">
                                        Считается по формуле
                                        <p class="p-0 m-0"><kbd>(ERR + BAD) / Все * 100</kbd></p>
                                    </div>
                                </div>
                            </b-popover>
                        </template>

                        <template slot="exp" slot-scope="row">
                            <font-awesome-icon v-if="row.detailsShowing" icon="chevron-up" style="font-size:15px;cursor:hand" @click.stop="row.toggleDetails"/>
                            <font-awesome-icon v-else icon="chevron-down" style="font-size:15px;cursor:hand" @click.stop="row.toggleDetails"/>
                        </template>

                        <template slot="typE_PROJECT" slot-scope="row">
                            <template v-if="row.value === 'in'">
                                <font-awesome-icon icon="arrow-left" style="font-size:15px;color: #007bff"/>
                            </template>
                            <template v-if="row.value === 'out'">
                                <font-awesome-icon icon="arrow-right" style="font-size:15px;color: red"/>
                            </template>
                            <template v-if="row.value === 'in/out'">
                                <font-awesome-icon icon="arrow-left" style="font-size:15px;color: #007bff"/>
                                <font-awesome-icon icon="arrow-right" style="font-size:15px;color: red"/>
                            </template>
                        </template>

                        <template slot="proc" slot-scope="row">
                            {{row.value}}
                        </template>

                        <template slot="row-details" slot-scope="row">
                            <b-table
                                class="m-0" fixed hover small
                                thead-class="hidden_header"
                                :fields="telephonyShortImportantListExtended"
                                :items="items.telephonyFull.filter(function(item){ return item.projecT_NAME.startsWith(row.item.projecT_NAME);})"
                                style="cursor: default"
                            >

                                <template slot="projecT_NAME" slot-scope="row">
                                    <a :href="'http://37.221.186.119:8080/fx/npo/ru.naumen.npo.published_jsp?uuid='+row.item.projecT_ID+''" target="_blank">{{projectRename(row.value)}}</a>
                                </template>

                                <template slot="typE_PROJECT" slot-scope="row">
                                    <template v-if="row.value === 'in'">
                                        <font-awesome-icon icon="arrow-left" style="font-size:15px;color: #007bff"/>
                                    </template>
                                    <template v-if="row.value === 'out'">
                                        <font-awesome-icon icon="arrow-right" style="font-size:15px;color: red"/>
                                    </template>
                                </template>

                                <template slot="proc" slot-scope="row">
                                    {{row.value}}
                                </template>
                            </b-table>

                        </template>
                    </b-table>
                </b-card-body>
                <b-card-footer class="text-center p-0" style="cursor: default">
                    <span v-html="items.dateInterval" style="font-size:11px"/>
                </b-card-footer>
            </template>
        </b-collapse>
    </b-card>
</template>
<script>

    export default {
        props:["exception", "items"],
        data() {
            return {
                telephonyShortImportantList: [
                    { key: 'exp', label: '', 'class': 'text-center', 'thStyle': 'width:38px;' },
                    { key: "projecT_NAME", label: "Проект", 'thStyle': 'width:180px;' },
                    { key: "serv", label: "Провайдер", 'thStyle': 'width:100px;' },
                    { key: "typE_PROJECT", label: "Тип", 'class': 'text-center' },
                    { key: "numS_ALL", label: "Все", 'class': 'text-center' },
                    { key: "ok", label: "OK", 'class': 'text-center' },
                    { key: "bad", label: "BAD", 'class': 'text-center' },
                    { key: "err", label: "ERR", 'class': 'text-center' },
                    { key: "proc", label: "%", 'class': 'text-center' },
                ],
                telephonyShortImportantListExtended: [
                    { key: 'exp', 'class': 'text-center', 'tdClass': 'row1' },
                    { key: "projecT_NAME", 'tdClass': 'row2' },
                    { key: "serv", 'tdClass': 'row3' },
                    { key: "typE_PROJECT", 'tdClass': 'text-center' },
                    { key: "numS_ALL", 'tdClass': 'text-center' },
                    { key: "ok", 'tdClass': 'text-center' },
                    { key: "bad", 'tdClass': 'text-center' },
                    { key: "err", 'tdClass': 'text-center' },
                    { key: "proc", 'tdClass': 'text-center' }
                ]
            };
        },
        methods: {
            projectRename(r) {
                let raw;
                this.items.telephonyShort.filter(function(item) {
                    if(r.startsWith(item.projecT_NAME)) raw = r.replace(item.projecT_NAME + '_', '');
                });
                return raw;
            },
            telephonyShortCaclulate(arr) {
                if(arr) {
                    let sadArr = arr.map(x => {
                        if(x.proc > 50){
                            x._rowVariant = 'danger';
                            x._showDetails = true;
                        }

                        return x;
                    });
                    return sadArr;
                }
            }
        }
    };
</script>

<style>
    table.b-table > tbody > tr.b-table-details > td {
        padding: 0;
    }

    .row1 {
        width: 38px;
    }
    .row2 {
        width: 180px;
    }
    .row3 {
        width: 100px;
    }
</style>
<style scoped>
    .card-header {
        padding: 3px 10px;
    }

    .btnHover {
        display: none;
    }
    .btnHidden:hover >>> .btnHover {
        display: block;
        position: absolute;
    }
</style>
-->

<!--
<template>
    <div>
        <ve-line :data="test" :settings="chartSettings"/>
    </div>
</template>

<script>
    import { USER_GET } from '../../store/actions/user'
    
    export default {
        data () {
            return {
                test: [],

                chartSettings: {
                    area: true
                },
                chartData: {
                    columns: ['date', 'PV'],
                    rows: [
                        { 'date': '1/1', 'PV': 1393 },
                        { 'date': '1/2', 'PV': 3530 },
                        { 'date': '1/3', 'PV': null },
                        { 'date': '1/4', 'PV': 1723 },
                        { 'date': '1/5', 'PV': 3792 },
                        { 'date': '1/6', 'PV': 4593 }
                    ]
                }
            } 
        },
        methods: {
            async test1() {
                await this.$store.dispatch(USER_GET, '/api/dashboard/getasteriskdata').then((resp) => {
                    this.test = {
                        "columns": ['grafana_value_datetime', 'grafana_value'],
                        "rows":  JSON.parse(resp.data[0].grafana_value)
                    };
                })
            }
        },
        async created() {
            await this.test1();
        }
    }
</script>

<style scoped>

</style>-->
