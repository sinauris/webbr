<template>
    <div class="d-flex justify-content-center">
        <b-form @submit.prevent="auth" style="width: 250px;">
            <b-form-group description="Логин или Почта от Active Directory">
                <b-input-group size="sm">
                    <b-input-group-prepend>
                        <b-btn variant="outline-secondary" style="min-width: 35px" disabled><font-awesome-icon icon="user"/></b-btn>
                    </b-input-group-prepend>
                    <b-form-input autofocus
                                  type="text"
                                  v-model="authForm.username"
                                  required
                                  placeholder="Логин">
                    </b-form-input>
                </b-input-group>
            </b-form-group>
            <b-form-group>
                <b-input-group size="sm">
                    <b-input-group-prepend>
                        <b-button variant="outline-secondary" style="min-width: 35px" disabled><font-awesome-icon icon="key"/></b-button>
                    </b-input-group-prepend>
                    <b-form-input type="password"
                                  v-model="authForm.password"
                                  required
                                  placeholder="Пароль">
                    </b-form-input>
                </b-input-group>
            </b-form-group>

            <b-btn
                block
                v-if="authLoading"
                size="sm"
                class="align mt-4"
                type="submit"
                variant="secondary"
                disabled>
                <font-awesome-icon icon="cog" spin/>
            </b-btn>

            <b-btn
                block
                v-else
                size="sm mt-4"
                class="align font-weight-bold"
                type="submit"
                variant="primary">Войти</b-btn>

            <div class="mt-2 text-center text-danger" style="font-size: 13px;">{{authError}}</div>
        </b-form>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex'
    import jwtDecode from 'jwt-decode'
    import { AUTH_REQUEST } from '../../store/actions/auth'
    import {
        USER_GET,
        USER_PLACE_ACTION,
        USER_TOKEN_NAME_ACTION,
        USER_TOKEN_LOGIN_ACTION,
        USER_TOKEN_ROLE_ACTION,
        USER_TOKEN_PLACE_ACTION,
        USER_TOKEN_THEME_ACTION,
        USER_PORTMAP_NAV_ACTION,
        USER_INVENTORY_NAV_ACTION,
        USER_DOMAIN_NAV_ACTION,
        USER_DUTY_NAV_ACTION } from '../../store/actions/user'

    export default {
        data () {
            return {
                authLoading: false,
                authAlert: false,
                authError: '',

                authForm: {
                    username: '',
                    password: '',
                    session: 'not_accepted'
                }
            }
        },
        computed: {
            ...mapGetters(['getTokenPlace', 'isGuestPolicy', 'isRgPolicy', 'getPlace']),
        },
        methods: {
            async auth() {

                this.authLoading = true;

                const username = this.authForm.username;
                const password = this.authForm.password;
                const session = this.authForm.session;

                await this.$store.dispatch(AUTH_REQUEST, { username, password, session })
                    .then(async () => {

                        let token = localStorage.getItem('webbr-token');
                        if(token) {
                            let decodedToken = jwtDecode(token);
                            await this.$store.dispatch(USER_TOKEN_NAME_ACTION, decodedToken.din);
                            await this.$store.dispatch(USER_TOKEN_LOGIN_ACTION, decodedToken.sub);
                            await this.$store.dispatch(USER_TOKEN_ROLE_ACTION, decodedToken.rol);
                            await this.$store.dispatch(USER_TOKEN_PLACE_ACTION, decodedToken.plc);
                            await this.$store.dispatch(USER_TOKEN_THEME_ACTION, decodedToken.thm);
                        }
                        
                        await this.$store.dispatch(USER_GET, '/api/configuration/getavailableplaces')
                            .then(async (resp) => {
                                    this.$store.dispatch(USER_PLACE_ACTION, resp.data);

                                    let pl = resp.data.filter(a => a.place_description === this.getTokenPlace);
                                    if (pl.length !== 0) {
                                        let shortDescr = pl[0].place_short_description;
                                        await this.$store.dispatch(USER_PORTMAP_NAV_ACTION, '/portmaps/' + shortDescr);
                                        await this.$store.dispatch(USER_INVENTORY_NAV_ACTION, '/inventory/' + shortDescr);
                                        if (this.isRgPolicy) await this.$store.dispatch(USER_DUTY_NAV_ACTION, '/duty/' + shortDescr);
                                        else await this.$store.dispatch(USER_DUTY_NAV_ACTION, '/duty');
                                    }
                                    else {
                                        await this.$store.dispatch(USER_PORTMAP_NAV_ACTION, '/portmaps');
                                        await this.$store.dispatch(USER_INVENTORY_NAV_ACTION, '/inventory');
                                        await this.$store.dispatch(USER_DUTY_NAV_ACTION, '/duty');
                                    }
                                    await this.$store.dispatch(USER_DOMAIN_NAV_ACTION, '/domain');
                                }
                            );


                        if(!this.isGuestPolicy) this.$router.push('/');
                        else this.$router.push('/dashboard');
                    })
                    .catch(err => {
                        this.authAlert = true;
                        this.authLoading = false;
                        try {
                            this.authError = Object.values(err.response.data)[0].toString();
                        }
                        catch (e) {

                        }
                    });
            }
        },
    }
</script>
