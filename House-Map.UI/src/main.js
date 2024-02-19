import Vue from 'vue';
import App from './App.vue';
import router from './router';
import store from './store';
import ElementUI from 'element-ui';
import 'element-ui/lib/theme-chalk/index.css';
import { $ajax, $v2 } from "./ajax/ajax";
import VueLazyLoad from 'vue-lazyload';
Vue.use(VueLazyLoad);
let qs = require('qs');
Vue.config.productionTip = false;
Vue.prototype.$ajax = $ajax;
Vue.prototype.$v2 = $v2;
Vue.prototype.$qs = qs;
Vue.prototype.imageLoading = function (id, model) {
    let self = this;
    let delay = Math.random();
    let timer = setTimeout(function () {
        self.$set(model, id, true);
        clearTimeout(timer);
    }, 1000 * delay);
};
Vue.prototype.$transformData = function (time, fmt) {
    let date = new Date(time);
    let o = {
        "M+": date.getMonth() + 1,
        "d+": date.getDate(),
        "h+": date.getHours(),
        "m+": date.getMinutes(),
        "s+": date.getSeconds(),
        "q+": Math.floor((date.getMonth() + 3) / 3),
        "S": date.getMilliseconds() //毫秒
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
    let k;
    for (k in o) {
        if (o.hasOwnProperty(k) && new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    }
    return fmt;
};
Vue.use(ElementUI);
new Vue({
    router,
    store,
    render: h => h(App)
}).$mount('#app');
//# sourceMappingURL=main.js.map