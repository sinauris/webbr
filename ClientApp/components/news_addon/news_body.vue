<template>
    <div v-if="item">
        <div style="cursor: default">
            <h4 class="mb-0">{{item[0].title}}</h4>
            <small class="text-muted">
                <span>{{item[0].author}}</span>
                <span class="ml-1 mr-1">•</span>
                {{ $dt.fromISO(item[0].createdtime).toRelative() }}
            </small>
            <small v-if="item[0].changetime" class="text-muted">
                <span class="ml-1 mr-1">•</span>
                <font-awesome-icon icon="pen" size="xs"/> {{ $dt.fromISO(item[0].changetime).toRelative() }}
            </small>
        </div>

        <hr/>

        <div class="p-0 m-0 mt-3">
            <froalaView v-model="item[0].body"/>
        </div>

        <div class="mt-4" v-if="getTokenLogin === item[0].adLogin || getTokenRole === 'admin' || getTokenRole === 'rg'">
            <hr/>
            <div class="float-right">
                <b-btn size="sm" variant="outline-danger" @click="removeNews">Удалить</b-btn>
                <b-btn class="ml-3" size="sm" variant="primary" @click="editThisNews"><font-awesome-icon class="mr-1" icon="pen" size="sm"/> Редактировать</b-btn>
            </div>
        </div>

        <br/>
    </div>
    <div v-else>
        <div class="text-center">
            <b-spinner style="width: 20rem; height: 20rem" type="grow" />
        </div>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex'
    import { USER_GET, USER_POST } from '../../store/actions/user'

    export default {
        data() {
            return {
                item: null,
                routeNewsId: null,
            }
        },
        methods: {
            editThisNews() {
                this.$router.push('/news/' + this.$route.params.id + '/edit');
            },

            async loadNews() {
                this.routeNewsId = this.$route.params.id;
                this.item = null;
                if (this.routeNewsId) {
                    await this.$store.dispatch(USER_GET, '/api/index/getonenews?id=' + this.routeNewsId)
                        .then((resp) => {
                            this.item = resp.data;
                        });
                }
            },

            async removeNews() {
                let acceptDelete = confirm('Подтверждаете удаление?');
                if (acceptDelete)
                {
                    let id = this.$route.params.id;

                    await this.$store.dispatch(USER_POST, {
                        url: '/api/index/deletenews',
                        id: id
                    })
                        .then(async () => {
                            this.$emit('reloadNews');
                            this.$router.push('/news/');
                        });
                }
            }
        },

        computed: {
            ...mapGetters(['isMcPolicy', 'isRgPolicy', 'getTokenLogin', 'getTokenRole']),
        },

        watch: {
            '$route'() {
                if (this.$route.name === 'NewsBody' && this.$route.params.id) this.loadNews()
            }
        },

        mounted () {
            this.$signalR.on('newsUpdate', () => this.loadNews());
        },

        async created() {
            await this.loadNews();
        },
    }
</script>

<style scoped>

</style>
