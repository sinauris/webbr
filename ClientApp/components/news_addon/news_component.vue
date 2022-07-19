<template>
    <div>
        <div v-if="item.length !== 0" class="mb-4 d-flex align-items-center" style="cursor: default">
            <h2>Недавние новости</h2>
            <b-btn v-if="isOtpOnlyPolicy" class="ml-3" variant="primary" size="sm" @click="createNewsRoute"><font-awesome-icon class="mr-1" icon="plus" size="sm"/> Написать</b-btn>
        </div>

        <div v-for="i in item" :key="i.id" class="mb-5" style="cursor: default">
            <b-link :to="'/news/'+i.id"><h5 class="mb-0">{{i.title}}</h5></b-link>

            <small class="text-muted">
                <span>{{i.author}}</span>
                <span class="ml-1 mr-1">•</span>
                {{ $dt.fromISO(i.createdtime).toRelative() }}
            </small>
            <small v-if="i.changetime" class="text-muted">
                <span class="ml-1 mr-1">•</span>
                <font-awesome-icon icon="pen" size="xs"/> {{ $dt.fromISO(i.changetime).toRelative() }}
            </small>

            <div class="p-0 m-0 mt-3">
                <froalaView v-if="i.body.length <= 1000" v-model="i.body"/>
                <div v-else class="mask text-center">
                    <h3 class="p-4" style="cursor: default">Текст новости слишком велик</h3>
                    <b-link :to="'/news/'+i.id"><h5>Перейти к новости</h5></b-link>
                </div>
            </div>
        </div>
        <br/>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex'
    import { USER_GET } from '../../store/actions/user'

    export default {
        data() {
            return {
                item: []
            }
        },

        methods: {
            async reloadNews() {
                await this.$store.dispatch(USER_GET, '/api/index/getlatestnews').then((resp) => this.item = resp.data);
            },

            createNewsRoute() {
                this.$router.push('/news/create');
            }
        },

        mounted () {
            this.$signalR.on('newsUpdate', () => this.reloadNews());
        },

        computed: {
            ...mapGetters(['isOtpOnlyPolicy'])
        },

        async created() {
            await this.reloadNews();
        }
    }
</script>

<style scoped>

</style>
