import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store';
import ElementUI from 'element-ui';
import 'element-ui/lib/theme-chalk/index.css';
import $ajax from './ajax/axios';
let qs = require('qs');

Vue.config.productionTip = false

Vue.prototype.$ajax = $ajax;
Vue.prototype.$qs = qs;
Vue.prototype.imageLoading = function(id,model){
  let self = this;
  let delay = Math.random();
  let timer = setTimeout(function () {
    self.$set(model, id, true);
    clearTimeout(timer);
  }, 1000 * delay)
};
Vue.use(ElementUI);
new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
