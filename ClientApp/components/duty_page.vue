<template>
    <div v-if="isRgPolicy">
        <br/>
        <b-nav tabs align="center">
            <b-nav-item active-class="active" v-for="p in getPlace" :key="p.placeid" @click="changeRoute('/duty/' + p.place_short_description)" :to="'/duty/' + p.place_short_description">{{ p.place_description }}</b-nav-item>
        </b-nav>
        <keep-alive>
            <router-view />
        </keep-alive>
    </div>

    <div v-else-if="isOtpOnlyPolicy">
        <duty_user />
    </div>
</template>

<script>
    import { USER_DUTY_NAV_ACTION } from '../store/actions/user'
    import { mapGetters } from 'vuex'
    import duty_user from './duty_addon/duty_user'

    export default {
        components: {
            duty_user
        },
        computed: {
            ...mapGetters(['isOtpOnlyPolicy', 'isRgPolicy', 'getPlace']),
        },
        async mounted() {
            await this.$store.dispatch(USER_DUTY_NAV_ACTION, this.$route.path);
        },
        methods: {
            changeRoute(route) {
                this.$store.dispatch(USER_DUTY_NAV_ACTION, route);
            }
        }
    }
</script>

<style scoped>
    .nav {
        justify-content: center;
    }
</style>