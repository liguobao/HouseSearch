<template>
  <div class="detail" :class="{'is-mobile':isMobile}">
    <header>
      <router-link tag="div" class="name" to="/">地图搜租房</router-link>
      <div class="form" v-if="detail">
        <a class="title" :href="detail.onlineURL" target="_blank">{{detail.title}}</a>
        <a class="source" :href="detail.onlineURL" target="_blank">{{detail.onlineURL}}</a>
      </div>
    </header>
    <div class="content" v-if="detail">
      <div class="images-wrap" v-if="detail.pictures && detail.pictures.length">
        <el-carousel indicator-position="inside" :interval="0" :autoplay="false" arrow="always">
          <el-carousel-item v-for="item in detail.pictures" :key="item">
            <a :href="item" target="_blank" class="image">
              <img :src="item" v-if="!is3D(item)">
              <span v-else>点击进入3D看房</span>
            </a>
          </el-carousel-item>
        </el-carousel>
      </div>
      <div class="info">
        <div class="title">
          <a :href="detail.onlineURL" target="_blank">
            <el-tag size="mini" :type="tagType(detail)">{{tagText(detail)}}</el-tag>
            {{detail.title}}
          </a>
          <span class="price" v-if="detail.price > 0">
            <em>{{detail.price}}</em>/月
          </span>
          <span>发布时间：{{detail.pubTime}}</span>
          <el-button
            class="collect"
            type="warning"
            icon="el-icon-star-off"
            circle
            size="mini"
            @click="collect(detail)"
          ></el-button>
        </div>
        <div class="address">
          <div class="location">
            <i class="el-icon-location"></i>
            <div class="">{{detail.location}}</div>
          </div>
          <div id="map" class="map" v-if="detail && detail.longitude"></div>
        </div>
        <div class="text" v-html="detail.text"></div>
      </div>
    </div>
    <template v-if="is404">
      <div class="is-404">
        <div>404,找不到数据</div>
        <el-button @click="goBack" type="primary">返回地图页</el-button>
      </div>
    </template>
    <component v-if="view" :is="view"></component>
  </div>
</template>
<style lang="scss" scoped>
.detail {
  min-height: 100vh;
  background: #f1f1f1;

  .collect {
    position: absolute;
    right: 0;
    top: 50%;
    transform: translateY(-50%);
  }

  header {
    padding: 25px 10%;
    background: rgba(26, 31, 42, 0.9);

    .name {
      color: #0e90d2;
      font-size: 21px;
      font-weight: 600;
      letter-spacing: 7px;
      margin-bottom: 20px;
      cursor: pointer;
    }

    .form {
      display: flex;
      justify-content: space-between;
      flex-wrap: wrap;
      word-break: break-all;

      .title {
        max-width: 80%;
      }

      a {
        color: #fff;
        font-size: 14px;
        transition: all 0.2s;
        text-decoration: underline;

        &:hover {
          color: lightblue;
        }
      }
    }
  }

  .map {
    height: 300px;
    width: 100%;
    margin-top: 15px;
  }

  .content {
    padding: 0 10%;
    margin: auto;
  }

  .images-wrap {
    margin-top: 10px;
  }

  .image {
    display: block;
    height: 100%;
    background-position: center;
    background-repeat: no-repeat;
    background-size: contain;

    img {
      display: block;
      object-fit: contain;
      height: 100%;
      max-width: 100%;
      margin: auto;
    }

    span {
      display: flex;
      text-decoration: underline;
      align-items: center;
      justify-content: center;
      font-size: 16px;
      color: #7c7c7c;
      height: 100%;
    }
  }

  .info {
    background: #fff;
    padding: 12px;
    min-height: 1000px;

    .title {
      position: relative;
      display: flex;
      align-items: baseline;
      color: #333;
      font-size: 18px;
      transition: all 0.2s;
      flex-wrap: wrap;

      .price {
        margin-left: 15px;
        font-size: 16px;
        margin-right: 30px;

        em {
          font-style: normal;
          font-weight: normal;
          color: #e31818;
        }
      }

      a {
        display: block;
        color: #333;
        text-decoration: underline;

        &:hover {
          color: lightblue;
        }
      }
    }

    .address {
      border-top: 1px solid #737b8a;
      border-bottom: 1px solid #737b8a;
      color: #737b8a;
      display: flex;
      flex-direction: column;
      margin: 20px 0;
      padding: 8px 0;
      font-size: 16px;

      .location {
        display: flex;
        align-items: baseline;
      }

      i {
        margin-right: 4px;
        font-size: 18px;
      }
    }
  }

  .text {
    word-break: break-word;
  }

  &.is-mobile {
    .content {
      padding: 0 4%;
    }
  }
}
.is-404 {
  margin-top: 120px;
  font-size: 32px;
  text-align: center;
}
</style>
<script>
import { tagText, tagType } from "../components/house-type";

const asyncComponent = require("../components/async-component.tsx").default;
export default {
  props: {
    id: {},
    onlineURL: {}
  },
  data() {
    return {
      detail: undefined,
      view: undefined,
      collection: false,
      zoom: 16,
      map: undefined,
      is404: false
    };
  },
  methods: {
    goBack() {
      this.$router.replace(`/Map`);
    },
    tagText(item) {
      return tagText(item);
    },
    tagType(item) {
      return tagType(item);
    },
    appendScript(url) {
      return new Promise(resolve => {
        let jsapi = document.createElement("script");
        jsapi.charset = "utf-8";
        jsapi.src = url;
        document.head.appendChild(jsapi);
        jsapi.onload = () => {
          resolve();
        };
        jsapi.onerror = () => {
          this.$message.error("地图初始化失败,请重新刷新页面");
        };
      });
    },
    is3D(url) {
      return url.indexOf("realsee.com/ziroom/") !== -1;
    },
    async collect(item) {
      let self = this;
      if (!self.user) {
        let com = require("../components/login-dialog").default;
        try {
          await asyncComponent(
            com,
            {
              props: {
                loginVisible: true,
                isMobile: self.isMobile
              }
            },
            template => {
              this.view = template;
            }
          );
          this.view = undefined;
        } catch (e) {
          this.view = undefined;
          return;
        }
      }
      if (this.collection) {
        return;
      }
      this.collection = true;
      const userId = this.$store.state.userInfo.id;
      const data = await this.$ajax.post(`v2/users/${userId}/collections`, {
        // userId,
        houseID: item.id
        // source: item.source
      });
      if (gtag) {
        gtag("event", "收藏", {
          event_category: item.title,
          label: item.city
        });
      }
      this.collection = false;
      if (this.isMobile) {
        alert(data.message);
      } else {
        this.$message.success(data.message);
      }
    },
    async getDetail() {
      return new Promise(async (resolve, reject) => {
        try {
          const data = await this.$ajax.get(
            `v2/houses/${this.id}?tdsourcetag=s_pcqq_aiomsg&onlineURL=${
              this.$route.query.onlineURL
            }`
          );
          this.detail = data.data;
          if (data.data) {
            resolve();
          } else {
            this.is404 = true;
            reject();
          }
        } catch (e) {
          this.$message.error(e.message);
          reject();
        }
      });
    },
    installAmapUI(map, self) {
      return new Promise((resolve, reject) => {
        AMapUI.loadUI(["misc/PositionPicker"], function(PositionPicker) {
          let positionPicker = new PositionPicker({
            mode: "dragMarker", //设定为拖拽地图模式，可选'dragMap'、'dragMarker'，默认为'dragMap'
            zIndex: 100000,
            map: map //依赖地图对象
          });
          positionPicker.on("success", function(positionResult) {
            self.myPosition = positionResult.position;
            self.positionPicker = positionPicker;
            var lngLat = new AMap.LngLat(positionResult.position.lng,  positionResult.position.lat);
            self.lnglat = lngLat;
            self.keyword = positionResult.address;
          });

          // positionPicker.start();

          resolve(positionPicker);
        });
      });
    },
    installPlugin(map, self) {
      return new Promise((resolve, reject) => {
        AMap.plugin(
          [
            "AMap.ToolBar",
            "AMap.Driving",
            "AMap.LineSearch",
            `AMap.StationSearch`,
            "AMap.Transfer",
            "AMap.Walking",
            "AMap.Riding",
            "AMap.Geolocation",
            "AMap.AutoComplete",
            "AMap.PlaceSearch"
          ],
          function() {
            //异步同时加载多个插件
            let toolbar = new AMap.ToolBar();
            map.addControl(toolbar);

            let options = {
              showMarker: false //是否显示定位按钮
            };
            resolve();
          }
        );
      });
    },
    async initMap() {
      let map = new AMap.Map("map", {});
      this.map = map;
      map.clearMap();
      // map.setCity(this.detail.city);
      let marker = new AMap.Marker({
        map: map,
        title: this.detail.title,
        position: [this.detail.longitude, this.detail.latitude]
      });
      await this.installPlugin(map, self);
      await this.installAmapUI(map, self);
      map.add(marker);
      map.setZoomAndCenter(16, [this.detail.longitude, this.detail.latitude]);
    }
  },
  computed: {
    isMobile() {
      return this.$store.state.isMobile;
    },
    user() {
      return !!this.$store.state.userInfo;
    }
  },
  created() {},
  async mounted() {
    let key = `8a971a2f88a0ec7458d43b8bc03b6462`;
    let plugin = `AMap.ArrivalRange,AMap.Scale,AMap.Geocoder,AMap.Transfer,AMap.AutoComplete,AMap.CitySearch,AMap.Walking`.split();
    plugin.push(`AMap.ToolBar`);
    let url = `https://webapi.amap.com/maps?v=1.4.8&key=${key}&plugin=${plugin.join()}`;

    if (this.id) {
      await this.getDetail();
      gtag("event", "房源详情页");
      if (!this.detail.longitude) {
        return;
      }
      await this.appendScript(url);
      await this.appendScript(`//webapi.amap.com/ui/1.0/main.js?v=1.0.11`);
      this.initMap();
    }
  }
};
</script>