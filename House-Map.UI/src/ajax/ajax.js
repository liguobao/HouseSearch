import axios from 'axios';
import router from './../router';
import Vue from 'vue';
import store from './../store';
const vue = new Vue();
axios.defaults.timeout = 30000;
axios.defaults.baseURL = 'https://house2048.cn/api/';
axios.defaults.headers['Content-Type'] = 'application/json;charset=utf-8';
const $ajax = axios.create({
    baseURL: 'https://house2048.cn/api/',
});
const $v2 = axios.create({
    baseURL: 'https://house2048.cn/api/v2/',
});
function defaultInterceptors(key) {
    let _key = key;
    _key.interceptors.request.use((config) => {
        if (localStorage.token) {
            config.headers.token = localStorage.getItem('token');
        }
        return config;
    });
    _key.interceptors.response.use((response) => {
        if (response.data.success || response.data.isSuccess || response.data.code != -1) {
            return response.data;
        }
        else {
            const message = response.data.error ? response.data.error : '请求出错';
            vue.$message.error(message);
            return Promise.reject(message);
        }
    }, (error) => {
        if (error.response) {
            switch (error.response.status) {
                case 401:
                    store.dispatch('UserLogout'); //可能是token过期，清除它
                    router.replace({
                        path: '/',
                        query: { redirect: router.currentRoute.fullPath } // 将跳转的路由path作为参数，登录成功后跳转到该路由
                    });
            }
        }
        const message = error.response.data.error ? error.response.data.error : '请求出错';
        vue.$message.error(message);
        // router.replace({
        //   path: '/'
        // });
        return Promise.reject(error.response);
    });
}
defaultInterceptors($ajax);
export { $ajax };
//# sourceMappingURL=ajax.js.map