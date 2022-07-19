<template>
    <div>
        <br/>
        <br/>
        <b-container fluid>
            <b-row>
                <b-col cols="12" sm="6" md="4" lg="3" offset-xl="1" xl="2">
                    <div class="mb-2" v-if="isOtpOnlyPolicy">
                        <b-row>
                            <b-col class="mr-1">
                                <b-btn block variant="primary" size="sm" @click="createNewsRoute"><font-awesome-icon class="mr-1" icon="plus" size="sm"/> Написать</b-btn>
                            </b-col>
                            <b-col class="ml-1">
                                <b-btn block v-if="!reloadNewsActive" variant="success" size="sm" @click="reloadNews"><font-awesome-icon class="mr-1" icon="sync" size="sm"/> Обновить</b-btn>
                                <b-btn block v-else variant="success" size="sm" disabled @click="reloadNews"><font-awesome-icon class="mr-1" icon="sync" size="sm" spin/> Обновить</b-btn>
                            </b-col>
                        </b-row>
                    </div>

                    <b-input-group size="sm" class="mb-3">
                        <b-input-group-text slot="prepend">
                            <font-awesome-icon icon="search"/>
                        </b-input-group-text>
                        <b-form-input
                            ref="searchInput"
                            type="text"
                            autofocus
                            v-model="search"
                            placeholder="Поиск"
                        />
                    </b-input-group>

                    <b-nav vertical>
                        <template v-for="ym in newsUniqueDateCompute">
                            <b class="mt-3 mb-1" style="cursor: default">{{ ym.year }} • {{ ym.monthLong | capitalize }}</b>
                            <b-nav-item
                                v-for="n in newsdata"
                                :key="n.id"
                                v-if="$dt.fromISO(n.createdtime).year === ym.year && $dt.fromISO(n.createdtime).month === ym.month && n.title.toLowerCase().includes(search.toLowerCase())"

                                :to="'/news/'+n.id"
                                active-class="activeNav"
                            >
                                <span class="text-muted">{{$dt.fromISO(n.createdtime).day}} {{ $dt.fromISO(n.createdtime).monthShort }}</span> {{n.title}}
                            </b-nav-item>
                        </template>

                    </b-nav>
                </b-col>
                <b-col cols="12" sm="6" md="6" lg="6" offset-xl="1" xl="6">
                    <router-view v-on:reloadNews="reloadNews" />
                </b-col>
            </b-row>

        </b-container>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex'
    import { USER_GET } from '../store/actions/user'


    export default {
        data() {
            return {
                search: '',
                newsdatamenulabel: [],
                newsdata: [],
                reloadNewsActive: false
            }
        },
        methods: {
            createNewsRoute() {
                this.$router.push('/news/create');
            },
            async reloadNews() {
                this.reloadNewsActive = true;
                await this.$store.dispatch(USER_GET, '/api/index/getnewstitle')
                    .then(async (resp) => {
                        this.newsdata = resp.data.reverse();
                    });
                this.reloadNewsActive = false;
            }
        },
        computed: {
            ...mapGetters(['isOtpOnlyPolicy']),
            filteredNews() {
                return this.newsdata.filter(n => {
                    return n.title.toLowerCase().indexOf(this.search.toLowerCase()) > -1
                })
            },

            newsUniqueDateCompute() {
                let dateArr;
                if (this.filteredNews.length !== 0) {
                    let newsArr = this.filteredNews;
                    let tempArr = [];

                    newsArr.forEach(x => {
                        let d = this.$dt.fromISO(x.createdtime);
                        tempArr.push({year: d.year, month: d.month, monthLong: d.monthLong });
                    });

                    return dateArr = tempArr.filter((thing, index, self) =>
                        index === self.findIndex((t) => (
                            t.year === thing.year && t.month === thing.month
                        ))
                    );
                }
            }
        },
        mounted() {
            this.$nextTick(() => this.$refs.searchInput.focus())
        },

        activated() {
            this.$nextTick(() => this.$refs.searchInput.focus())
        },
        async created() {
            this.reloadNews();
        }
    }
</script>

<style scoped>
    .activeNav {
        color: black;
        font-weight: bold;
        background-color: #eeeeee;
        border-left: 5px groove #f3f3f3;
    }

    .nav-link {
        padding-bottom: 2px;
        padding-top: 2px;
        font-size: 0.8rem;
    }
</style>
