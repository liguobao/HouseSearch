<template>
  <div class="header"
       :class="{'is-mobile':isMobile}"
  >
    <div>
      <div class="title" v-if="!isMobile">
        <router-link to="/"  >地图搜租房</router-link>
        <el-dropdown>
            <span class="el-dropdown-link location">
              <i class="el-icon-location"></i>{{location}}<i class="el-icon-caret-bottom"></i>
            </span>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item v-for="item in cities" :key="item">
              <a href="javascript:;"
                 target="_blank"
                 @click="cityLocation(item)"
                 class="link-to">{{item}}</a>
            </el-dropdown-item>
          </el-dropdown-menu>
        </el-dropdown>
      </div>
      <a href="https://wj.qq.com/s/2953926/aabe" target="_blank" class="title"  v-else>地图搜租房</a>
      <nav v-if="!isMobile">
        <ul>
          <li>
            <a href="https://wj.qq.com/s/2953926/aabe" target="_blank" class="nav-item">帮我们做得更好?</a>
            <!--<router-link class="nav-item" to="/">网站首页</router-link>-->
          </li>
          <!--<li>-->
            <!--<el-dropdown>-->
            <!--<span class="el-dropdown-link nav-item">-->
              <!--热门城市<i class="el-icon-caret-bottom"></i>-->
            <!--</span>-->
              <!--<el-dropdown-menu slot="dropdown">-->
                <!--<el-dropdown-item v-for="item in cities" :key="item">-->
                  <!--<a href="javascript:;"-->
                     <!--target="_blank"-->
                     <!--@click="navTo(item)"-->
                     <!--class="link-to">{{item}}</a>-->
                <!--</el-dropdown-item>-->
              <!--</el-dropdown-menu>-->
            <!--</el-dropdown>-->
          <!--</li>-->
          <li>
            <el-dropdown class="dropdown">
            <span class="el-dropdown-link nav-item">
              <template v-if="user">
                {{$store.state.userInfo.userName}}
              </template>
              <template v-else>
                个人中心
              </template>
              <i class="el-icon-caret-bottom"></i>
            </span>
              <el-dropdown-menu slot="dropdown">
                <template v-if="user">
                  <el-dropdown-item><a href="javascript:" class="link-to"
                                       @click="showDashboards('user')">房源看板</a>
                  </el-dropdown-item>
                  <!--<el-dropdown-item><a href="javascript:" class="link-to" @click="getUserHouseList">收藏列表</a>-->
                  <!--</el-dropdown-item>-->
                  <el-dropdown-item disabled><a href="javascript:" class="link-to">绑定QQ</a>
                  </el-dropdown-item>
                  <el-dropdown-item><a href="javascript:" class="link-to" @click="setAddress">设置地址</a>
                  </el-dropdown-item>
                  <el-dropdown-item><a href="javascript:" class="link-to" @click="logOut">退出登录</a>
                  </el-dropdown-item>
                </template>
                <template v-else>
                  <el-dropdown-item><a :href="oauthUrl ? oauthUrl : 'javascript:'" class="link-to">QQ登录</a>
                  </el-dropdown-item>
                  <el-dropdown-item><a href="javascript:" class="link-to"
                                       @click="login('login')">邮箱登录</a>
                  </el-dropdown-item>
                  <el-dropdown-item><a href="javascript:" class="link-to"
                                       @click="login('register')">注册账号</a>
                  </el-dropdown-item>
                </template>
              </el-dropdown-menu>
            </el-dropdown>
          </li>
          <li v-if="user">
            <a href="javascript:" class="nav-item" @click="getUserHouseList">房源收藏</a>
            <!--<router-link class="nav-item" to="/">网站首页</router-link>-->
          </li>
          <li>
            <el-dropdown>
            <span class="el-dropdown-link nav-item">
              说明/公告<i class="el-icon-caret-bottom"></i>
            </span>
              <el-dropdown-menu slot="dropdown">
                <el-dropdown-item v-for="item in instructions" :key="item.url">
                  <a :href="item.url" target="_blank" class="link-to">{{item.name}}</a>
                </el-dropdown-item>
                <el-dropdown-item><a href="javascript:" class="link-to"
                                     @click="getHistoryNotices">历史公告</a>
                </el-dropdown-item>
                <el-dropdown-item><a href="javascript:" class="link-to" @click="scrollTo('contact')">联系我？</a>
                </el-dropdown-item>
              </el-dropdown-menu>
            </el-dropdown>
          </li>
          <li>
              <a href="https://mp.weixin.qq.com/s/Vzu4fsFspSvNqvOJ0kWfyg" target="_blank" class="nav-item">关于我们</a>
          </li>
        </ul>
      </nav>
    </div>
    <el-dialog
        top="50px"
        :width="isMobile ? '100%' : '70%'"
        title="历史公告"
        :visible.sync="noticesVisible"
        append-to-body
        center
    >
      <div class="history-notices">
        <el-collapse accordion>
          <el-collapse-item :name="item.id" v-for="item in historyNotices"
                            :key="item.id">
              <template slot="title">
                <div v-html="historyTitle(item)"></div>
              </template>
            <div class="history-notices-item"  v-html='item.content'></div>
          </el-collapse-item>
        </el-collapse>
      </div>
    </el-dialog>

    <component v-if="view" :is="view"></component>
  </div>
</template>
<style lang="scss" scoped>
  ul {
    margin: 0;
  }

  .is-mobile.header {
    background: #1a1f2a;
    .title {
      font-size: 16px;
    }
    > div {
      width: auto !important;
    }
  }

  .more {
    position: absolute;
    top: 50%;
    transform: translateY(-50%);
    color: #909399;
    right: 5px;
  }
</style>
<style lang="scss" scoped>
  .location{
    color: #fff;
    cursor: pointer;
    font-weight: normal;
  }
  .header {
    padding: 0 22px;
    > div {
      display: flex;
      justify-content: space-between;
      align-items: flex-end;
      max-width: 1200px;
      margin: auto;
      position: relative;
    }
  }

  nav {
    margin-right: 60px;
    position: absolute;
    right: 0;
    top: 50%;
    transform: translateY(-50%);
    > ul {
      display: flex;
      li {
        max-width: 260px;
      }
      .dropdown {
        width: 100%;
        overflow: hidden;
      }
      > li:not(:last-of-type) {
        margin-right: 5px;
      }
      .nav-item {
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        display: block;
        max-width: 100%;
        color: #fff;
        font-size: 16px;
        padding: 6px 16px;
        cursor: pointer;
        transition: all 0.3s;
        .el-icon-caret-bottom {
          transition: all 0.3s;
        }
        &:hover {
          background: #ba1f2e;
          border-radius: 2px;
        }
        &:hover .el-icon-caret-bottom {
          transform: rotate(180deg);
        }
      }
    }
  }

  .is-disabled {
    .link-to {
      color: #bbb;
    }
  }

  .link-to {
    color: #333;
    display: block;
    &:hover {
      color: #409EFF;
    }
  }

  .title {
    color: #0e90d2;
    font-size: 21px;
    font-weight: 600;
    letter-spacing: 7px;
    transition: all 0.5s;
    a{
      color: #0e90d2;
      font-size: 30px;
      font-weight: 600;
      letter-spacing: 7px;
      transition: all 0.5s;
      &:hover {
        color: #095f8a;
      }
    }
    &:hover {
      color: #095f8a;
    }
  }

  a {
    text-decoration: none;
  }

  .history-notices {
    max-height: 600px;
    overflow-x: hidden;
    overflow-y: auto;
  }

  .history-notices-item {
    word-break: break-all;
  }
</style>
<script>
  export default {
    props: {
      toggleDialog: {},
      getUserHouseList: {},
      showDashboards: {},
      scrollTo: {},
      isMobile: {},
      token: {},
      getUserInfo: {},
      location:{},
      type:{
        default:''
      }
    },
    computed: {
      user() {
        return !!this.$store.state.userInfo
      },
      dialogName() {
        let dialogName = this.$store.state.dialogName;
        if(dialogName === 'setAddress') {
          if(this.$store.state.userInfo && !this.$store.state.userInfo.email) {
            this.setAddress();
          }
          this.$store.dispatch('setDialogName',undefined);
        }
        return dialogName;
      }
    },
    methods: {
      cityLocation(item){
        this.$emit('changeLocation',item)
      },
      navTo(item) {
        const params = {
          city: item
        };
        let {href} = this.$router.resolve({path: `/Map?${this.$qs.stringify(params)}`});
        window.open(href, '_blank');
      },
      historyTitle(item) {
        const name = item.content.slice(0, 80);
        const time = this.$transformData(item.dataCreateTime, 'yyyy-MM-dd hh:mm:ss')
        return `${name}... (${time})`
      },
      async login(type) {
        const asyncComponent = require('./../components/async-component.js').default;
        let com = require('./login-dialog').default;
        try {
          await asyncComponent(com, {
            props: {
              loginType: type,
              appendToBody: true,
              isMobile: this.isMobile
            }
          }, (template) => {
            this.view = template;
          });
          this.view = undefined;
        }catch (e) {
          this.view = undefined;
        }
      },
      logOut() {
        this.$store.dispatch('UserLogout');
      },
      async setAddress() {
        let text = '';
        if(this.user){
          let user = this.$store.state.userInfo;
          if(user.workAddress){
            text = `记录: ${user.workAddress}`
          }
        }
        this.$prompt(text, '设置上班地址', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          inputValidator: /\S/,
          inputErrorMessage: '不能为空'
        }).then(async ({value}) => {
          const userId = this.$store.state.userInfo.id;
          await this.$ajax.post(`/users/${userId}/address`, {
            address: value
          });
          this.$message.success('保存成功');
          this.getUserInfo();
        }).catch(() => {

        });
      },
      async getHistoryNotices() {
        const data = await this.$ajax.get('/notices');
        this.historyNotices = data.data;
        this.noticesVisible = true;
      },
      async getCities() {
        const data = await this.$v2.get('/cities?fields=id,city,sources&index=0&count=15');
        this.cities = data.data.map(item=>{
          return item.city
        })
      }
    },
    data() {
      return {
        cities: [],
        instructions: [
          {
            name: '使用教程',
            url: 'https://github.com/liguobao/58HouseSearch/blob/master/%E4%BD%BF%E7%94%A8%E6%95%99%E7%A8%8B.md'
          },
          {
            name: '一些技巧',
            url: 'https://github.com/liguobao/58HouseSearch/blob/master/%E4%B8%80%E4%BA%9B%E6%8A%80%E5%B7%A7.md'
          },
          {
            name: '更新日志',
            url: 'https://github.com/liguobao/58HouseSearch/blob/master/%E6%97%A5%E5%B8%B8%E6%9B%B4%E6%96%B0.md'
          }
        ],
        oauthUrl: undefined,
        noticesVisible: false,
        historyNotices: [],
        view: undefined
      }
    },
    async created() {
      const data = await this.$ajax.get('/account/oauth-url');
      this.oauthUrl = data.url;
      this.getCities()
    },
    async mounted(){

    }
  }
</script>