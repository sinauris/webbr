import { USER_REQUEST, USER_GET, USER_POST, USER_ERROR, USER_SUCCESS } from '../actions/user'

import { USER_PLACE_ACTION, USER_PLACE_SAVE } from '../actions/user'
import { USER_PORTMAP_NAV_ACTION, USER_PORTMAP_NAV_SAVE } from '../actions/user'
import { USER_INVENTORY_NAV_ACTION, USER_INVENTORY_NAV_SAVE } from '../actions/user'
import { USER_DOMAIN_NAV_ACTION, USER_DOMAIN_NAV_SAVE } from '../actions/user'
import { USER_DUTY_NAV_ACTION, USER_DUTY_NAV_SAVE } from '../actions/user'

import { USER_TOKEN_NAME_ACTION, USER_TOKEN_LOGIN_ACTION, USER_TOKEN_ROLE_ACTION, USER_TOKEN_PLACE_ACTION, USER_TOKEN_THEME_ACTION } from '../actions/user'
import { USER_TOKEN_NAME_SAVE, USER_TOKEN_LOGIN_SAVE, USER_TOKEN_ROLE_SAVE, USER_TOKEN_PLACE_SAVE, USER_TOKEN_THEME_SAVE } from '../actions/user'

import {AUTH_ERROR, AUTH_LOGOUT} from '../actions/auth'
import axios from 'axios'
import router from '../../router'


const state = {
    getPlaceSave: '',

    getPortmapNavSave: '',
    getInventoryNavSave: '',
    getDomainNavSave: '',
    getDutyNavSave: '',

    getPortmap: [],

    tokenNameSave: '',
    tokenLoginSave: '',
    tokenRoleSave: '',
    tokenPlaceSave: '',
    tokenThemeSave: ''
};

const getters = {
    getPlace: state => state.getPlaceSave,

    getPortmapNav: state => state.getPortmapNavSave,

    getInventoryNav: state => state.getInventoryNavSave,

    getDomainNav: state => state.getDomainNavSave,

    getDutyNav: state => state.getDutyNavSave,


    getTokenName: state => state.tokenNameSave,
    getTokenLogin: state => state.tokenLoginSave,
    getTokenRole: state => state.tokenRoleSave,
    getTokenPlace: state => state.tokenPlaceSave,
    getTokenTheme: state => state.tokenThemeSave,

    isGuestPolicy: state => {
        if (state.tokenRoleSave === 'guest') return true;
    },

    isBasicPolicy: state => {
        if (state.tokenRoleSave === 'oit' || state.tokenRoleSave === 'oaod' || state.tokenRoleSave === 'otp' || state.tokenRoleSave === 'mc' || state.tokenRoleSave === 'rg' || state.tokenRoleSave === 'admin') return true;
    },

    isOtpOnlyPolicy: state => {
        if (state.tokenRoleSave === 'otp' || state.tokenRoleSave === 'mc' || state.tokenRoleSave === 'rg' || state.tokenRoleSave === 'admin') return true;
    },

    isMcPolicy: state => {
        if (state.tokenRoleSave === 'mc' || state.tokenRoleSave === 'rg' || state.tokenRoleSave === 'admin') return true;
    },

    isRgPolicy: state => {
        if (state.tokenRoleSave === 'rg' || state.tokenRoleSave === 'admin') return true;
    },

    isAdminPolicy: state => {
        if (state.tokenRoleSave === 'admin') return true;
    },
};

const actions = {
    [USER_GET]: ({commit, dispatch}, url) => {
        return new Promise(async (resolve, reject) => {
            await axios.get(url)
                .then(resp => {
                    resolve(resp)
                })
                .catch(err => {
                    if (err.response) {
                        if (err.response.status === 401) {
                            commit(AUTH_ERROR, err);
                            dispatch(AUTH_LOGOUT);
                            reject(err);

                            router.push('/');
                        }
                        reject(err)
                    }
                })
        })
    },
    [USER_POST]: ({commit, dispatch}, data) => {
        return new Promise(async (resolve, reject) => {
            await axios.post(data.url, data)
                .then(resp => resolve(resp))
                .catch(err => {
                    if (err.response.status === 401) {
                        commit(AUTH_ERROR, err);
                        dispatch(AUTH_LOGOUT);
                        reject(err);

                        router.push('/');
                    }
                    reject(err)
                })
        })
    },

    [USER_PLACE_ACTION]: ({commit, dispatch}, data) => { commit(USER_PLACE_SAVE, data); },
    [USER_PORTMAP_NAV_ACTION]: ({commit, dispatch}, data) => { commit(USER_PORTMAP_NAV_SAVE, data); },
    [USER_INVENTORY_NAV_ACTION]: ({commit, dispatch}, data) => { commit(USER_INVENTORY_NAV_SAVE, data); },
    [USER_DOMAIN_NAV_ACTION]: ({commit, dispatch}, data) => { commit(USER_DOMAIN_NAV_SAVE, data); },
    [USER_DUTY_NAV_ACTION]: ({commit, dispatch}, data) => { commit(USER_DUTY_NAV_SAVE, data); },

    [USER_TOKEN_NAME_ACTION]: ({commit, dispatch}, data) => { commit(USER_TOKEN_NAME_SAVE, data); },
    [USER_TOKEN_LOGIN_ACTION]: ({commit, dispatch}, data) => { commit(USER_TOKEN_LOGIN_SAVE, data); },
    [USER_TOKEN_ROLE_ACTION]: ({commit, dispatch}, data) => { commit(USER_TOKEN_ROLE_SAVE, data); },
    [USER_TOKEN_PLACE_ACTION]: ({commit, dispatch}, data) => { commit(USER_TOKEN_PLACE_SAVE, data); },
    [USER_TOKEN_THEME_ACTION]: ({commit, dispatch}, data) => { commit(USER_TOKEN_THEME_SAVE, data); }
};

const mutations = {
  [USER_REQUEST]: (state) => {
    //state.status = 'loading'
  },
  [USER_SUCCESS]: (state, resp) => {
    //state.status = 'success'
    //Vue.set(state, 'profile', resp)
  },
  [USER_ERROR]: (state) => {
    //state.status = 'error'
  },
  [AUTH_LOGOUT]: (state) => {
    //state.profile = {}
  },
    [USER_PLACE_SAVE]: (state, data) => { state.getPlaceSave = data },
    [USER_PORTMAP_NAV_SAVE]: (state, data) => { state.getPortmapNavSave = data },
    [USER_INVENTORY_NAV_SAVE]: (state, data) => { state.getInventoryNavSave = data },
    [USER_DOMAIN_NAV_SAVE]: (state, data) => { state.getDomainNavSave = data },
    [USER_DUTY_NAV_SAVE]: (state, data) => { state.getDutyNavSave = data },

    [USER_TOKEN_NAME_SAVE]: (state, data) => { state.tokenNameSave = data },
    [USER_TOKEN_LOGIN_SAVE]: (state, data) => { state.tokenLoginSave = data },
    [USER_TOKEN_ROLE_SAVE]: (state, data) => { state.tokenRoleSave = data },
    [USER_TOKEN_PLACE_SAVE]: (state, data) => { state.tokenPlaceSave = data },
    [USER_TOKEN_THEME_SAVE]: (state, data) => { state.tokenThemeSave = data },
};

export default {
  state,
  getters,
  actions,
  mutations,
}
