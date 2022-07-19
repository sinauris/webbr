import Vue from 'vue'
import Router from 'vue-router'
import store from './store'

import error404_Page from './components/error404_page'

import Home_Page from './components/home_page'

import Dashboard_Page from './components/dashboard_page'

import Portmaps_Page from './components/portmaps_page'
import Portmaps_Page_Table from './components/portmap_addon/portmap_table'
import Portmaps_Page_Info from './components/portmap_addon/portmap_info'

import Servers_Page from './components/servers_page'

import Inventory_Page from './components/inventory_page'
import Inventory_Page_Table from './components/inventory_addon/inventory_table'

import Domain_Page from './components/domain_page'
import Domain_Page_Users_Table from './components/domain_addon/domain_users_table'
import Domain_Page_Computers_Table from './components/domain_addon/domain_computers_table'
import Domain_Page_Groups_Table from './components/domain_addon/domain_groups_table'

import Duty_Page from './components/duty_page'
import Duty_Page_Admin_Table from './components/duty_addon/duty_admin_table'

import News_Page from './components/news_page'
import News_Create from './components/news_addon/news_create'
import News_Edit from './components/news_addon/news_edit'
import News_Body from './components/news_addon/news_body'

import Configuration_Page from './components/configuration_page'

import Places_Page from './components/configuration_addon/places_page'

import Users_Page from './components/configuration_addon/users_page'

import Sw_Page from './components/configuration_addon/sw_page'
import Sw_Commutator_Table from './components/configuration_addon/sw_addon/commutator_table'


Vue.use(Router);

// const ifNotAuthenticated = (to, from, next) => {
//   if (!store.getters.isAuthenticated) {
//     next();
//     return
//   }
//   next('/')
// };

// const ifAuthenticated = (to, from, next) => {
//   if (store.getters.isAuthenticated) {
//     next();
//     return
//   }
//   next('/')
// };

const guestRedirect = (to, from, next) => {
    if (store.getters.isAuthenticated) {
        if (store.getters.getTokenRole === 'guest') {
            next('/dashboard');
            return
        }
        next();
    }
    next()
};

const guestPolicyAuth = (to, from, next) => {
    if (store.getters.isAuthenticated) {
        if (store.getters.getTokenRole === 'guest'
            || store.getters.getTokenRole === 'oit'
            || store.getters.getTokenRole === 'oaod'
            || store.getters.getTokenRole === 'otp'
            || store.getters.getTokenRole === 'mc'
            || store.getters.getTokenRole === 'rg'
            || store.getters.getTokenRole === 'admin') {
            next();
            return
        }
        next('/')
    }
    next('/')
};

const basicPolicyAuth = (to, from, next) => {
    if (store.getters.isAuthenticated) {
        if (store.getters.getTokenRole === 'oit'
            || store.getters.getTokenRole === 'oaod'
            || store.getters.getTokenRole === 'otp'
            || store.getters.getTokenRole === 'mc'
            || store.getters.getTokenRole === 'rg'
            || store.getters.getTokenRole === 'admin') {
            next();
            return
        }
        next('/')
    }
    next('/')
};

const otpOnlyPolicyAuth = (to, from, next) => {
    if (store.getters.isAuthenticated) {
        if (store.getters.getTokenRole === 'otp'
            || store.getters.getTokenRole === 'mc'
            || store.getters.getTokenRole === 'rg'
            || store.getters.getTokenRole === 'admin') {
            next();
            return
        }
        next('/')
    }
    next('/')
};

const mcPolicyAuth = (to, from, next) => {
    if (store.getters.isAuthenticated) {
        if (store.getters.getTokenRole === 'mc'
            || store.getters.getTokenRole === 'rg'
            || store.getters.getTokenRole === 'admin') {
            next();
            return
        }
        next('/')
    }
    next('/')
};

const rgPolicyAuth = (to, from, next) => {
    if (store.getters.isAuthenticated) {
        if (store.getters.getTokenRole === 'rg'
            || store.getters.getTokenRole === 'admin') {
            next();
            return
        }
        next('/')
    }
    next('/')
};

const adminPolicyAuth = (to, from, next) => {
    if (store.getters.isAuthenticated) {
        if (store.getters.getTokenRole === 'admin') {
            next();
            return
        }
        next('/')
    }
    next('/')
};

export default new Router({
    mode: 'history',
    
    routes: [

        { path: '/', name: 'Home', component: Home_Page, beforeEnter: guestRedirect },

        { path: '/dashboard', name: 'Dashboard', component: Dashboard_Page, beforeEnter: guestPolicyAuth },

        { path: '/portmaps', name: 'Portmaps', component: Portmaps_Page, beforeEnter: basicPolicyAuth,
            children: [
                { path: '/portmaps/info', name: 'portmapinfo', component: Portmaps_Page_Info },
                { path: '/portmaps/:id', name: 'portmapid', component: Portmaps_Page_Table }
            ]
        },

        { path: '/servers', name: 'Servers', component: Servers_Page, beforeEnter: basicPolicyAuth },

        { path: '/inventory', name: 'Inventory', component: Inventory_Page, beforeEnter: basicPolicyAuth,
            children: [
                { path: '/inventory/:id', name: 'inventoryid', component: Inventory_Page_Table }
            ]
        },

        { path: '/domain', name: 'Domain', component: Domain_Page, beforeEnter: basicPolicyAuth, redirect: '/domain/users',
            children: [
                { path: 'users', name: 'domainusers', component: Domain_Page_Users_Table },
                { path: 'computers', name: 'domaincomputers', component: Domain_Page_Computers_Table },
                { path: 'groups', name: 'domaingroups', component: Domain_Page_Groups_Table },
            ]
        },
        
        { path: '/duty', name: 'Duty', component: Duty_Page, beforeEnter: otpOnlyPolicyAuth,
            children: [
                { path: '/duty/:id', name: 'dutyid', component: Duty_Page_Admin_Table }
            ],
        },

        { path: '/news', name: 'News', component: News_Page, beforeEnter: basicPolicyAuth,
            children: [
                { path: '/news/create', name: 'NewsCreate', component: News_Create, beforeEnter: otpOnlyPolicyAuth },
                { path: '/news/:id', name: 'NewsBody', component: News_Body },
                { path: '/news/:id/edit', name: 'NewsEdit', component: News_Edit, beforeEnter: otpOnlyPolicyAuth }
            ]
        },

        { path: '/configuration', name: 'Configuration', component: Configuration_Page, beforeEnter: otpOnlyPolicyAuth,
            children: [
                { path: 'places', name: 'Places', component: Places_Page, beforeEnter: rgPolicyAuth },
                { path: 'users', name: 'Users', component: Users_Page, beforeEnter: rgPolicyAuth },
                { path: 'sw', name: 'Sw', component: Sw_Page,
                    children: [
                        { path: '/configuration/sw/:id', name: 'commutatorid', component: Sw_Commutator_Table }
                    ]
                }
            ]
        },


        // Маршрут для ошибки 404 (несуществующие URL)
        { path: '*', name: '404', component: error404_Page }
    ],
})
