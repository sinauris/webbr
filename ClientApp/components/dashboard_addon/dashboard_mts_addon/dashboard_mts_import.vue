<template>
    <div class="m-2 text-center" style="cursor: default">
                <b-card no-body>
                    <span slot="header" class="font-weight-bold">
                        <b-link href="http://37.221.187.183/Техническая_дирекция/Проекты/МТС_Банк/МТС_Банк._Скрипт_импорта._Проверка_работоспособности_и_перезапуск" target="_blank">Импорт</b-link>
                    </span>
                    <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default">
                        <b-col lg="1">
                            <div>
                                <font-awesome-icon v-if="!importSshResult || !importScriptResult" icon="circle" style="color: red"/>
                                <font-awesome-icon v-else icon="circle" style="color: limegreen"/>
                            </div>
                        </b-col>
                        <b-col lg="10">
                            <div v-if="!importSshResult || !importScriptResult" class="text-danger font-weight-bold">Скрипты импорта</div>
                            <div v-else>Скрипты импорта</div>
                        </b-col>
                        <b-col lg="1">
                            <div style="cursor: pointer" @click="mqImportCollapseShow = !mqImportCollapseShow">
                                <font-awesome-icon v-if="mqImportCollapseShow" icon="chevron-up"/>
                                <font-awesome-icon v-else icon="chevron-down"/>
                            </div>
                        </b-col>
                    </b-row>

                    <b-collapse v-model="mqImportCollapseShow" id="importCollapseShow">
                        <hr class="m-0 p-0"/>
                        <b-list-group flush>

                            <template v-for="v in mtsImportData" v-if="v.ssh_result === 'OK'">
                                <b-list-group-item v-if="v.import_result === 'RUNNING'" class="m-0 p-0 border-top-0">
                                    <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default">
                                        <b-col lg="1">
                                            <font-awesome-icon icon="circle" style="color: limegreen"/>
                                        </b-col>
                                        <b-col lg="7">
                                            <span class="text-success font-weight-bold" style="font-size: 13px">Запущен</span>
                                        </b-col>
                                        <b-col lg="4">
                                            <div style="font-size: 13px">{{ v.description }}</div>
                                            <div class="text-muted" style="font-size: 11px">{{ v.ip }}</div>
                                        </b-col>
                                    </b-row>
                                </b-list-group-item>

                                <b-list-group-item v-else class="m-0 p-0 border-top-0" style="background: red">
                                    <b-row align-v="center" class="m-1 text-white font-weight-bold" style="line-height: 1; cursor: default">
                                        <b-col lg="1">
                                            <font-awesome-icon icon="circle"/>
                                        </b-col>
                                        <b-col lg="7">
                                            <span>Остановлен</span>
                                        </b-col>
                                        <b-col lg="4">
                                            <div style="font-size: 13px">{{ v.description }}</div>
                                            <div style="font-size: 11px">{{ v.ip }}</div>
                                        </b-col>
                                    </b-row>
                                </b-list-group-item>
                            </template>

                            <template v-else>
                                <b-row align-v="center" class="m-1" style="line-height: 1; cursor: default;">
                                    <b-col lg="1">
                                        <font-awesome-icon icon="circle" style="color: red"/>
                                    </b-col>
                                    <b-col lg="7">
                                        <span class="text-danger font-weight-bold">Ошибка подключения</span>
                                    </b-col>
                                    <b-col lg="4">
                                        <div style="font-size: 14px;">{{ v.description }}</div>
                                        <div class="text-muted" style="font-size: 10px;">{{ v.ip }}</div>
                                    </b-col>
                                </b-row>
                                <b-alert v-if="v.ssh_exception" show variant="danger" class="mt-3 mb-0 p-0">
                                    <h6 class="mt-1 alert-heading">Ошибка</h6>
                                    <p class="m-1 p-0" style="font-size: 12px;">{{ v.ssh_exception }}</p>
                                </b-alert>
                            </template>

                        </b-list-group>
                        <b-card-footer v-if="mtsImportData.length !== 0" class="text-center p-0 border-top-0">
                            <div style="font-size:11px">
                                <abbr :title="$dt.fromISO(mtsImportData[0].updated)">{{ $dt.fromISO(mtsImportData[0].updated).toRelative() }}</abbr>
                            </div>
                        </b-card-footer>
                    </b-collapse>
                </b-card>
            </div>
</template>

<script>
    export default {
        props: ["mtsImportData"],
        data() {
            return {
                mqImportCollapseShow: false
            };
        },
        computed: {
            importSshResult() {
                if (this.mtsImportData) {
                    let result = true;
                    this.mtsImportData.forEach(x => {
                        if (x.ssh_result === 'FAIL') {
                            result = false;
                            this.mqImportCollapseShow = true;
                        }
                    });
                    return result;
                }
            },
            importScriptResult() {
                if (this.mtsImportData) {
                    let result = true;
                    this.mtsImportData.forEach(x => {
                        if (x.import_result === 'STOPPED') {
                            result = false;
                            this.mqImportCollapseShow = true;
                        }
                    });
                    return result;
                }
            }
        }
    }
</script>

<style scoped>

</style>