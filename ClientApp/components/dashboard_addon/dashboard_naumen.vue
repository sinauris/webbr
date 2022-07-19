<template>
    <b-card no-body header-class="card-header-progress" class="btnHidden">
        <div slot="header" class="text-center font-weight-bold">
            <div style="font-size:14px; cursor: default;">
                <font-awesome-icon :icon="['far', 'address-book']"/> Лицензии
            </div>
        </div>
        <b-card-body class="p-0 m-0">
            <b-list-group flush>
                <b-table class="mb-0" hover small :fields="fieldsLicense" :items="naumenLicenseCaclulate(items)">
                    <template slot="HEAD_license_name" slot-scope="row">
                        <span v-b-tooltip="{title: 'Название лицензии', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">Название</span>
                    </template>
                    <template slot="HEAD_license_use" slot-scope="row">
                        <span v-b-tooltip="{title: 'Задействованные лицензии', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">Исп.</span>
                    </template>
                    <template slot="HEAD_license_all" slot-scope="row">
                        <span v-b-tooltip="{title: 'Доступные лицензии', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">Всего</span>
                    </template>
                    <template slot="license_name" slot-scope="row">
                        <span v-b-tooltip="{title: row.item.license_description, placement: 'left', trigger: 'hover', delay: { 'show': 300, 'hide': 50 }}" style="cursor: help">{{row.value}}</span>
                    </template>
                </b-table>
            </b-list-group>
        </b-card-body>
    </b-card>
</template>

<script>
    export default {
        props: ["items"],
        data() {
            return {
                fieldsLicense: [
                    { key: 'license_name', label: 'Название', 'thStyle': 'cursor: default', 'class': 'defaultCursor' },
                    { key: 'license_use', label: 'Исп.', 'class': 'text-center defaultCursor', 'thStyle': 'cursor: default' },
                    { key: 'license_all', label: 'Всего', 'class': 'text-center defaultCursor', 'thStyle': 'cursor: default' }
                ],
            }
        },
        methods: {
            naumenLicenseCaclulate(arr) {
                if(arr) {
                    return arr.map(x => {
                        let percent = x.license_use / x.license_all * 100;
                        if (percent > 95) {
                            x._rowVariant = 'danger';
                            x._showDetails = true;
                        }
                        return x;
                    });
                }
            }
        }
    }
</script>

<style scoped>
    
</style>