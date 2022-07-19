import { SIGNALR_START, SIGNALR_STOP} from '../actions/signalr'
import { AUTH_LOGOUT } from '../actions/auth'
import Vue from 'vue'

const signalrLib = require('@aspnet/signalr');
let signalrCore = new signalrLib.HubConnectionBuilder().withUrl("/webbrhub", { accessTokenFactory: () => localStorage.getItem('webbr-token') }).configureLogging(signalrLib.LogLevel.None).build();
Vue.prototype.$signalR = signalrCore;

const state = {
    connected: false
};

const getters = {
    getSignalrState: state => state.connected,
};

const actions = {
    [SIGNALR_START]: ({commit, dispatch}) => {

        let start = (async () => {
            try {
                await signalrCore.start();
                commit(SIGNALR_START);
            } catch (err) {
                setTimeout(() => start(), 10000);
            }
        });

        if (!state.connected) {
            start();
            commit(SIGNALR_START);
            signalrCore.onclose(() => {
                commit(SIGNALR_STOP);
                start();
            });
        }
    },

    [SIGNALR_STOP]: ({commit, dispatch}) => {
        signalrCore.stop();
        commit(SIGNALR_STOP);
    }
};

const mutations = {
    [SIGNALR_START]: (state) => state.connected = true,
    [SIGNALR_STOP]: (state) => state.connected = false
};

export default {
    state,
    getters,
    actions,
    mutations,
}