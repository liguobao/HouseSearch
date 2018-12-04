<template>
  <div class="detail" :class="{'is-mobile':isMobile}">
    <header>
      <router-link tag="div" class="name" to="/">地图搜租房</router-link>
      <div class="form" v-if="detail">
        <a class="title" :href="detail.onlineURL" target="_blank">{{detail.title}}</a>
        <a class="source" href="javascript:;">{{detail.source}}</a>
      </div>
    </header>
    <div class="content"  v-if="detail">
      <div class="images-wrap" v-if="detail.pictures && detail.pictures.length">
        <el-carousel indicator-position="inside" :interval="0"  :autoplay="false" arrow="always">
          <el-carousel-item v-for="item in detail.pictures" :key="item">
            <a :href="item" target="_blank" class="image" >
              <img :src="item"/>
            </a>
          </el-carousel-item>
        </el-carousel>
      </div>
      <div class="info">
        <div class="title">
          {{detail.title}}
          <el-button class="" type="warning" icon="el-icon-star-off" circle size="mini" @click="collect(detail)" ></el-button>
        </div>
        <div class="address">
          <i class="el-icon-location"></i>
          <div class="">{{detail.location}}</div>
        </div>
        <div class="text" v-html="detail.text"></div>
      </div>
    </div>
    <component v-if="view" :is="view"></component>
  </div>
</template>
<style lang="scss" scoped>
  .detail{
    min-height: 100vh;
    background: #f1f1f1;
    header{
      padding: 25px 10%;
      background: rgba(26, 31, 42,0.9);
      .name{
        color: #0e90d2;
        font-size: 21px;
        font-weight: 600;
        letter-spacing: 7px;
        margin-bottom: 20px;
        cursor: pointer;
      }
      .form{
        display: flex;
        justify-content: space-between;
        .title{
          max-width: 80%;
        }
        a{
          color: #fff;
          font-size: 14px;
          transition: all 0.2s;
          &:hover{
            text-decoration: underline;
          }
        }
      }
    }
    .content{
      padding: 0 10%;
      margin: auto;
    }
    .images-wrap{
      margin-top: 10px;
    }
    .image {
      display: block;
      height: 100%;
      background-position: center;
      background-repeat: no-repeat;
      background-size: contain;
      img{
        display: block;
        object-fit: contain;
        height: 100%;
        max-width: 100%;
        margin: auto;
      }
    }
    .info{
      background: #fff;
      padding: 12px;
      .title{
        display: flex;
        align-items: baseline;
        justify-content: space-between;
        color: #333;
        font-size: 18px;
      }
      .address{
        border-top: 1px solid #737b8a;
        border-bottom: 1px solid #737b8a;
        color: #737b8a;
        display: flex;
        align-items: baseline;
        margin: 20px 0;
        padding: 8px 0;
        font-size: 16px;
        i{
          margin-right: 4px;
          font-size: 18px;
        }
      }
    }
    .text{
      word-break: break-word;
    }
    &.is-mobile{
      .content{
        padding: 0 4%;
      }
    }
  }
</style>
<script>
  const asyncComponent = require('./../components/async-component.js').default;
  export default {
    props:{
      id:{}
    },
    data() {
      return {
        detail: undefined,
        view:undefined,
        collection:false
      }
    },
    methods:{
      async collect(item) {
        let self = this;
        if (!self.user) {

          let com = require('../components/login-dialog').default;
          try {
            await asyncComponent(com, {
              props: {
                loginVisible: true,
                isMobile: self.isMobile
              }
            }, (template) => {
              this.view = template;
            });
            this.view = undefined;
          } catch (e) {
            this.view = undefined;
            return
          }
        }
        if (this.collection) {
          return
        }
        this.collection = true;
        const userId = this.$store.state.userInfo.id;
        const data = await this.$v2.post(`/users/${userId}/collections`, {
          // userId,
          houseID: item.id,
          // source: item.source
        });
        if (gtag) {
          gtag('event', '收藏', {
            'event_category': item.title,
            label: item.city
          });
        }
        this.collection = false;
        if (this.isMobile) {
          alert(data.message)
        } else {
          this.$message.success(data.message);
        }
      },
      async getDetail() {
        try {
          const data = await this.$v2.get(`/houses/${this.id}?tdsourcetag=s_pcqq_aiomsg`);
          this.detail = data.data;
        }catch (e) {
          this.$message.error(e.message)
        }
      }
    },
    computed:{
      isMobile() {
        return this.$store.state.isMobile
      },
      user() {
        return !!this.$store.state.userInfo
      }
    },
    created() {
      if(this.id) {
        this.getDetail();
        gtag('event', '房源详情页');
      }
    }
  }
</script>