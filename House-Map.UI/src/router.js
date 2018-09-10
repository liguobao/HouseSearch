import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'

Vue.use(Router)

const router = new Router({
  mode: 'hash',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home,
      meta: {
        title: '地图搜租房'
      }
    },
    {
      path: '/user/callback',
      name: 'thirdPartyLogin',
      meta: {
        title: '第三方登录'
      },
      component: () => import('./views/third-party-login.vue')
    }
  ]
});

router.afterEach((to, from) => {
  if(to.meta.title) {
    document.title = to.meta.title
  }else {
    document.title = '地图搜租房'
  }
});

export default router
