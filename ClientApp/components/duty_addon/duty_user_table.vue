<template>
    <b-table
        responsive
        :bordered="true"
        :hover="true"
        :items="userTable"
        :fields="fields"
        :sort-by.sync="sortBy"
        :sort-desc.sync="sortDesc"
    >

        <template slot="balance" slot-scope="row">
            {{row.value}} руб.
        </template>

        <template slot="balance2" slot-scope="row" v-if="row.value">
            {{row.value}} руб.
        </template>

        <template slot="comment" slot-scope="row">
            <div v-if="row.value && row.value.length > 35" style="cursor:pointer;" :id="'commentPop-'+row.item.id">
                {{row.value | truncate(35, '...')}}
            </div>
            <b-popover v-if="row.value && row.value.length > 35" :target="'commentPop-'+row.item.id" placement="top" triggers="click blur hover">
                {{row.item.comment}}
            </b-popover>
            <template v-else>
                {{row.value}}
            </template>
        </template>

        <template slot="status" slot-scope="row">
            {{row.value}}
        </template>

        <template slot="date" slot-scope="row">
            <template v-if="timeConstruct(row.value)">
                <abbr :title="$dt.fromISO(row.value)">{{ $dt.fromISO(row.value).toRelative() }}</abbr>
            </template>
            <template v-else>
                {{ row.value }}
            </template>
        </template>

    </b-table>
</template>

<script>
    export default {
        props: ['userTable'],
        data() {
            return {
                items: [],
                fields: [
                    { key: 'name', label: 'ФИО', sortable: true, 'thStyle': 'width:160px;', 'class': 'text-nowrap' },
                    { key: 'stpinc', label: 'Инциденты', sortable: true, 'thStyle': 'width:170px;', 'class': 'text-nowrap' },
                    { key: 'balance', label: 'Баланс', sortable: true, 'thStyle': 'width:100px;', 'class': 'text-nowrap' },
                    { key: 'balance2', label: 'Баланс 2', sortable: true, 'thStyle': 'width:100px;', 'class': 'text-nowrap' },
                    { key: 'comment', label: 'Комментарий', sortable: true, 'thStyle': 'width:270px;', 'class': 'text-nowrap' },
                    { key: 'status', label: 'Статус', sortable: true, 'thStyle': 'width:200px;', 'class': 'text-nowrap' },
                    { key: 'date', label: 'Дата', sortable: true, 'thStyle': 'width:140px;', 'class': 'text-nowrap' }
                ],
                sortBy: null,
                sortDesc: false
            }
        },
        methods: {
            timeConstruct(time) {
                if(this.$dt.fromISO(time).isValid) return true
            }
        }
    }
</script>

<style scoped>

</style>
