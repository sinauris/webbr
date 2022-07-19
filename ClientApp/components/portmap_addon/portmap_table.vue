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
                        
                        <b-btn class="ml-3" :pressed.sync="onlineOnly" size="sm" variant="outline-success" v-b-tooltip="{title: 'Показать Online', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}">Онлайн</b-btn>

                        <b-form-radio-group class="ml-3" buttons button-variant="outline-success" size="sm" v-model="radioSelected">
                            <b-form-radio value="" v-b-tooltip="{title: 'Показать все', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Все</b-form-radio>
                            <b-form-radio value="linux u1604" v-b-tooltip="{title: 'Показать Linux', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Linux</b-form-radio>
                            <b-form-radio value="windows" v-b-tooltip="{title: 'Показать Windows', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: pointer;">Windows</b-form-radio>
                        </b-form-radio-group>
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

                    <template slot="HEAD_onlinestatus" slot-scope="row">
                        <font-awesome-icon class="text-muted" icon="bolt" size="lg"/>
                    </template>

                    <template slot="onlinestatus" slot-scope="row">
                        <template v-if="row.value === 'online' && row.item.ip">
                            <b-btn v-if="row.detailsShowing" class="action-button" style="width: 100%" size="sm" @click="row.toggleDetails">
                                <font-awesome-icon icon="minus-circle" style="font-size:15px;color: #4caf50; vertical-align: sub;"/>
                            </b-btn>
                            <b-btn v-else class="action-button" style="width: 100%" size="sm" @click="row.toggleDetails">
                                <font-awesome-icon icon="plus-circle" style="font-size: 15px; color: #4caf50; vertical-align: sub;"/>
                            </b-btn>
                        </template>

                        <template v-if="row.value == 'offline'">
                            <b-btn v-if="row.detailsShowing" class="action-button" style="width: 100%" size="sm" @click="row.toggleDetails">
                                <font-awesome-icon icon="minus-circle" style="font-size: 15px; color: #9e9e9e; vertical-align: sub;"/>
                            </b-btn>
                            <b-btn v-else class="action-button" style="width: 100%" size="sm" @click="row.toggleDetails">
                                <font-awesome-icon icon="plus-circle" style="font-size: 15px; color: #9e9e9e; vertical-align: sub;"/>
                            </b-btn>
                        </template>
                        <template v-if="row.value == 'online' && !row.item.ip">
                            <b-btn class="action-button" style="width: 100%" size="sm" disabled>
                                <font-awesome-icon icon="plus-circle" style="font-size: 15px; color: #4caf50; vertical-align: sub;"/>
                            </b-btn>
                        </template>
                    </template>

                    <template slot="rm" slot-scope="row">
                        <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{ row.value }}</span>
                    </template>

                    <template slot="ip" slot-scope="row">
                        <template v-if="row.item.typeip === 'Dynamic'">
                            <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{ row.value }}</span>
                            <b-badge variant="success" style="cursor: help" v-b-tooltip.hover.top title="Динамический IP">DHCP</b-badge>
                        </template>
                        <template v-else-if="row.item.typeip === 'Static'">
                            <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{ row.value }}</span>
                            <b-badge style="background-color: #9e9e9e; cursor: help" v-b-tooltip.hover.top title="Статический IP">DHCP</b-badge>
                        </template>
                        <template v-else>
                            <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{ row.value }}</span>
                        </template>
                    </template>

                    <template slot="mac" slot-scope="row">
                        <span @click="copyToClipboard(row.value.toUpperCase())" style="cursor: pointer">{{ row.value.toUpperCase() }}</span>
                    </template>

                    <template slot="sw" slot-scope="row">
                        <b-link :href="'http://'+row.value+''" target="_blank" style="text-decoration: none">{{ row.value }}</b-link>
                    </template>

                    <template slot="swport" slot-scope="row">
                        <span style="cursor: default">{{ row.value }}</span>
                    </template>

                    <template slot="vlan" slot-scope="row">
                        <span @click="copyToClipboard(row.value)" style="cursor: pointer">{{ row.value }}</span>
                    </template>

                    <template slot="nau_login" slot-scope="row">
                        <b-link :id="'nauPop-'+row.item.mac" :href="'http://37.221.186.119:8080/fx/npo/ru.naumen.npo.published_jsp?uuid=' + row.item.nau_uuid" target="_blank" style="text-decoration: none">
                            <template v-if="row.item.nau_roles">
                                <b-badge v-if="row.item.nau_roles.includes('Руководитель группы') || row.item.nau_roles.includes('Руководители групп')" style="background-color: #ff9d00; color: #000">РГ</b-badge>
                                <b-badge v-else-if="row.item.nau_roles.includes('Супервизор')" style="background-color: #ffd809; color: #000">СВ</b-badge>                                
                                <b-badge v-else-if="row.item.nau_roles.includes('Оператор')">ОП</b-badge>
                                <b-badge v-else>{{ row.item.nau_roles }}</b-badge>
                            </template>
                            <template v-if="!row.item.nau_roles">
                                <b-badge v-if="row.item.nau_department === 'Операторы'">ОП</b-badge>
                                <b-badge v-else-if="row.item.nau_department === 'Супервайзеры'" style="background-color: #ffd809; color: #000">СВ</b-badge>
                                <b-badge v-else-if="row.item.nau_department === 'Руководители групп'" style="background-color: #ff9d00; color: #000">РГ</b-badge>
                                <b-badge v-else-if="row.item.nau_department === 'Группа управления качеством'" style="background-color: #e8fff6; color: #000">ГУК</b-badge>
                                <b-badge v-else-if="row.item.nau_department === 'Группа планирования загрузки'" style="background-color: #e8fff6; color: #000">ГПЗ</b-badge>
                                <b-badge v-else-if="row.item.nau_department === 'Учебный центр'" style="background-color: #e8fff6; color: #000">УЦ</b-badge>
                                <b-badge v-else-if="row.item.nau_department === 'Менеджеры проектов'" style="background-color: #e8fff6; color: #000">ПМ</b-badge>
                                <b-badge v-else-if="row.item.nau_department === 'Отдел рекрутинга'" style="background-color: #e8fff6; color: #000">HR</b-badge>
                                <b-badge v-else-if="row.item.nau_department === 'Отдел кадров'" style="background-color: #e8fff6; color: #000">ОК</b-badge>
                                <b-badge v-else-if="row.item.nau_department === 'Отдел технической поддержки'" style="background-color: #000; color: #fff">ОИТ</b-badge>
                                <b-badge v-else>{{ row.item.nau_department }}</b-badge>
                            </template>
                            {{ row.value }}
                        </b-link>
                        
                        <template v-if="row.item.nau_skills && row.item.nau_projects && row.item.nau_profile_name !== 'Administration' && !row.item.nau_roles.includes('Супервизор') && !row.item.nau_roles.includes('Руководитель группы') && !row.item.nau_roles.includes('Руководители групп')">
                            <template v-for="proj in projectsArray(JSON.parse(row.item.nau_skills), JSON.parse(row.item.nau_projects))">
                                <b-badge :href="'http://37.221.186.119:8080/fx/npo/ru.naumen.npo.published_jsp?uuid=' + proj.Object" target="_blank" class="mr-1" variant="success">
                                    <template v-if="proj.Type === 'outcoming_project'">
                                        <span>Исх |</span>
                                    </template>
                                    <template v-if="proj.Type === 'incoming_project'">
                                        <span>Вхд |</span>
                                    </template>

                                    {{ proj.Name }}
                                </b-badge>
                            </template>
                        </template>
                        
                        <b-popover :target="'nauPop-'+row.item.mac" placement="left" triggers="hover">
                            <template slot="title" style="text-align:center">
                                <b>{{ row.item.nau_first_name }}</b> <b v-if="row.item.nau_last_name">{{ row.item.nau_last_name }}</b>
                            </template>
                            <div>
                                ФИО: 
                                <b>
                                    <template v-if="row.item.nau_last_name">{{ row.item.nau_last_name }}</template>
                                    <template v-if="row.item.nau_first_name">{{ row.item.nau_first_name }}</template>
                                    <template v-if="row.item.nau_middle_name">{{ row.item.nau_middle_name }}</template>
                                </b>
                            </div>
                            <div v-if="row.item.nau_login">Логин: <b>{{ row.item.nau_login }}</b></div>
                            
                            <div v-if="row.item.nau_roles">Роль:
                                <template v-for="role in row.item.nau_roles.split('; ')">
                                    <span><b-badge style="margin-right: 3px; font-size: 11px" variant="info">{{ role }}</b-badge></span>
                                </template>
                            </div>
                            
                            <div v-if="row.item.nau_department">Отдел: <b>{{ row.item.nau_department }}</b></div>
                            
                            <div v-if="row.item.nau_profile_name">Профиль: <b>{{ row.item.nau_profile_name }}</b></div>
                            <div v-if="row.item.nau_location_name">Площадка: <b>{{ row.item.nau_location_name }}</b></div>
                            <div v-if="row.item.nau_creation_time">Дата создания: <b>{{ $dt.fromISO(row.item.nau_creation_time).toRelative() }}</b></div>
                            <div v-if="row.item.nau_skills">Проекты: <br/>
                                <template v-for="skill in JSON.parse(row.item.nau_skills)">
                                    <div v-if="skill.Name">
                                        <b-badge style="font-size: 11px" variant="success">
                                            <template v-if="skill.Type === 'outcoming_project'">
                                                <span>Исх |</span>
                                            </template>
                                            <template v-if="skill.Type === 'incoming_project'">
                                                <span>Вхд |</span>
                                            </template>
                                            {{ skill.Name }}
                                        </b-badge>
                                    </div>
                                </template>
                            </div>
                        </b-popover>
                    </template>

                    <template slot="typeos" slot-scope="row">
                        <template v-if="row.value.includes('Windows')">
                            <span style="cursor: help" :id="'winPop-'+row.item.mac">
                                <font-awesome-icon :icon="['fab', 'windows']"/> {{ row.item.win_computer_name }} • {{ row.item.win_user_login }}
                            </span>
                            <b-popover :target="'winPop-'+row.item.mac" placement="left" triggers="hover">
                                <template slot="title" style="text-align:center">
                                    <b>{{ row.item.win_computer_name }}</b>
                                </template>
                                <div v-if="row.item.win_user_username">ФИО: <b>{{ row.item.win_user_username }}</b></div>
                                <div v-if="row.item.win_user_login">Логин: <b>{{ row.item.win_user_login }}</b></div>
                                <div v-if="row.item.win_user_department">Отдел: <b>{{ row.item.win_user_department }}</b></div>
                                <div v-if="row.item.win_user_description">Должность: <b>{{ row.item.win_user_description }}</b></div>
                                <div v-if="row.item.win_user_city">Площадка: <b>{{ row.item.win_user_city }}</b></div>
                                <div v-if="row.item.win_computer_power_datetime">Включение: <b>{{ $dt.fromISO(row.item.win_computer_power_datetime).toRelative() }}</b></div>
                                <div v-if="row.item.win_computer_logon_datetime">Вход выполнен: <b>{{ $dt.fromISO(row.item.win_computer_logon_datetime).toRelative() }}</b></div>
                                <div v-if="row.item.typeos">Система: <b>{{ row.item.typeos }}</b></div>
                            </b-popover>
                        </template>

                        <template v-else-if="row.item.image">
                            <span style="cursor: default">
                                <font-awesome-icon :icon="['fab', 'linux']"/> {{ row.value }} • <span v-b-tooltip="{title: 'Название образа', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">{{row.item.image}}</span>
                            </span>
                        </template>
                    </template>

                    <template slot="computer" slot-scope="row">
                        <b-row class="text-center">
                            <b-col cols="3">
                                <template v-if="row.item.inv_cpu">
                                <span :id="'cpu-' + row.item.mac" style="cursor: help">
                                    <font-awesome-icon icon="microchip" />
                                </span>
                                    <b-tooltip :target="'cpu-' + row.item.mac" placement="top">
                                        <div>{{ row.item.inv_motherboard }}</div>
                                        <hr style="background-color: white; margin: 10px 0 5px 0"/>
                                        <div>{{ row.item.inv_cpu }}</div>
                                    </b-tooltip>
                                </template>
                            </b-col>
                            <b-col cols="3">
                                <template v-if="row.item.inv_ram">
                                    <span :id="'ram-' + row.item.mac" style="cursor: help"><font-awesome-icon icon="memory" /></span>
                                    <b-tooltip :target="'ram-' + row.item.mac" placement="top">
                                        <div style="text-align: start" v-for="(item, index) in row.item.inv_ram.split(';')">
                                            <template v-if="row.item.inv_ram.split(';').length !== 1">{{ index + 1 }}.</template> {{ item }}
                                        </div>
                                    </b-tooltip>
                                </template>
                            </b-col>
                            <b-col cols="3">
                                <template v-if="row.item.inv_hdd">
                                    <span :id="'hdd-' + row.item.mac" style="cursor: help"><font-awesome-icon icon="hdd" /></span>
                                    <b-tooltip :target="'hdd-' + row.item.mac" placement="top">
                                        <div style="text-align: start" v-for="(item, index) in row.item.inv_hdd.split(';')">
                                            <template v-if="row.item.inv_hdd.split(';').length !== 1">{{ index + 1 }}.</template> {{ item }}
                                        </div>
                                    </b-tooltip>
                                </template>
                            </b-col>
                            <b-col cols="3">
                                <template v-if="row.item.inv_monitor">
                                    <span :id="'mon-' + row.item.mac" style="cursor: help"><font-awesome-icon icon="tv" /></span>
                                    <b-tooltip :target="'mon-' + row.item.mac" placement="top">
                                        <div style="text-align: start" v-for="(item, index) in row.item.inv_monitor.split('; ')">
                                            <template v-if="row.item.inv_monitor.split('; ').length !== 1">{{ index + 1 }}.</template> {{ item.replace(';', '') }}
                                        </div>
                                    </b-tooltip>
                                </template>
                            </b-col>
                        </b-row>
                    </template>

                    <template slot="updatetime" slot-scope="row">
                        <span style="cursor: default">{{ $dt.fromISO(row.value).toRelative() }}</span>
                    </template>

                    <template slot="HEAD_actions" slot-scope="data">
                        <font-awesome-icon
                            v-if="dataLoading"
                            icon="cog"
                            size="lg"
                            spin
                            disabled 
                        />
                        <font-awesome-icon
                            v-else
                            @click="load"
                            icon="retweet"
                            size="lg"
                            style="cursor:pointer;"
                            v-b-tooltip="{title: 'Обновить данные', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" 
                        />
                    </template>
                    
                    <template slot="actions" slot-scope="row">
                        <b-btn-group style="width: 100%">
                            <b-btn
                                class="action-button"
                                size="sm"
                                style="width:100%"
                                @click.stop="changeModal(row.item)"
                                v-b-modal.changeRm v-b-tooltip="{title: 'Изменить запись', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}">
                                <font-awesome-icon icon="pencil-alt"/>
                            </b-btn>
                            <b-btn
                                class="action-button"
                                size="sm"
                                style="width:100%"
                                @click.stop="deleteRm(row.item.sw, row.item.swport)"
                                v-b-tooltip="{title: 'Удалить запись', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}">
                                <font-awesome-icon icon="trash-alt"/>
                            </b-btn>
                        </b-btn-group>
                    </template>

                    <template slot="row-details" slot-scope="row">
                        <div v-if="row.item.typeos === 'Linux'">
                            <div class="d-flex">
                                <div style="padding:4px">
                                    <b-btn class="very-sm" size="sm" variant="warning" @click="sshCommand(row.item.rm, row.item.ip)"><font-awesome-icon icon="terminal"/> SSH</b-btn>
                                    <b-btn class="very-sm" size="sm" variant="warning" @click="vncCommand(row.item.rm, row.item.ip)"><font-awesome-icon icon="tv"/> VNC</b-btn>
                                    <b-btn class="very-sm" size="sm" variant="warning" @click="vncvoCommand(row.item.rm, row.item.ip)"><font-awesome-icon icon="tv"/> VNC View</b-btn>

                                    <b-btn v-if="!rseButtonLoad" class="very-sm" size="sm" variant="warning" @click="rseCommand(row.item.rm, row.item.ip)" style="width: 55px;"><font-awesome-icon icon="terminal"/> RSE</b-btn>
                                    <b-btn v-else class="very-sm" size="sm" style="width: 55px;" disabled><font-awesome-icon icon="cog" spin/></b-btn>


                                    <b-btn class="very-sm" size="sm" variant="warning" @click="pulseCommand(row.item.rm, row.item.ip)"><font-awesome-icon icon="volume-up"/> Pulse</b-btn>
                                    <b-btn class="very-sm" size="sm" variant="warning" :id="'messagePop-'+row.item.mac"><font-awesome-icon icon="comment"/> Сообщение</b-btn>
                                    <b-popover :target="'messagePop-'+row.item.mac" @show="onShow" triggers="click blur">
                                        <template slot="title">
                                            Отправить сообщение
                                        </template>
                                        <b-form @submit.stop.prevent="messageCommand(row.item.rm, row.item.ip)">
                                            <b-input-group size="sm" style="padding-bottom: 10px;">
                                                <b-input-group-text slot="prepend">
                                                    <font-awesome-icon icon="heading"/>
                                                </b-input-group-text>
                                                <b-form-input v-model="messageTitle" placeholder="Заголовок"/>
                                            </b-input-group>
                                            <b-input-group size="sm" style="padding-bottom: 14px;">
                                                <b-input-group-text slot="prepend">
                                                    <font-awesome-icon icon="comment-dots"/>
                                                </b-input-group-text>
                                                <b-form-textarea v-model="messageBody" :rows="1" :max-rows="4" placeholder="Тело сообщения" required/>
                                            </b-input-group>
                                            <b-btn type="submit" class="very-sm" :disabled="messageButtonDisabled" :variant="messageButtonVariant" block size="sm" v-html="messageButtonName"></b-btn>
                                        </b-form>
                                    </b-popover>
                                </div>
                                <div class="ml-auto" style="padding:4px">
                                    <b-btn v-if="!rebootButtonLoad" class="very-sm" size="sm" variant="warning" @click="rebootLinuxCommand(row.item.rm, row.item.ip)" style="min-width: 106px"><font-awesome-icon icon="redo"/> Перезагрузить</b-btn>
                                    <b-btn v-else class="very-sm" size="sm" style="width: 106px" disabled><font-awesome-icon icon="cog" spin/></b-btn>

                                    <b-btn v-if="!shutdownButtonLoad" class="very-sm" size="sm" variant="warning" @click="shutdownLinuxCommand(row.item.rm, row.item.ip)" style="min-width: 90px"><font-awesome-icon icon="power-off"/> Выключить</b-btn>
                                    <b-btn v-else class="very-sm" size="sm" style="width: 90px" disabled><font-awesome-icon icon="cog" spin/></b-btn>
                                </div>
                            </div>
                        </div>
                        <div v-else>
                            <div class="d-flex">
                                <div style="padding:4px">
                                    <b-btn class="very-sm" size="sm" variant="primary" @click="rdpCommand(row.item.rm, row.item.ip)"><font-awesome-icon icon="tv"/> RDP</b-btn>
                                    <b-btn class="very-sm" size="sm" variant="primary" @click="explorerCCommand(row.item.rm, row.item.ip)"><font-awesome-icon icon="folder"/> C$</b-btn>
                                    <b-btn class="very-sm" size="sm" variant="primary" @click="explorerDCommand(row.item.rm, row.item.ip)"><font-awesome-icon icon="folder"/> D$</b-btn>
                                    <b-btn class="very-sm" size="sm" variant="primary" @click="cmdCommand(row.item.rm, row.item.ip)"><font-awesome-icon icon="terminal"/> CMD</b-btn>
                                    <b-btn class="very-sm" size="sm" variant="primary" @click="pwsCommand(row.item.rm, row.item.windowspcname)"><font-awesome-icon icon="terminal"/> PowerShell</b-btn>
                                </div>
                                <div class="ml-auto" style="padding:4px">
                                    <b-btn class="very-sm" size="sm" variant="primary" @click="rebootWindowsCommand(row.item.rm, row.item.ip)"><font-awesome-icon icon="redo"/> Перезагрузить</b-btn>
                                    <b-btn class="very-sm" size="sm" variant="primary" @click="shutdownWindowsCommand(row.item.rm, row.item.ip)"><font-awesome-icon icon="power-off"/> Выключить</b-btn>
                                </div>
                            </div>
                        </div>
                    </template>
                </b-table>

                <b-row>
                    <b-col md="6" class="d-flex align-self-center">
                        <div>Всего на странице <b>{{ totalRows }}</b></div>
                    </b-col>

                    <b-col v-if="this.totalRows > perPage" md="2" class="ml-auto">
                        <b-pagination align="right" :total-rows="totalRows" :per-page="perPage" v-model="currentPage" class="my-0"/>
                    </b-col>
                </b-row>
            </b-col>
        </b-row>

        <b-modal
            id="changeRm"
            title="Изменение РМ"
            @ok="changeModalSubmit"
            ref="changeRmRef"
            hide-footer>
            <b-form @submit.stop.prevent="changeModalSubmit">
                <b-form-group label-for="modalRm">
                    <template slot="label">
                        <b>Название РМ</b>
                    </template>
                    <b-form-input size="sm" id="modalRm" type="text" v-model="modal.rm" placeholder="Номер или буквенное обозначение"/>
                </b-form-group>
                <b-form-group label-for="modalIp">
                    <template slot="label">
                        <b>IP-адрес</b> <span v-b-tooltip.hover title="При ручном изменении IP-адреса возможна его замена с последующим опросом."><font-awesome-icon :icon="['far', 'question-circle']" size="sm" style="cursor: help" /></span>
                    </template>
                    <b-form-input size="sm" id="modalIp" type="text" v-model="modal.ip" placeholder="XXX_XXX_XXX_XXX"/>
                </b-form-group>
                <b-form-group label-for="modalMac">
                    <template slot="label">
                        <b>MAC-адрес</b>
                    </template>
                    <b-form-input size="sm" id="modalMac" type="text" v-model="modal.mac" disabled/>
                </b-form-group>
                <b-row>
                    <b-col cols="6">
                        <b-form-group label-for="modalSw">
                            <template slot="label">
                                <b>Коммутатор</b>
                            </template>
                            <b-form-input size="sm" id="modalSw" type="text" v-model="modal.sw" disabled/>
                        </b-form-group>
                    </b-col>
                    <b-col cols="2" offset="1">
                        <b-form-group label-for="modalSwport">
                            <template slot="label">
                                <b>Порт</b>
                            </template>
                            <b-form-input size="sm" id="modalSwport" type="text" v-model="modal.swport" disabled/>
                        </b-form-group>
                    </b-col>
                    <b-col cols="2" offset="1">
                        <b-form-group label-for="modalVlan">
                            <template slot="label">
                                <b>Vlan</b>
                            </template>
                            <b-form-input size="sm" id="modalVlan" type="text" v-model="modal.vlan" disabled/>
                        </b-form-group>
                    </b-col>
                </b-row>
                <hr/>
                <b-btn class="float-right" type="submit" variant="primary">Сохранить</b-btn>
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
                modal: {
                    rm: '',
                    ip: '',
                    mac: '',
                    sw: '',
                    swport: '',
                    vlan: ''
                },
                fields: [
                    { key: 'onlinestatus', label: '', 'class': 'text-center text-nowrap', 'thStyle': 'width:38px;', 'tdClass': 'betterButton' },
                    { key: 'rm', label: 'РМ', sortable: true, 'thStyle': 'width:240px;', 'class': 'text-nowrap' },
                    { key: 'ip', label: 'IP', sortable: true, 'thStyle': 'width:170px;', 'class': 'text-nowrap' },
                    { key: 'mac', label: 'MAC', sortable: true, 'thStyle': 'width:130px;', 'class': 'text-nowrap' },
                    { key: 'sw', label: 'Коммутатор', sortable: true, 'thStyle': 'width:125px;', 'class': 'text-nowrap' },
                    { key: 'swport', label: 'Порт', sortable: true, 'thStyle': 'width:75px;', 'class': 'text-nowrap' },
                    { key: 'vlan', label: 'Vlan', sortable: true, 'thStyle': 'width:75px;', 'class': 'text-nowrap' },
                    { key: 'nau_login', label: 'Naumen', sortable: true, 'thStyle': 'width:210px;', 'class': 'text-nowrap' },
                    { key: 'typeos', label: 'Система', sortable: true, 'thStyle': 'width:250px;', 'class': 'text-nowrap' },
                    { key: 'computer', label: 'Компьютер', 'thStyle': 'width:130px;', 'class': 'text-nowrap' },
                    { key: 'updatetime', label: 'Обновлено', sortable: true, 'thStyle': 'width:130px;', 'class': 'text-nowrap' },
                    { key: 'actions', label: 'Действия', 'class': 'text-center text-nowrap', 'thStyle': 'width:60px;', 'tdClass': 'betterButton' }
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
                navigateVariable: '',

                onlineOnly: false,
                radioSelected: '',

                messageButtonName: 'Отправить',
                messageButtonVariant: 'primary',
                messageButtonDisabled: false,
                messageTitle: '',
                messageBody: '',

                rseButtonLoad: false,
                rebootButtonLoad: false,
                shutdownButtonLoad: false
            }
        },
        mounted() {
            this.$nextTick(() => this.$refs.searchInput.focus())
        },

        activated() {
            this.$nextTick(() => this.$refs.searchInput.focus())
        },

        watch: {
            async '$route'() { if (this.$route.name === 'portmapid' && this.$route.params.id) await this.tableLoad() }
        },

        methods: {
            onFiltered(filteredItems) {
                this.totalRows = filteredItems.length;
                // this.currentPage = 1
            },
            onShow() {
                this.messageTitle = '';
                this.messageBody = '';
                this.messageButtonName = 'Отправить';
                this.messageButtonDisabled = false;
                this.messageButtonVariant = 'primary';
            },
            
            projectsArray(skillsArr, projectsArr) {
                let array = [];
                for (let j = projectsArr.length; j--;) {
                    for (let i = skillsArr.length; i--;) {
                        if(skillsArr[i].Name && projectsArr[j].Name && skillsArr[i].Name.includes(projectsArr[j].Name)) 
                            array.push({ 'Name': projectsArr[j].Name, 'Object': projectsArr[j].Object, 'Type': skillsArr[i].Type });
                    }
                }

                const filteredArr = array.reduce((acc, current) => {
                    const x = acc.find(item => item.Name === current.Name);
                    if (!x) {
                        return acc.concat([current]);
                    } else {
                        return acc;
                    }
                }, []);
                return [...new Set(filteredArr)]
            },
            
            
            getDistinctSkills(array) {
                let arrComplete = [];

                let test = array.filter(x => x.nau_skills).map(x => JSON.parse(x.nau_skills));
                for (let i = test.length; i--;) {
                    for (let j = test[i].length; j--;) {
                        if(test[i][j].Level >= 1) arrComplete.push(test[i][j].Name);
                    }
                }
                
                // for (let i = array.length; i--;) {
                //     let arr1 = [];
                //    
                //     if(array[i].nau_skills) {
                //         console.log(JSON.parse(array[i].nau_skills));
                //         arr1.push(JSON.parse(array[i].nau_skills));
                //     }
                //    
                //     for (let j = arr1.length; j--;) {
                //         // let json = JSON.parse(arr1[j]);
                //         console.log(arr1);
                //         // if(json.Level >= 1) arrComplete.push(json.Name);
                //     }
                //    
                // }
                let arrSet = [...new Set(arrComplete)];
                // console.log(arrSet);

                // for (let i = arrSet.length; i--;) {
                    // console.log(asd)
                // }
                
                
                return arrSet
            },
            // getUniqueCount(key) {
            //     let res = this.items.reduce((acc, o) => (acc[o[key]] = (acc[o[key]] || 0)+1, acc), {} );
            //     console.log(res)
            //     return Object.entries(res).map(( [k, v] ) => ({ text: [k] + ' (' + (v) + ')', value: k }));
            // },

            async deleteRm(sw, swport) {
                let acceptDelete = confirm('Подтверждаете удаление?');
                if(acceptDelete){
                    await this.$store.dispatch(USER_POST, {
                        url: '/api/portmap/deleteportmap',
                        sw: sw,
                        swport: swport
                    })
                        .then(async () => {
                            await this.load();
                        });
                }
            },

            changeModal(item) {
                this.modal.rm = item.rm;
                this.modal.ip = item.ip;
                this.modal.mac = item.mac;
                this.modal.sw = item.sw;
                this.modal.swport = item.swport;
                this.modal.vlan = item.vlan;
            },

            async changeModalSubmit() {
                await this.$store.dispatch(USER_POST, {
                    url: '/api/portmap/updateportmap',
                    sw: this.modal.sw,
                    swport: this.modal.swport,
                    rm: this.modal.rm,
                    ip: this.modal.ip
                })
                .then(async () => {
                    this.$refs.changeRmRef.hide();
                    await this.load();
                });
            },

            // [Linux] Кнопки команд
            async sshCommand(rm, ip) {
                window.location.href = "ssh://" + ip;
                await this.$store.dispatch(USER_POST, { url: '/api/commands/sshcommand', rm: rm, ip: ip });
            },
            async vncCommand(rm, ip) {
                window.location.href = "vnc://" + ip;
                await this.$store.dispatch(USER_POST, { url: '/api/commands/vnccommand', rm: rm, ip: ip });
            },
            async vncvoCommand(rm, ip) {
                window.location.href = "vncvo://" + ip;
                await this.$store.dispatch(USER_POST, { url: '/api/commands/vncvocommand', rm: rm, ip: ip });
            },
            async rseCommand(rm, ip) {
                this.rseButtonLoad = true;
                await this.$store.dispatch(USER_POST, { url: '/api/commands/rsecommand', rm: rm, ip: ip })
                    .then(() => this.rseButtonLoad = false )
            },
            async pulseCommand(rm, ip) {
                window.location.href = "xfw://" + ip;
                await this.$store.dispatch(USER_POST, { url: '/api/commands/pulsecommand', rm: rm, ip: ip });
            },
            async messageCommand(rm, ip) {
                await this.$store.dispatch(USER_POST, { url: '/api/commands/messagecommand', rm: rm, ip: ip, title: this.messageTitle, message: this.messageBody })
                    .then(() => {
                        this.messageButtonName = 'Прочитано';
                        this.messageButtonVariant = 'success';
                        })
                    .catch(() => {
                        this.messageButtonName = 'Ошибка';
                        this.messageButtonVariant = 'danger';
                    });
            },
            async rebootLinuxCommand(rm, ip) {
                let confirmQuestions = confirm('Подтверждаете перезагрузку ПК?');
                if(confirmQuestions) {
                    this.rebootButtonLoad = true;
                    await this.$store.dispatch(USER_POST, { url: '/api/commands/rebootlinuxcommand', rm: rm, ip: ip });
                    this.rebootButtonLoad = false
                }

            },
            async shutdownLinuxCommand(rm, ip) {
                let confirmQuestions = confirm('Подтверждаете выключение ПК?');
                if(confirmQuestions) {
                    this.shutdownButtonLoad = true;
                    await this.$store.dispatch(USER_POST, {url: '/api/commands/shutdownlinuxcommand', rm: rm, ip: ip});
                    this.shutdownButtonLoad = false
                }
            },


            // [Windows] Кнопки команд
            async rdpCommand(rm, ip) {
                window.location.href = "rdp://" + ip;
                await this.$store.dispatch(USER_POST, { url: '/api/commands/rdpcommand', rm: rm, ip: ip });
            },
            async explorerCCommand(rm, ip) {
                window.location.href = "ecs://" + ip;
                await this.$store.dispatch(USER_POST, { url: '/api/commands/explorerccommand', rm: rm, ip: ip });
            },
            async explorerDCommand(rm, ip) {
                window.location.href = "eds://" + ip;
                await this.$store.dispatch(USER_POST, { url: '/api/commands/explorerdcommand', rm: rm, ip: ip });
            },
            async cmdCommand(rm, ip) {
                window.location.href = "pxs://" + ip;
                await this.$store.dispatch(USER_POST, { url: '/api/commands/cmdcommand', rm: rm, ip: ip });
            },
            async pwsCommand(rm, ip) {
                window.location.href = "pws://" + ip;
                await this.$store.dispatch(USER_POST, { url: '/api/commands/pwscommand', rm: rm, ip: ip });
            },
            async rebootWindowsCommand(rm, ip) {
                let confirmQuestions = confirm('Подтверждаете перезагрузку ПК?');
                if(confirmQuestions) {
                    window.location.href = "psr://" + ip;
                    await this.$store.dispatch(USER_POST, { url: '/api/commands/rebootwindowscommand', rm: rm, ip: ip });
                }
            },
            async shutdownWindowsCommand(rm, ip) {
                let confirmQuestions = confirm('Подтверждаете выключение ПК?');
                if(confirmQuestions) {
                    window.location.href = "pss://" + ip;
                    await this.$store.dispatch(USER_POST, { url: '/api/commands/shutdownwindowscommand', rm: rm, ip: ip });
                }
            },

            copyToClipboard(item) {
                const el = document.createElement('textarea');
                el.value = item;
                document.body.appendChild(el);
                el.select();
                document.execCommand('copy');
                document.body.removeChild(el);
            },

            async load() {
                this.$nextTick(() => this.$refs.searchInput.focus());
                this.currentPage = 1;
                this.dataLoading = true;
                let response = await this.$store.dispatch(USER_GET, '/api/portmap/getportmap' + this.$route.params.id);
                this.items = response.data;
                // this.getDistinctSkills(response.data);
                this.dataLoading = false;
                this.totalRows = this.items.length;
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
            }
        },

        computed: {
            ...mapGetters(['getPortmapNav']),
            
            filterTable() {
                let filterArray = this.filter.split(' ');
                filterArray.push(this.onlineOnly ? 'online' : '');
                filterArray = filterArray.concat(this.radioSelected.split(' '));
                let array = '^' + filterArray.map(x => x ? '(?=.*' + x + ')' : '').join('');
                return new RegExp(array, 'gi');
            }
        },

        async created() {
            await this.tableLoad();
        }
    }
</script>

<style scoped>
    
</style>