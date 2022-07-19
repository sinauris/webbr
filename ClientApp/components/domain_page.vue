<template>
    <div>
        <br/>
        <b-nav tabs>
            <b-nav-item active-class="active" @click="changeRoute('/domain/users')" to="/domain/users">Пользователи</b-nav-item>
            <b-nav-item active-class="active" @click="changeRoute('/domain/computers')" to="/domain/computers">Компьютеры</b-nav-item>
            <b-nav-item active-class="active" @click="changeRoute('/domain/groups')" to="/domain/groups">Группы</b-nav-item>
        </b-nav>
        
        <keep-alive>
            <router-view />
        </keep-alive>
    </div>
</template>

<script>
    import { USER_DOMAIN_NAV_ACTION } from '../store/actions/user'
    import { mapGetters } from 'vuex'

    export default {
        computed: {
            ...mapGetters(['isOtpOnlyPolicy', 'isRgPolicy']),
        },
        async mounted() {
            await this.$store.dispatch(USER_DOMAIN_NAV_ACTION, this.$route.path);
        },
        methods: {
            changeRoute(route) {
                this.$store.dispatch(USER_DOMAIN_NAV_ACTION, route);
            }
        }
    }
</script>

<style scoped>
    .nav {
        justify-content: center;
    }
</style>