<template>
    <div>
        <br />
        <hr />
        <h6 class="text-center">Для того, чтобы наделить кнопки рабочим функционалом, требуется <b-btn class="very-sm" size="sm" variant="outline-primary" @click="appDownload"><font-awesome-icon :icon="['fas', 'download']"/> Скачать</b-btn> и запустить утилиту установки</h6>
        <hr />
        
        <h5 class="text-center text-warning">Команды <strong>Linux</strong></h5> <br />
        <div class="d-flex justify-content-center">
            <ul>
                <li class="text-warning"><b-btn class="very-sm" size="sm" variant="warning"><font-awesome-icon icon="terminal"/> SSH</b-btn> - открыть соединение по протоколу <b>SSH</b></li>
                <li class="text-warning"><b-btn class="very-sm" size="sm" variant="warning"><font-awesome-icon icon="tv"/> VNC</b-btn> - открыть рабочий стол по протоколу <b>VNC</b></li>
                <li class="text-warning"><b-btn class="very-sm" size="sm" variant="warning"><font-awesome-icon icon="tv"/> VNC View</b-btn> - открыть рабочий стол по протоколу <b>VNC</b> в режиме просмотра</li>
                <li class="text-warning"><b-btn class="very-sm" size="sm" variant="warning"><font-awesome-icon icon="terminal"/> RSE</b-btn> - выполнить команду RSE на РМ оператора (~5 секунд)</li>
                <li class="text-warning"><b-btn class="very-sm" size="sm" variant="warning"><font-awesome-icon icon="volume-up"/> Pulse</b-btn> - запустить <b>Pulse Audio</b> удаленной машины у себя на <b>Windows</b></li>
                <li class="text-warning"><b-btn class="very-sm" size="sm" variant="warning"><font-awesome-icon icon="comment"/> Сообщение</b-btn> - отправить текстовое сообщение на РМ <b>текущему пользователю</b></li>
                <li class="text-warning"><b-btn class="very-sm" size="sm" variant="warning"><font-awesome-icon icon="redo"/> Перезагрузить</b-btn> - перезагрузить удаленный компьютер на ОС <b>Linux</b></li>
                <li class="text-warning"><b-btn class="very-sm" size="sm" variant="warning"><font-awesome-icon icon="power-off"/> Выключить</b-btn> - выключить удаленный компьютер на ОС <b>Linux</b></li>
            </ul>
        </div>

        <h5 class="text-center text-primary">Команды <strong>Windows</strong></h5> <br />
        <div class="d-flex justify-content-center">
            <ul>
                <li class="text-primary"><b-btn class="very-sm" size="sm" variant="primary"><font-awesome-icon icon="tv"/> RDP</b-btn> - открыть рабочий стол по протоколу <b>RDP</b></li>
                <li class="text-primary"><b-btn class="very-sm" size="sm" variant="primary"><font-awesome-icon icon="folder"/> C$</b-btn> - открыть в проводнике <b>Диск C</b> удаленного компьютера</li>
                <li class="text-primary"><b-btn class="very-sm" size="sm" variant="primary"><font-awesome-icon icon="folder"/> D$</b-btn> - открыть в проводнике <b>Диск D</b> удаленного компьютера</li>
                <li class="text-primary"><b-btn class="very-sm" size="sm" variant="primary"><font-awesome-icon icon="terminal"/> CMD</b-btn> - открыть <b>Командную строку</b> удаленного компьютера</li>
                <li class="text-primary"><b-btn class="very-sm" size="sm" variant="primary"><font-awesome-icon icon="terminal"/> PowerShell</b-btn> - открыть сессию <b>Powershell</b> удаленного компьютера</li>
                <li class="text-primary"><b-btn class="very-sm" size="sm" variant="primary"><font-awesome-icon icon="redo"/> Перезагрузить</b-btn> - перезагрузить удаленный компьютер на ОС <b>Windows</b></li>
                <li class="text-primary"><b-btn class="very-sm" size="sm" variant="primary"><font-awesome-icon icon="power-off"/> Выключить</b-btn> - выключить удаленный компьютер на ОС <b>Windows</b></li>
            </ul>
        </div>
    </div>
</template>

<script>
    export default {
        methods: {
            async appDownload() {
                this.$http({
                    url: '/api/commands/getwebbrutils',
                    method: 'GET',
                    responseType: 'blob'
                }).then((response) => {
                    const url = window.URL.createObjectURL(new Blob([response.data]));
                    const link = document.createElement('a');
                    link.href = url;
                    link.setAttribute('download', 'webbrUtils.bat');
                    document.body.appendChild(link);
                    link.click();
                });
            }
        }
    }
</script>

<style scoped>
    
</style>