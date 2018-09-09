<template>
  <div class="header"
       :class="{'is-mobile':isMobile}"
  >
    <div>
      <router-link to="/" class="title">房子是租来的,而生活不是。</router-link>

      <nav v-if="!isMobile">
        <ul>
          <li>
            <router-link class="nav-item" to="/">网站首页</router-link>
          </li>
          <li>
            <el-dropdown>
            <span class="el-dropdown-link nav-item">
              热门城市<i class="el-icon-caret-bottom"></i>
            </span>
              <el-dropdown-menu slot="dropdown">
                <el-dropdown-item v-for="item in cities" :key="item">
                  <a :href="`https://www.woyaozufang.live/Home/HouseList?cityname=${item}&token=${token}`"
                     target="_blank"
                     class="link-to">{{item}}</a>
                </el-dropdown-item>
              </el-dropdown-menu>
            </el-dropdown>
          </li>
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
                  <el-dropdown-item><a href="javascript:" class="link-to" @click="showDashboards('user')">房源看板</a>
                  </el-dropdown-item>
                  <el-dropdown-item><a href="javascript:" class="link-to" @click="getUserHouseList">收藏列表</a>
                  </el-dropdown-item>
                  <el-dropdown-item disabled><a href="javascript:" class="link-to">绑定QQ</a></el-dropdown-item>
                  <el-dropdown-item><a href="javascript:" class="link-to" @click="setAddress">设置地址</a>
                  </el-dropdown-item>
                  <el-dropdown-item><a href="javascript:" class="link-to" @click="logOut">退出登录</a></el-dropdown-item>
                </template>
                <template v-else>
                  <el-dropdown-item ><a :href="oauthUrl ? oauthUrl : 'javascript:'" class="link-to">QQ登录</a>
                  </el-dropdown-item>
                  <el-dropdown-item><a href="javascript:" class="link-to"
                                       @click="toggleDialog('loginVisible',true,'login')">邮箱登录</a></el-dropdown-item>
                  <el-dropdown-item><a href="javascript:" class="link-to"
                                       @click="toggleDialog('loginVisible',true,'register')">注册账号</a></el-dropdown-item>
                </template>
              </el-dropdown-menu>
            </el-dropdown>
          </li>
          <li>
            <el-dropdown>
            <span class="el-dropdown-link nav-item">
              使用说明<i class="el-icon-caret-bottom"></i>
            </span>
              <el-dropdown-menu slot="dropdown">
                <el-dropdown-item v-for="item in instructions" :key="item.url">
                  <a :href="item.url" target="_blank" class="link-to">{{item.name}}</a>
                </el-dropdown-item>
                <el-dropdown-item><a href="javascript:" class="link-to" @click="scrollTo('contact')">联系我？</a>
                </el-dropdown-item>
              </el-dropdown-menu>
            </el-dropdown>
          </li>
        </ul>
      </nav>
    </div>
  </div>
</template>
<style lang="scss" scoped>
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
        max-width: 200px;
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
    &:hover {
      color: #095f8a;
    }
  }

  a {
    text-decoration: none;
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
      token: {}
    },
    computed: {
      user() {
        return !!this.$store.state.userInfo
      },
    },
    methods: {
      logOut() {
        this.$store.dispatch('UserLogout');
      },
      async setAddress() {
        this.$prompt('', '设置上班地址', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          inputValidator: /\S/,
          inputErrorMessage: '不能为空'
        }).then(async ({value}) => {
          const userId = this.$store.state.userInfo.id;
          await this.$ajax.post(`/users/${userId}/address`, {
            address: value
          });
          this.$message.success('保存成功')
        }).catch(() => {

        });
      }
    },
    data() {
      return {
        cities: ['上海', '北京', '广州', '深圳', '杭州', '南京', '武汉', '成都', '厦门', '苏州'],
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
        oauthUrl: undefined
      }
    },
    async created() {
      const data = await this.$ajax.get('/account/oauth-url');
      this.oauthUrl = data.url
    }
  }
</script>