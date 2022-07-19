import { AUTH_REQUEST, AUTH_ERROR, AUTH_SUCCESS, AUTH_LOGOUT } from '../actions/auth'
import { SIGNALR_START, SIGNALR_STOP } from '../actions/signalr'
import axios from 'axios'

const state = { token: localStorage.getItem('webbr-token') || '', };

const getters = {
  isAuthenticated: state => !!state.token,
};

const actions = {
    [AUTH_REQUEST]: ({commit, dispatch}, user) => {
        return new Promise((resolve, reject) => {

        commit(AUTH_REQUEST);
        axios.post('auth/token', user)
            .then(resp => {
                let token = resp.data.auth_token;

                localStorage.setItem('webbr-token', token);

                axios.defaults.headers.common['Authorization'] = 'Bearer ' + token;

                commit(AUTH_SUCCESS, token);
                dispatch(SIGNALR_START);
                resolve(token);
            })
            .catch(err => {
                commit(AUTH_ERROR, err);
                localStorage.removeItem('webbr-token');
                reject(err)
            })
        })
    },
    [AUTH_LOGOUT]: ({commit, dispatch}) => {
        return new Promise((resolve, reject) => {
            commit(AUTH_LOGOUT);
            localStorage.removeItem('webbr-token');
            dispatch(SIGNALR_STOP);
            resolve()
        })
    }
};

const mutations = {
    [AUTH_REQUEST]: (state) => {
        //state.status = 'loading'
    },
    [AUTH_SUCCESS]: (state, resp) => {
        //state.status = 'success';
        state.token = resp;
        //state.hasLoadedOnce = true
    },
    [AUTH_ERROR]: (state) => {
        //state.status = 'error';
        //state.hasLoadedOnce = true
    },
    [AUTH_LOGOUT]: (state) => {
        state.token = '';
    }
};

export default {
  state,
  getters,
  actions,
  mutations,
}
