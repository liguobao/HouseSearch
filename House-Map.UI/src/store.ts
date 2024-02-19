import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)


let ua = navigator.userAgent;
let ipad = ua.match(/(iPad).*OS\s([\d_]+)/),
  isIphone = !ipad && ua.match(/(iPhone\sOS)\s([\d_]+)/),
  isAndroid = ua.match(/(Android)\s+([\d.]+)/),
  isMobile = isIphone || isAndroid;
let u = localStorage.getItem('u');
let userInfo: undefined | null = undefined;

if (u) {
  try {
    userInfo = JSON.parse(u)
  } catch (e) {
    userInfo = undefined;
  }
}


export default new Vuex.Store({
  state: {
    token: localStorage.getItem('token'),
    userInfo: userInfo,
    fullscreenLoading: false,
    isMobile: !!isMobile,
    dialogName: undefined,
  },
  mutations: {
    LOGIN: (state, data) => {
      //更改token的值
      state.token = data.token;
      state.userInfo = data.data;
      window.localStorage.setItem('token', data.token);
      window.localStorage.setItem('u', JSON.stringify(data.data));
    },
    LOGOUT: (state) => {
      //登出的时候要清除token
      state.token = null;
      state.userInfo = null;
      window.localStorage.removeItem('token');
      window.localStorage.removeItem('u');
    },
    UPDATEUSERINFO: (state, data) => {
      // 更新用户信息
      state.userInfo = data;
      window.localStorage.setItem('u', JSON.stringify(data));
    },
    UPDATEFULLSCREENLOADING: (state, data) => {
      state.fullscreenLoading = data;
    },
    SETDIALOGNAME: (state, data) => {
      state.dialogName = data;
    }
  },
  actions: {
    UserLogin({commit}, data) {
      commit('LOGIN', data);
    },
    UserLogout({commit}) {
      commit('LOGOUT');
    },
    UpdateUserInfo({commit}, data) {
      commit('UPDATEUSERINFO', data);
    },
    UpdateFullscreenLoading({commit}, data) {
      commit('UPDATEFULLSCREENLOADING', data);
    },
    setDialogName({commit}, data) {
      commit('SETDIALOGNAME', data);
    }
  }
})

