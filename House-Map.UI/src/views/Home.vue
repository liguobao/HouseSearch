<template>
  <div class="home"
       :class="{'is-mobile': isMobile}"
       v-loading.fullscreen.lock="fullscreenLoading"
  >
    <Header
        :is-mobile="isMobile"
        class="header"
        ref="header" :class="{sticky: sticky}"
        :toggle-dialog="toggleDialog"
        :get-user-house-list="getUserHouseList"
        :show-dashboards="showDashboards"
        :scroll-to="scrollTo"
        :token="token"
    >

    </Header>
    <div class="banner ">
      <h2 class="slogan">满大街找租房心力交瘁？试试换个方式直接在地图上搜租房吧。</h2>
      <p class="sub-slogan">房源爬虫 + 高德地图强力驱动,帮助你迅速找到合适房源</p>
      <el-button type="danger" class="start" @click="scrollTo('introduction')">马上开始！</el-button>
    </div>
    <div class="introduction " ref="introduction">
      <div>
        <h3 class="sub-title">这是什么？</h3>
        <p class="text">
          通过实时爬虫获取公开租房信息，直接在高德地图上直观展示房源位置+基础信息，同时提供住址到公司的路线计算（公交+地图 or
          步行导航），已实现【豆瓣租房小组】、【Zuber合租平台】、【蘑菇租房】、【CCB建融家园】、【58同城品牌公寓】、【58同城诚信租房】、【上海互助租房】数据展示，部分房源价格支持筛选功能。
        </p>
        <div class="cities">
          <div class="city-item" v-for="item in cities" :key="item.name">
            <a target="_blank" :href="mapUrl + `?cityname=${item.cityname}&token=${token}`" class="highlight-name">{{item.name}}</a>
            <div class="form" v-if="item.form && item.form.length">
              <a target="_blank" :href="mapUrl + `?cityname=${item.cityname}&source=${where.source}&token=${token}`"
                 class="highlight-name" v-for="(where,index) in item.form" :key="where.name">
                {{where.name}}
                <template v-if="index < item.form.length - 1">、</template>
              </a>
            </div>
          </div>
          <div class="city-item">
            <a target="_blank" href="javascript:;" class="highlight-name" @click="showDashboards('all')">更多城市</a>
            <div class="form">
              <a target="_blank" :href="mapUrl + `?cityname=成都&token=${token}`" class="highlight-name">
                成都、
              </a>
              <a target="_blank" :href="mapUrl + `?cityname=杭州&token=${token}`" class="highlight-name">
                杭州、
              </a>
              <a target="_blank" :href="mapUrl + `?cityname=厦门&token=${token}`" class="highlight-name">
                厦门...
              </a>
            </div>
          </div>
          <div class="city-item search">
            <a target="_blank" href="javascript:;" class="highlight-name" @click="toggleDialog('searchVisible',true)">高级搜索</a>
            <p>支持关键字 + 信息来源 + 发布日期组合搜索</p>
          </div>
        </div>
        <div class="new-douban running">
          <a href="javascript:;" class="highlight-name" @click="toggleDialog('doubanAddVisible',true)">新增豆瓣租房小组</a>
          <p>你在的城市没有数据？没有对应的租房小组数据？试试手动添加爬虫任务吧！（如：厦门租房小组 https://www.douban.com/group/XMhouse/）</p>
        </div>
      </div>
    </div>
    <div class="thanks ">
      <div class="line"></div>
      <div class="content">
        <h3>感谢他们</h3>
        <p>灵感+部分代码来源实验楼<a href="https://www.shiyanlou.com/user/8834/" target="_blank" class="highlight-name">ekCit</a> 的
          <a target="_blank" href="https://www.shiyanlou.com/courses/599" class="highlight-name">高德API+Python解决租房问题</a>
          课程，感谢他...</p>
        <ul>
          <li>
            <img src="./../images/microsoft.png" alt="微软"/>
          </li>
          <li>
            <img src="./../images/tencent.png" alt="腾讯"/>
          </li>
          <li>
            <img src="./../images/aliyun.png" alt="阿里云"/>
          </li>
        </ul>
      </div>
    </div>
    <div class="contact " ref="contact">
      <div>
        <p>联系我？试试下面的方式咯。</p>
        <div class="ways">
          <div>
            <span>知乎: </span>
            <a href="https://www.zhihu.com/people/codelover" class="highlight-name" target="_blank">李国宝</a>
          </div>
          <div>
            <span>GitHub: </span>
            <a href="https://github.com/liguobao/58HouseSearch" class="highlight-name" target="_blank">58HouseSearch</a>
          </div>
          <div>
            <span>邮件: </span><em>codelover@qq.com</em>
          </div>
        </div>
      </div>
    </div>
    <footer>
      <div>
        <div>
          Copyright 2016 - 2018 www.woyaozufang.live. All Rights Reserved
          <a href="http://www.miitbeian.gov.cn/" class="highlight-name" target="_blank">粤ICP备18055424号</a>
          <a href="/" class="highlight-name" >地图搜租房</a>
        </div>
        <div class="call-me">
          <el-tooltip class="item" effect="dark" content="88888888" placement="top">
            <i class="iconfont icon-icon highlight-name"></i>
          </el-tooltip>
          <el-tooltip class="item" effect="dark" content="138-0000-0000" placement="top">
            <i class="iconfont icon-dianhua highlight-name"></i>
          </el-tooltip>
          <el-tooltip class="item" effect="dark" content="138-0000-0000" placement="top">
            <i class="iconfont icon-changyonglogo28 highlight-name"></i>
          </el-tooltip>
          <el-tooltip class="item" effect="dark" content="codelover@qq.com" placement="top">
            <i class="iconfont icon-jianzhuanquan- highlight-name"></i>
          </el-tooltip>
        </div>
      </div>
    </footer>

    <search-dialog :token="token" :is-mobile="isMobile" :visible="searchVisible" @close="toggleDialog"></search-dialog>

    <el-dialog
        :title="userSource ? '房源列表' : '全部城市'"
        :width="isMobile ? '100%' : '70%'"
        center
        :top="isMobile ? '0px' : '50px'"
        :visible="dashboardsVisible"
        :before-close="() => {toggleDialog('dashboardsVisible')}"
    >
      <dashboards :type="dashboardsType" :key="dashboardsType" :is-mobile="isMobile" :token="token"></dashboards>
    </el-dialog>

    <el-dialog
        title="新增豆瓣租房小组"
        :width="isMobile ? '100%' : '500px'"
        center
        :visible="doubanAddVisible"
        :before-close="() => {toggleDialog('doubanAddVisible')}"
    >
      <douban-add></douban-add>
    </el-dialog>

    <el-dialog
        title="地图搜租房"
        :width="isMobile ? '100%' : '700px'"
        center
        :visible="loginVisible"
        :before-close="() => {toggleDialog('loginVisible')}"
    >
      <login @close="toggleDialog" :login-type="loginType"></login>
    </el-dialog>

    <el-dialog
        top="50px"
        :width="isMobile ? '100%' : '70%'"
        title="房源列表"
        :visible.sync="userHouseVisible"
        append-to-body
        center
        key="user"
        :before-close="() => {toggleDialog('userHouseVisible')}"
    >
      <house-search-list type="user" @close="toggleDialog" :house-list="userHouseList" :token="token"></house-search-list>
    </el-dialog>

  </div>
</template>
<style lang="scss" scoped>
  .is-mobile.home{
    min-width: auto !important;
    min-height: auto !important;
    .banner{
      height: 200px;
      background-size: 100% auto;
      .slogan{
        font-size: 16px;
        text-align: center;
        padding: 0 10px;
      }
      .sub-slogan{
        font-size: 12px;
        text-align: center;
        padding: 0 10px;
      }
      .start{
        font-size: 12px;
        padding: 6px 10px;
      }
    }
    .header{
      padding-left: 10px;
      padding-right: 10px;
      position: sticky;
      top: 0;
    }
    footer{
      padding-left: 20px;
      padding-right: 20px;
      text-align: center;
      >div{
        align-items: center;
        flex-direction: column;
      }
      a{
        display: block;
      }
    }
  }
</style>
<style lang="scss" scoped>
  @keyframes right2left {
    0% {
      opacity: 0;
      transform: translateX(-100%);
    }
    100% {
      opacity: 1;
      transform: translateX(0);
    }
  }

  @keyframes left2right {
    0% {
      opacity: 0;
      transform: translateX(100%);
    }
    100% {
      opacity: 1;
      transform: translateX(0);
    }
  }

  @keyframes toUp {
    0% {
      opacity: 0;
      transform: translateY(50%);
    }
    100% {
      opacity: 1;
      transform: translateY(0);
    }
  }

  @keyframes toDown {
    0% {
      opacity: 0;
      transform: translateY(-60%);
    }
    100% {
      opacity: 1;
      transform: translateY(0);
    }
  }

  .home {
    min-height: 120vh;
    position: relative;
    min-width: 1200px;
    overflow: auto;
  }

  .header {
    position: fixed;
    top: 25px;
    left: 0;
    width: 100%;
    padding: 20px 0;
    transition: all 0.4s;
    z-index: 99;
    &.sticky {
      top: 0;
      opacity: 0.95;
      background: #1a1f2a;
    }
  }

  .banner {
    background: url("./../images/banner1.jpg") no-repeat;
    background-position: center;
    background-size: cover;
    height: 680px;
    display: flex;
    align-items: center;
    flex-direction: column;
    justify-content: center;
    color: #fff;
    font-weight: 400;
    &.running {
      .slogan {
        animation: right2left 0.5s ease-out both;
      }
      .sub-slogan {
        animation: left2right 0.5s ease-out both;
      }
      .start {
        animation: toUp 0.5s 0.2s ease-out both;
      }
    }
    .slogan {
      font-size: 40px;
      font-weight: inherit;
    }
    .sub-slogan {
      font-weight: inherit;
      font-size: 30px;

    }
    .start {
      margin-top: 20px;
      background: #d22e3e;
      border-color: #d22e3e;
      transition: all 0.3s;
      &:hover {
        background: #ba1f2e;
        border-color: #ba1f2e;
      }
    }
  }

  .introduction {
    padding: 40px 20px;
    background: #1a1f2a;
    color: #fff;
    &.running {
      .sub-title {
        animation: toUp 0.5s ease-out both;
      }
      .text {
        animation: toUp 0.5s 0.1s ease-out both;
      }
      @for $i from 1 to 7 {
        .city-item:nth-of-type(#{$i}) {
          animation: toUp 0.5s (0.1s*$i) ease-out both;
        }
      }
    }
    > div {
      max-width: 1200px;
      margin: auto;
    }
    .sub-title {
      margin-top: 40px;
      font-size: 20px;
      font-weight: 600;
    }
    .text {
      margin-bottom: 20px;
      color: #737b8a;
      font-size: 14px;
    }
    .cities {
      display: flex;
      flex-wrap: wrap;
      margin-bottom: 32px;
      .city-item {
        flex: auto;
        min-width: 180px;
        &.search {
          width: 184px;
        }
        p {
          font-size: 14px;
          color: #737b8a;
        }
      }
      .form {
        .highlight-name {
          font-size: 14px;
          font-weight: 400;
        }
      }
    }
    .new-douban {
      &.running {
        animation: toDown 0.5s ease-out both;
      }
      p {
        font-size: 14px;
        color: #737b8a;
      }
      margin-bottom: 20px;
    }
  }

  .thanks {
    background: #fff;
    padding: 20px;
    &.running {
      h3 {
        animation: toUp 0.5s ease-out both;
      }
      p {
        animation: toUp 0.5s 0.1s ease-out both;
      }
      @for $i from 1 to 4 {
        li:nth-of-type(#{$i}) {
          animation: toUp 0.5s (0.1s*$i) ease-out both;
        }
      }
    }
    .line {
      border-bottom: 1px solid #eee;
      margin: 20px 0;
    }
    .content {
      max-width: 1200px;
      margin: 40px auto 50px auto;
      h3 {
        color: #2b3242;
        font-size: 20px;
        font-weight: 600;
      }
      p {
        color: #737b8a;
        font-size: 14px;
        margin-bottom: 30px;
      }
      .highlight-name {
        font-size: inherit;
        font-weight: inherit;
      }
    }
    ul {
      display: flex;
    }
    li {
      margin-right: 12px;
      width: 135px;
      border: 1px solid #E9E9E9;
      transition: all 0.5s;
      filter: grayscale(100%);
      &:hover {
        filter: grayscale(0%);
        border-color: #737b8a;
      }
      img {
        display: block;
        max-width: 100%;
      }
    }
  }

  .highlight-name {
    color: #0e90d2;
    font-size: 20px;
    font-weight: 600;
    transition: all 0.3s;
    &:hover {
      color: #095f8a;
    }
  }

  .contact {
    padding: 40px 20px 140px 20px;
    background: #1a1f2a;
    color: #fff;
    &.running {
      p {
        animation: toUp 0.5s ease-out both;
      }
      .ways {
        @for $i from 1 to 4 {
          div:nth-of-type(#{$i}) {
            animation: toUp 0.5s (0.1s*$i) ease-out both;
          }
        }
      }
    }
    > div {
      max-width: 1200px;
      margin: 40px auto 0 auto;
      p {
        font-size: 20px;
        font-weight: 600;
        margin-bottom: 5px;
      }
      .ways {
        display: flex;
        align-items: baseline;
        flex-wrap: wrap;
        > div:not(:last-of-type) {
          margin-right: 15%;
        }
        span, em {
          font-weight: normal;
          font-style: normal;
          font-size: 16px;
        }
        a {
          font-size: 16px;
          font-weight: 400;
        }
        a, em {
          margin-left: 5px;
        }
      }
    }

  }

  footer {
    padding: 30px 0;
    background: #0d121b;
    font-size: 12px;
    color: #555d6d;
    > div {
      margin: 0 auto;
      max-width: 1200px;
      display: flex;
      justify-content: space-between;
      .highlight-name {
        font-size: 12px;
        margin-right: 5px;
      }
      .call-me {
        i {
          cursor: pointer;
        }
        .highlight-name {
          font-size: 14px;
          margin: 0 2px;
        }
      }
    }
  }
</style>
<script>
  import Header from './../components/header';
  import SearchDialog from '../components/search-dialog';
  import Dashboards from './../components/dashboards';
  import DoubanAdd from './../components/douban-add';
  import Login from './../components/login'
  import HouseSearchList from './../components/house-search-list'

  export default {
    name: 'home',
    components: {
      Header,
      SearchDialog,
      Dashboards,
      DoubanAdd,
      Login,
      HouseSearchList
    },
    computed: {
      fullscreenLoading() {
        return this.$store.state.fullscreenLoading
      },
      token() {
        return this.$store.state.token ? this.$store.state.token : ''
      }
    },
    data() {
      return {
        cities: [
          {
            name: '上海地区',
            url: '',
            cityname: '上海',
            form: [
              {
                name: '互助租房',
                url: '',
                source: 'huzhuzufang'
              },
              {
                name: '豆瓣',
                url: '',
                source: 'douban'
              },
              {
                name: 'Zuber',
                url: '',
                source: 'zuber'
              },
            ]
          },
          {
            name: '北京地区',
            url: '',
            cityname: '北京',
            form: [
              {
                name: '豆瓣',
                url: '',
                source: 'douban'
              },
              {
                name: 'Zuber',
                url: '',
                source: 'zuber'
              },
              {
                name: '蘑菇',
                url: '',
                source: 'mogu'
              },
            ]
          },
          {
            name: '广州地区',
            url: '',
            cityname: '广州',
            form: [
              {
                name: '豆瓣',
                url: '',
                source: 'douban'
              },
              {
                name: 'Zuber',
                url: '',
                source: 'zuber'
              },
              {
                name: '蘑菇',
                url: '',
                source: 'mogu'
              },
            ]
          },
          {
            name: '深圳地区',
            url: '',
            cityname: '深圳',
            form: [
              {
                name: '豆瓣',
                url: 'douban'
              },
              {
                name: 'Zuber',
                url: 'zuber'
              },
            ]
          }
        ],
        mapUrl: `https://www.woyaozufang.live/Home/HouseList`,
        sticky: false,
        elements: [],
        searchVisible: false,
        dashboardsVisible: false,
        doubanAddVisible: false,
        loginVisible: false,
        loginType: undefined,
        userHouseList: [],
        userHouseVisible: false,
        userSource: false,
        dashboardsType: 'all',
        isMobile: false
      }
    },
    methods: {
      showDashboards(type) {
        this.dashboardsType = type;
        this.toggleDialog('dashboardsVisible', true)
      },
      async getUserHouseList() {
        this.$store.dispatch('UpdateFullscreenLoading', true);
        const userId = this.$store.state.userInfo.id;
        const data = await this.$ajax.get(`/users/${userId}/collections`);
        this.userHouseList = data.data;
        this.userHouseVisible = true;
        this.$store.dispatch('UpdateFullscreenLoading', false);
      },
      async getUserInfo() {
        const u = localStorage.getItem('u');
        if (!u) {
          this.$store.dispatch('UserLogout');
        } else {
          try {
            const userId = JSON.parse(u).id;
            const data = await this.$ajax.get(`/users/${userId}`);
            this.$store.dispatch('UpdateUserInfo', data.data);
          } catch (e) {
            this.$store.dispatch('UserLogout');
          }
        }
      },
      toggleDialog(key, val, type) {
        if (key === 'loginVisible') {
          if (type) {
            this.loginType = type;
          } else {
            this.loginType = undefined;
          }
        }
        this[key] = val || false;
      },
      scroll() {
        if(this.isMobile) {
          return
        }
        let scrollTop = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop;
        let offsetTop = this.$refs.header.$el.offsetTop;
        this.sticky = scrollTop > (offsetTop);
      },
      scrollTo(el) {
        this.$refs[el].scrollIntoView({
          behavior: 'smooth',
          block: 'start',
          inline: 'center'
        });
      },
      animation() {
        //滚动条高度+视窗高度 = 可见区域底部高度
        let visibleBottom = window.scrollY + document.documentElement.clientHeight;
        //可见区域顶部高度
        let visibleTop = window.scrollY;

        for (let i = 0; i < this.elements.length; i++) {
          let centerY = this.elements[i].offsetTop + (this.elements[i].offsetHeight / 2);
          if (centerY > visibleTop && centerY < visibleBottom) {
            this.elements[i].classList.add('running');
          } else {
            if(!this.isMobile) {
              this.elements[i].classList.remove('running');
            }
          }
        }

      }
    },
    async created() {
      this.getUserInfo();
      let ua = navigator.userAgent;
      let ipad = ua.match(/(iPad).*OS\s([\d_]+)/),
        isIphone =!ipad && ua.match(/(iPhone\sOS)\s([\d_]+)/),
        isAndroid = ua.match(/(Android)\s+([\d.]+)/),
        isMobile = isIphone || isAndroid;
      if(isMobile) {
        this.isMobile = true
      }
    },
    async mounted() {
      document.addEventListener('scroll', () => {
        this.scroll();
        this.animation();
      });
      this.elements = [document.querySelector('.banner'), document.querySelector('.introduction'), document.querySelector('.thanks'), document.querySelector('.contact')];
      this.animation();

    }
  }
</script>
