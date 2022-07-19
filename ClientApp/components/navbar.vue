<template>
    <b-navbar toggleable="md" type="dark" variant="dark">
        <b-navbar-toggle target="nav_collapse"/>
        <b-navbar-brand to="/"><b>Webbr</b></b-navbar-brand>
        <b-collapse is-nav id="nav_collapse">
            <b-navbar-nav v-if="isAuthenticated">
                <b-nav-item active-class="active" to="/dashboard" v-if="!isGuestPolicy">Дашборд</b-nav-item>
                <b-nav-item active-class="active" :to="getPortmapNav" v-if="!isGuestPolicy">Карты</b-nav-item>
                <b-nav-item active-class="active" to="/servers" v-if="!isGuestPolicy">Серверы</b-nav-item>
                <b-nav-item active-class="active" :to="getInventoryNav" v-if="!isGuestPolicy">Инвентаризация</b-nav-item>
                <b-nav-item active-class="active" :to="getDomainNav" v-if="!isGuestPolicy">Домен</b-nav-item>
<!--                <b-nav-item active-class="active" to="/naumen" v-if="!isGuestPolicy">Наумен</b-nav-item>-->
                <b-nav-item active-class="active" :to="isRgPolicy ? getDutyNav : '/duty'" v-if="isOtpOnlyPolicy && !isGuestPolicy">Дежурства</b-nav-item>
                <b-nav-item active-class="active" to="/news" v-if="!isGuestPolicy">Новости</b-nav-item>
            </b-navbar-nav>
            <b-navbar-nav class="ml-auto" v-if="isAuthenticated">

                <div style="display: block; padding: 0.5rem; color: rgba(255, 255, 255, 0.5)">
                    <span v-if="theme === 'dark'" style="cursor: pointer; width: 125px" @click="changeTheme">
                        <font-awesome-icon icon="moon" size="lg" /> Тёмная тема
                    </span>

                    <span v-if="theme === 'light'" style="cursor: pointer; width: 125px" @click="changeTheme">
                        <font-awesome-icon icon="sun" size="lg" /> Светлая тема
                    </span>
                </div>
                
                <b-nav-item active-class="active" to="/configuration" v-if="isOtpOnlyPolicy && !isGuestPolicy">Настройки</b-nav-item>
                <b-nav-item class="mr-2" active-class="active" @click="logout">
                    Выход ({{ getTokenLogin }})
                </b-nav-item>
            </b-navbar-nav>
        </b-collapse>
    </b-navbar>
</template>

<script>
    import { mapGetters } from 'vuex'
    import { AUTH_LOGOUT } from '../store/actions/auth'

    export default {
        data() {
            return {
                theme: ''
            };
        },
        methods: {
            async logout() {
                await this.$store.dispatch(AUTH_LOGOUT).then(() => this.$router.push('/'));
            },
            changeTheme() {
                let webbrThemeElem = document.getElementById('webbr_theme');
                if(webbrThemeElem.href.includes('bootstrapLight')) {
                    webbrThemeElem.href = '/dist/bootstrapDark.css';
                    this.theme = 'dark';
                    this.$cookies.remove("webbrTheme");
                    this.$cookies.set("webbrTheme", 'dark', -1);
                }
                else if(webbrThemeElem.href.includes('bootstrapDark')) {
                    webbrThemeElem.href = '/dist/bootstrapLight.css';
                    this.theme = 'light';
                    this.$cookies.remove("webbrTheme");
                    this.$cookies.set("webbrTheme", 'light', -1);
                }
            },
        },
        computed: {
            ...mapGetters(['isAuthenticated', 'isGuestPolicy', 'isRgPolicy', 'isOtpOnlyPolicy', 'getTokenPlace', 'getTokenLogin', 'getPortmapNav', 'getInventoryNav', 'getDutyNav', 'getDomainNav'])
        },
        created() {
            this.theme = this.$cookies.get('webbrTheme');
        }
    }
</script>

<style>
    .navbar, .navbar-dark, .bg-dark {
        padding: 0 8px !important
    }
</style>