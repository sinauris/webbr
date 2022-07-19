<template>
    <div>
        <br/>
        <b-nav tabs align="center">
            <b-nav-item active-class="active" v-for="p in getPlace" :key="p.placeid" @click="changeRoute('/inventory/' + p.place_short_description)" :to="'/inventory/' + p.place_short_description">{{ p.place_description }}</b-nav-item>
        </b-nav>
        
        <keep-alive>
            <router-view />
        </keep-alive>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex'
    import { USER_INVENTORY_NAV_ACTION } from '../store/actions/user'

    export default {
        async mounted() {
            await this.$store.dispatch(USER_INVENTORY_NAV_ACTION, this.$route.path);
        },
        methods: {
            changeRoute(route) {
                this.$store.dispatch(USER_INVENTORY_NAV_ACTION, route);
            }
        },
        computed: {
            ...mapGetters(['getPlace']),
        }
    }
</script>

<style scoped>
    .nav {
        justify-content: center;
    }
</style>