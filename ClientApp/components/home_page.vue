<template>
    <div>
        <b-jumbotron class="text-center pt-6 pb-5" style="cursor: default">
            <template slot="header">
                <kbd class="noselect" style="padding-left: 0.5em; letter-spacing: 20px; color: #e9ecef">{{ text }}</kbd>
            </template>
            <template slot="lead">
                    <span class="noselect">Сайт, который умеет <abbr title="(нет)">всё</abbr>.</span>
            </template>
        </b-jumbotron>


        <b-container fluid v-if="isAuthenticated">
            <b-row>
                <b-col cols="12" sm="6" md="4" lg="3" offset-xl="1" xl="2">
                    <home_index/>
                </b-col>
                <b-col cols="12" sm="6" md="6" lg="6" offset-xl="1" xl="4">
                    <news_component/>
                </b-col>
            </b-row>
        </b-container>
        <div v-else-if="!isAuthenticated">
            <home_auth/>
        </div>
    </div>
</template>


<script>
    import home_auth from './home_addon/home_auth'
    import home_index from './home_addon/home_index'
    import news_component from './news_addon/news_component'
    import { mapGetters } from 'vuex'
    export default {
        data() {
            return { 
                webbrText: 'WEBBR',
                text: ''
            }
        },
        components: {
            home_auth,
            news_component,
            home_index
        },
        computed: {
            ...mapGetters(['isAuthenticated']),
            
        },
        methods: {
            computedText() {
                let index = 0;
                let interval = setInterval(() => {
                    index = (index % this.webbrText.length) + 1;
                    this.text = this.webbrText.slice(0, index);
                    if(index === this.webbrText.length) clearInterval(interval)
                }, 250);
            }
        },
        created() {
            this.computedText();
        }
    }
</script>


<style>
    ul { font-size: 14px }
    .content li+li { margin-top: 0 }
</style>
