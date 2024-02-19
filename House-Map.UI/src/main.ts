import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import ElementUI from 'element-ui';
import 'element-ui/lib/theme-chalk/index.css';
import {$ajax} from "./ajax/ajax";
import VueLazyLoad from 'vue-lazyload'

Vue.use(VueLazyLoad);
let qs = require('qs');

Vue.config.productionTip = false

Vue.prototype.$ajax = $ajax;
Vue.prototype.$qs = qs;
Vue.prototype.imageLoading = function (id: any, model: any) {
  let self = this;
  let delay = Math.random();
  let timer = setTimeout(function () {
    self.$set(model, id, true);
    clearTimeout(timer);
  }, 1000 * delay)
};
Vue.prototype.$transformData = function (time: any, fmt: any) {
  let date = new Date(time);
  type DateTypeKeys = "M+" | "d+" | "h+" | "m+" | "s+" | "q+" | "S"
  type DateType = {
    [key in DateTypeKeys]: number
  }

  let o: DateType = {
    "M+": date.getMonth() + 1,                 //月份
    "d+": date.getDate(),                    //日
    "h+": date.getHours(),                   //小时
    "m+": date.getMinutes(),                 //分
    "s+": date.getSeconds(),                 //秒
    "q+": Math.floor((date.getMonth() + 3) / 3), //季度
    "S": date.getMilliseconds()             //毫秒
  };

  if (/(y+)/.test(fmt))
    fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
  let k: DateTypeKeys;
  for (k in o) {
    if (o.hasOwnProperty(k) && new RegExp("(" + k + ")").test(fmt))
      fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
  }
  return fmt
};
Vue.use(ElementUI);
new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
