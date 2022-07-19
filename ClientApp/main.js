import App from './App'
import Vue from 'vue'
import axios from 'axios'
import router from './router'
import store from './store'
import { sync } from 'vuex-router-sync'
import BootstrapVue from 'bootstrap-vue'
import jwtDecode from 'jwt-decode'
import VueCookies from 'vue-cookies'


import 'font-awesome/css/font-awesome.min.css'
import '../lib/froala-editor-self/css/froala_editor.pkgd.min.css'
import '../lib/froala-editor-self/css/froala_style.min.css'
import '../lib/froala-editor-self/js/froala_editor.pkgd.min.js'
import '../lib/froala-editor-self/js/languages/ru'

// import 'froala-editor/css/froala_editor.pkgd.min.css'
// import 'froala-editor/css/froala_style.min.css'
// import 'froala-editor/js/froala_editor.pkgd.min.js'
// import 'froala-editor/js/languages/ru'

import VueFroala from 'vue-froala-wysiwyg'

import { DateTime } from "luxon"


// Font Awesome 5 Core
import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

// FA5 BRANDS
import { faLinux } from '@fortawesome/free-brands-svg-icons/faLinux'
import { faWindows } from '@fortawesome/free-brands-svg-icons/faWindows'

// FA5 SOLID
import { faSync } from '@fortawesome/free-solid-svg-icons/faSync'
import { faUser } from '@fortawesome/free-solid-svg-icons/faUser'
import { faKey } from '@fortawesome/free-solid-svg-icons/faKey'
import { faTerminal } from '@fortawesome/free-solid-svg-icons/faTerminal'
import { faTv } from '@fortawesome/free-solid-svg-icons/faTv'
import { faFolder } from '@fortawesome/free-solid-svg-icons/faFolder'
import { faVolumeUp } from '@fortawesome/free-solid-svg-icons/faVolumeUp'
import { faComment } from '@fortawesome/free-solid-svg-icons/faComment'
import { faRedo } from '@fortawesome/free-solid-svg-icons/faRedo'
import { faPowerOff } from '@fortawesome/free-solid-svg-icons/faPowerOff'
import { faDownload } from '@fortawesome/free-solid-svg-icons/faDownload'
import { faSearch } from '@fortawesome/free-solid-svg-icons/faSearch'
import { faBolt } from '@fortawesome/free-solid-svg-icons/faBolt'
import { faPlusCircle } from '@fortawesome/free-solid-svg-icons/faPlusCircle'
import { faMinusCircle } from '@fortawesome/free-solid-svg-icons/faMinusCircle'
import { faRetweet } from '@fortawesome/free-solid-svg-icons/faRetweet'
import { faPen } from '@fortawesome/free-solid-svg-icons/faPen'
import { faPencilAlt } from '@fortawesome/free-solid-svg-icons/faPencilAlt'
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons/faTrashAlt'
import { faHeading } from '@fortawesome/free-solid-svg-icons/faHeading'
import { faCommentDots } from '@fortawesome/free-solid-svg-icons/faCommentDots'
import { faCog } from '@fortawesome/free-solid-svg-icons/faCog'
import { faCloud } from '@fortawesome/free-solid-svg-icons/faCloud'
import { faBell } from '@fortawesome/free-solid-svg-icons/faBell'
import { faCarBattery } from '@fortawesome/free-solid-svg-icons/faCarBattery'
import { faPlus } from '@fortawesome/free-solid-svg-icons/faPlus'
import { faChevronUp } from '@fortawesome/free-solid-svg-icons/faChevronUp'
import { faChevronDown } from '@fortawesome/free-solid-svg-icons/faChevronDown'
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons/faInfoCircle'
import { faLayerGroup } from '@fortawesome/free-solid-svg-icons/faLayerGroup'
import { faCircle } from '@fortawesome/free-solid-svg-icons/faCircle'
import { faSave } from '@fortawesome/free-solid-svg-icons/faSave'
import { faUniversity } from '@fortawesome/free-solid-svg-icons/faUniversity'
import { faGlobeEurope } from '@fortawesome/free-solid-svg-icons/faGlobeEurope'
import { faSun } from '@fortawesome/free-solid-svg-icons/faSun'
import { faMoon } from '@fortawesome/free-solid-svg-icons/faMoon'
import { faMicrochip } from '@fortawesome/free-solid-svg-icons/faMicrochip'
import { faMemory } from '@fortawesome/free-solid-svg-icons/faMemory'
import { faHdd } from '@fortawesome/free-solid-svg-icons/faHdd'

// FA5 REGULAR
import { faClock } from '@fortawesome/free-regular-svg-icons/faClock'
import { faQuestionCircle } from '@fortawesome/free-regular-svg-icons/faQuestionCircle'
import { faCheckSquare } from '@fortawesome/free-regular-svg-icons/faCheckSquare'
import { faTimesCircle } from '@fortawesome/free-regular-svg-icons/faTimesCircle'
import { faAddressBook } from '@fortawesome/free-regular-svg-icons/faAddressBook'



import { USER_TOKEN_NAME_ACTION, USER_TOKEN_LOGIN_ACTION, USER_TOKEN_ROLE_ACTION, USER_TOKEN_PLACE_ACTION, USER_TOKEN_THEME_ACTION } from './store/actions/user'

// Font Awesome 5 - add to library
library.add(faSync, faUser, faKey, faTerminal, faTv, faVolumeUp, faComment, faRedo, faPowerOff, faFolder, faDownload, faSearch, faBolt, faMinusCircle, faPlusCircle, faRetweet, faPen, faPencilAlt, faTrashAlt, faHeading, faCommentDots, faCog, faCloud, faBell, faCarBattery, faPlus, faChevronUp, faChevronDown, faInfoCircle, faLayerGroup, faCircle, faSave, faUniversity, faGlobeEurope, faSun, faMoon, 
    faMicrochip, faMemory, faHdd,  );
library.add(faWindows, faLinux);
library.add(faClock, faQuestionCircle, faCheckSquare, faTimesCircle, faAddressBook);


Vue.component(FontAwesomeIcon.name, FontAwesomeIcon);

Vue.use(BootstrapVue);
Vue.use(VueFroala);
Vue.use(VueCookies);


Vue.filter('truncate', function(text, length, clamp) {
    clamp = clamp || '...';
    var node = document.createElement('div');
    node.innerHTML = text;
    var content = node.textContent;
    return content.length > length ? content.slice(0, length) + clamp : content;
});

Vue.filter('capitalize', function (value) {
    if (!value) return '';
    value = value.toString();
    return value.charAt(0).toUpperCase() + value.slice(1)
});


let token = localStorage.getItem('webbr-token');
if (token) {
    let decodedToken = jwtDecode(token);
    store.dispatch(USER_TOKEN_NAME_ACTION, decodedToken.din);
    store.dispatch(USER_TOKEN_LOGIN_ACTION, decodedToken.sub);
    store.dispatch(USER_TOKEN_ROLE_ACTION, decodedToken.rol);
    store.dispatch(USER_TOKEN_PLACE_ACTION, decodedToken.plc);
    store.dispatch(USER_TOKEN_THEME_ACTION, decodedToken.thm);
}


Vue.config.devtools = false;
Vue.config.productionTip = false;

Vue.prototype.$dt = DateTime;
Vue.prototype.$http = axios;

sync(store, router);

axios.interceptors.request.use(function (config) {
    const token = localStorage.getItem('webbr-token');
    if (token != null) { config.headers.Authorization = `Bearer ${token}`; }
    return config;
}, function (err) { return Promise.reject(err); });

new Vue({
    el: '#app',
    router,
    store,
    template: '<App/>',
    components: { App },
});