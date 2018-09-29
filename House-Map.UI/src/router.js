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
    },
    {
      path: '/Map',
      name: 'Map',
      meta: {
        title: '地图搜租房'
      },
      component: () => import('./views/Map.vue')
    }
  ]
});


const baiduTongji = () => {
  window._hmt = window._hmt || [];
  let sc;
  return new Promise((resolve, reject) => {
    if ((sc = document.getElementById('baidu'))) {
      document.head.removeChild(sc);
    }
    let hm = document.createElement('script');
    hm.src = "https://hm.baidu.com/hm.js?a9c2d4398a08658288f9e13d2899545a";
    hm.id = 'baidu';
    let s = document.getElementsByTagName('script')[0];
    s.parentNode.insertBefore(hm, s);
    hm.onload = function () {
      resolve()
    };
    hm.onerror = function () {
      reject()
    }
  });
};

router.beforeEach(async (to, from, next) => {
  // 统计代码
  try {
    if(process.env.NODE_ENV === "production") {
      await baiduTongji();
    }
    next();
  }catch (e) {
    next();
  }

});


router.afterEach((to, from) => {
  if(to.meta.title) {
    document.title = to.meta.title
  }else {
    document.title = '地图搜租房'
  }
});

export default router
