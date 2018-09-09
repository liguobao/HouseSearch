import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    token: localStorage.getItem('token'),
    userInfo: localStorage.getItem('u') ? JSON.parse(localStorage.getItem('u')) : undefined,
    fullscreenLoading: false
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
    }
  }
})
