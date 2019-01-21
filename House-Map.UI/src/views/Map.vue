<template>
  <div class="map" :class="{'showInfo':isMobile}">
    <header v-if="!isMobile">
      <div class="title" v-if="!isMobile">
        <router-link to="/">地图搜租房</router-link>
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
      <div class="search-wrap">
        <div class="key-word">
          <div class="keyword-tag">
            <el-tag
                class="tag"
                :key="tag"
                v-for="tag in keywordArr"
                closable
                :disable-transitions="false"
                @close="removeKeyword(tag)">
              {{tag}}
            </el-tag>
          </div>
          <el-input
              class="keyword-tag-input"
              v-model="keywordTag"
              ref="saveTagInput"
              size="small"
              v-if="inputVisible"
              @keyup.enter.native="keywordConfirm"
              @blur="keywordConfirm"
              placeholder="搜索关键字"
              :maxlength="50"></el-input>
          <el-button v-if="!inputVisible&&keywordArr.length <= 6" class="button-new-tag" size="small"
                     @click="showInput">+ 关键词
          </el-button>
        </div>
        <el-button type="primary" class="search-btn" size="small" @click="refresh">搜索</el-button>
      </div>
    </header>
    <div class="filter-wrap" v-if="!isMobile">
      <ul>
        <li>
          <span>房源类型</span>
          <div>
            <el-select v-model="form.rentType" @change="refresh" placeholder="请选择房源类型" size="mini" style="width: 100%">
              <el-option
                  :ket="item.value"
                  v-for="item in rentTypeArr"
                  :label="item.label"
                  :value="item.value"
              ></el-option>
            </el-select>
          </div>

          <span>价位</span>
          <div>
            <el-col :span="11">
              <el-input @change="refresh" v-model="form.fromPrice" size="mini" type="number" placeholder="最低价"
                        :maxlength="8"></el-input>
            </el-col>
            <el-col class="line" :span="2">-</el-col>
            <el-col :span="11">
              <el-input @change="refresh" v-model="form.toPrice" size="mini" type="number" placeholder="最高价"
                        :maxlength="8"></el-input>
            </el-col>
          </div>

          <span>房源</span>
          <div class="displaySource">
            <span>全部</span>
            <span :class="{active:item.source === $route.query.source}" v-for="item in source" @click="selectSource(item.source)">{{item.displaySource}}</span>

            <!--<el-select @change="refresh" v-model="form.source" size="mini" placeholder="请选择房源"-->
                       <!--filterable>-->
              <!--&lt;!&ndash;<el-option label="全部" value=""></el-option>&ndash;&gt;-->
              <!--<el-option-->
                  <!--v-for="item in otherSource"-->
                  <!--:label="item.displaySource"-->
                  <!--:value="item.source"-->
                  <!--:key="item.id"-->
              <!--&gt;-->
              <!--</el-option>-->
            <!--</el-select>-->
          </div>
        </li>

        <li>
          <el-button type="info" size="mini" @click="resetFilter">重置</el-button>
        </li>
      </ul>
    </div>
    <div class="map-content">
      <div v-if="!isMobile" class="map-house-list">
        <!--<span class="toggleHouseList" @click="toggleHouseListUp = !toggleHouseListUp">{{toggleHouseListUp ? '收起' : '展开'}}</span>-->
        <el-collapse-transition>
          <house-list height="100%" v-show="toggleHouseListUp" :house-list="mapHouseList" @click="houseListClick"
                      v-if="mapHouseList && mapHouseList.length"></house-list>
        </el-collapse-transition>

      </div>
      <div class="container-wrap">
        <template v-if="!inMap">
          <div id="panel" v-show="transferWays" :class="{'slide-up' : toggleUp,'is-mobile' : isMobile}">
            <!--<span class="panel-handle" @click="toggleUp = !toggleUp">{{!toggleUp ? '收起' : '展开'}}</span>-->
          </div>
          <span @click="whereAmI" class="where-am-i" :class="{'is-mobile' : isMobile}">
            <i class="el-icon-location"></i>
            我在哪
        </span>
        </template>
        <template v-if="!isMobile">
          <div class="card">
            <h4>出行到达圈查询</h4>
            <div class="card-item">
              <span class="card-name">上班地点</span>
              <el-input
                  id="keyword"
                  class="card-value"
                  size="mini"
                  type="text"
                  placeholder="请输入内容"
                  v-model="keyword"
                  clearable>
              </el-input>
            </div>
            <div class="card-item">
              <span class="card-name">时长(分钟)</span>
              <el-slider class="card-value" :min="1" :max="60" v-model="times" size="mini"></el-slider>
              <span class="card-times">{{times}}</span>
            </div>
            <div class="card-item">
              <span class="card-name">出行方式</span>
              <el-select v-model="waysMethod" placeholder="请选择出行方式" class="card-value" size="mini">
                <el-option
                    label="地铁+公交"
                    value="">
                </el-option>
                <el-option
                    label="地铁"
                    value="SUBWAY">
                </el-option>
                <el-option
                    label="公交"
                    value="BUS">
                </el-option>
              </el-select>
            </div>
            <div class="card-item btn">
              <el-button type="primary" size="mini" @click="search" :loading="searching">查询</el-button>
              <el-button type="info" size="mini" :loading="searching" @click="next">下一页</el-button>
              <!--<el-button size="mini">清空</el-button>-->
            </div>
          </div>
          <span class="highlight-text">特此声明:房源信息来自网络，本网站不对其真实性负责。首次载入无数据可尝试【F5】强制刷新。 <a
              href="https://wj.qq.com/s/2953926/aabe" target="_blank" class="do-more-better">帮我们做得更好?</a></span>
          <div class="icon-tips">
            <ul>
              <!--<li class="btn">-->
              <!--<el-button type="success" size="medium" @click="toggleRight">上班导航</el-button>-->
              <!--</li>-->
              <li v-for="item in iconTips">
                <img :src="item.src"/>
                <span>{{item.text}}</span>
              </li>
            </ul>
          </div>
          <!--<div class="map-house-list">-->
          <!--<span class="toggleHouseList" @click="toggleHouseListUp = !toggleHouseListUp">{{toggleHouseListUp ? '收起' : '展开'}}</span>-->
          <!--<el-collapse-transition>-->
          <!--<house-list v-show="toggleHouseListUp" :house-list="mapHouseList" @click="houseListClick"-->
          <!--v-if="mapHouseList && mapHouseList.length"></house-list>-->
          <!--</el-collapse-transition>-->

          <!--</div>-->
        </template>
        <div class="container" id="map-container"></div>
      </div>
      <template v-if="isMobile">
        <div class="mobile-type" :class="{'in-map' : inMap}">
          <span @click="toMap" :class="{active: mobileType !== 'list'}">地图模式</span>
          <span @click="toList" :class="{active: mobileType === 'list'}">列表模式</span>
        </div>
        <template v-if="!inMap">
          <div class="filter">
            <div class="filter-item">
              <span>上班地点: </span>
              <el-input
                  id="keyword"
                  class="card-value"
                  size="mini"
                  placeholder="请输入内容"
                  v-model="keyword"
                  clearable>
              </el-input>
            </div>
            <div class="filter-item">
              <el-button type="primary" size="mini" :loading="searching" @click="next">下一页</el-button>
              <!--<el-button icon="el-icon-tickets" size="mini" type="text" @click="toList"-->
              <!--class="to-list"></el-button>-->
            </div>
          </div>
          <div class="mobile-bg" v-if="makerInfo" @click="handleMobileBg">
            <div class="content">
              <section>
                <span class="content-name">房源: </span>
                <a :href="`/#/detail/${makerInfo.id}`" target="_blank"
                   class="content-value">{{makerInfo.title}}</a>
              </section>
              <section v-if="makerInfo.displayMoney">
                <span class="content-value">{{makerInfo.displayMoney}}</span>
              </section>
              <section>
                <span class="content-value">{{makerInfo.sourceContent}}</span>
              </section>
              <section v-if="makerInfo.pictures && makerInfo.pictures.length">
                        <span class="content-name btn" @click="preview(makerInfo)"><i
                            class="el-icon-picture"></i>查看图片</span>
              </section>
              <section>
                        <span class="content-name btn" @click="collect(makerInfo)"><i
                            class="el-icon-star-on"></i>收藏</span>
              </section>
              <section>
                        <span class="content-name btn" @click="navTo(makerInfo)"><i
                            class="el-icon-location"></i>开始导航</span>
              </section>
            </div>
          </div>
        </template>

      </template>
    </div>

    <component v-if="view" :is="view"></component>
  </div>
</template>
<style scoped lang="scss">
  .location {
    cursor: pointer;
  }

  .filter-wrap {
    min-height: 40px;
    display: flex;
    border-bottom: 1px solid #efefef;
    border-top: 1px solid #efefef;
    padding: 0 10px;
    .displaySource {
      display: flex;
      width: auto;
      align-items: center;
      span {
        cursor: pointer;
        transition: color 0.3s;
        &.active,
        &:hover{
          color: #0D87F4;
        }
      }
    }
    .line {
      line-height: 28px;
      text-align: center;
    }
    ul {
      display: flex;
      width: 100%;
    }
    li {
      flex: auto;
      border-right: 1px solid #efefef;
      display: flex;
      font-size: 12px;
      align-items: center;
      padding: 0 8px;
      span {
        margin-right: 8px;
      }
      > div {
        width: 200px;
        margin-right: 10px;
      }
    }
  }

  .keyword-tag {
    .tag {
      margin-right: 10px;
    }
  }

  .button-new-tag {
    height: 32px;
  }

  .keyword-tag-input {
    width: 120px;
  }

  .link-to {
    color: #333;
    display: block;
  }

  header {
    min-height: 80px;
    display: flex;
    align-items: center;
    padding: 10px;
    .title {
      border-right: 1px solid #efefef;
      padding-right: 20px;
      margin-right: 20px;
      a {
        color: #0e90d2;
        font-size: 28px;
        font-weight: 600;
        letter-spacing: 7px;
        transition: all 0.5s;
        &:hover {
          color: #095f8a;
        }
      }
    }
  }

  .search-wrap {
    flex: auto;
    height: 44px;
    display: flex;
    align-items: center;
    .search-btn {
      margin-left: 20px;
    }
    .key-word {
      font-size: 14px;
      display: flex;
    }
  }

  .container-wrap {
    position: relative;
    flex: auto;
  }

  .to-list {
    color: #fff;
    font-size: 18px;
  }

  .highlight-text {
    position: absolute;
    color: red;
    left: 83px;
    top: 2px;
    font-weight: bold;
    z-index: 40;
  }

  .icon-tips {
    position: absolute;
    right: 20px;
    top: 30px;
    z-index: 66;
    li {
      margin-bottom: 10px;
      &:not(.btn) {
        pointer-events: none;
        -webkit-user-select: none;
        -moz-user-select: none;
        -ms-user-select: none;
        user-select: none;
      }
      &.btn {
        margin-bottom: 20px;
      }
    }
    span {
      margin-left: 10px;
      color: #333;
    }
  }

  .right {
    width: 0px;
    transition: all 0.3s;
    overflow: hidden;
    background: #333;
    padding: 0px;
    opacity: 0;
    &.show-right {
      opacity: 1;
      width: 300px;
      padding: 15px;
    }
    section {
      background: rgba(119, 136, 153, 0.8);
      padding: 10px;
      border-radius: 3px;
      color: #ccc;
      margin-bottom: 10px;
      .el-icon-circle-close {
        cursor: pointer;
      }
      p {
        margin: 0;
      }
      .nav {
        display: block;
        color: #ccc;
      }
      .sub-title {
        margin-bottom: 5px;
      }
      .text {
        color: #999;
      }
      .keyword-clean {
        display: none;
      }
      .clean {
        .el-input__suffix {
          display: block;
        }
      }
      .to {
        display: block;
        color: #ccc;
        transition: color 0.2s;
        &:hover {
          color: #a7a4a4;
        }
      }
    }
    .link {
      display: flex;
      flex-direction: row;
      flex-wrap: wrap;
      text-align: left;
      .to {
        flex: auto;
        &:nth-of-type(2n) {
          margin-left: 20px;
        }
      }
    }
    .commuting {
      display: flex;
      justify-content: flex-start;
      align-items: center;
      .text {
        margin-bottom: 0;
      }
    }
  }

  .card {
    display: flex;
    flex-direction: column;
    word-break: break-word;
    background: #fff;
    box-shadow: 0 2px 6px 0 rgba(114, 124, 245, .5);
    border-radius: 5px;
    position: absolute;
    left: 80px;
    top: 40px;
    z-index: 30;
    padding: 12px;
    h4 {
      color: grey;
      font-size: 13px;
      font-weight: normal;
    }
    .card-item {
      display: flex;
      font-size: 12px;
      align-items: center;
      &.btn {
        margin-top: 10px;
        text-align: center;
        justify-content: center;
      }
      .card-times {
        margin-left: 10px;
      }
      .card-value {
        flex: auto;
      }
      .card-name {
        min-width: 70px;
      }
    }
  }

  .mobile-type {
    height: 40px;
    position: fixed;
    z-index: 41;
    left: 0;
    width: 100%;
    background: #fff;
    top: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    text-align: center;
    &.in-map {
      z-index: 2555;
    }
    span {
      font-size: 12px;
      flex: 1;
      &.active {
        color: #409EFF;
      }
      &:first-of-type {
        border-right: 1px solid #eee;
      }
    }
  }

  .filter {
    position: fixed;
    z-index: 40;
    left: 0;
    top: 40px;
    width: 100%;
    background: rgba(0, 0, 0, 0.7);
    padding: 10px;
    display: flex;
    align-items: center;
    .filter-item {
      display: flex;
      flex-direction: row;
      font-size: 12px;
      align-items: center;
      &:nth-of-type(2n) {
        margin-left: 5px;
      }
      span {
        color: #fff;
        width: 70px;
      }
    }
  }

  .mobile-bg {
    position: fixed;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.7);
    z-index: 1000;
    left: 0;
    top: 0;
    .content {
      padding: 10px;
      height: 100%;
      max-height: 100%;
      overflow: auto;
      -webkit-overflow-scrolling: touch;
      width: 45%;
      position: absolute;
      right: 0;
      top: 0;
      background: #fff;
      border-top-left-radius: 3px;
      border-bottom-left-radius: 3px;
      .content-name {
        &.btn {
          display: block;
        }
      }
      section {
        padding: 15px 0;
        border-bottom: 1px solid #eee;
        font-size: 14px;
      }
    }
  }

  .where-am-i {
    position: fixed;
    z-index: 60;
    right: 20px;
    bottom: 20px;
    text-align: center;
    font-size: 12px;
    background: #fff;
    border-radius: 4px;
    padding: 10px;
    cursor: pointer;
    &.is-mobile {
      right: 18%;
    }
    i {
      display: block;
    }
  }

  .map-house-list {
    z-index: 67;
    max-height: 100%;
    overflow: auto;
    width: 260px;
    top: 40px;
    right: 178px;
    background: #fff;
    border-radius: 5px;
    .toggleHouseList {
      cursor: pointer;
      display: block;
      padding: 2px 10px;
      color: #fff;
      background: #409EFF;
    }
  }
</style>
<style lang="scss">
  .map {
    height: 100vh;
    width: 100%;
    display: flex;
    flex-direction: column;
    .map-content {
      flex: auto;
      width: 100%;
      display: flex;
    }
    &.showInfo {
      padding-top: 30px;
      .mobile-type {
        top: 27px
      }
      .filter {
        top: 67px;
      }
      .house-list.in-map {
        margin-top: 67px;
      }
    }
    .do-more-better {
      text-decoration: underline;
      font-size: #333;
    }
    .container {
      height: 100%;
      flex: auto;
    }
    .marker-info {
      i {
        margin-right: 4px;
      }
    }
    .marker-link {
      display: block;
      font-size: 12px;
      color: #0e90d2;
      text-decoration: none;
      cursor: pointer;
      &:hover {
        text-decoration: underline;
      }
    }
    .amap-icon {
      img {
        width: 19px;
      }
    }
    .amap-marker .marker-route {
      position: absolute;
      width: 40px;
      height: 44px;
      color: #e90000;
      background: url(https://webapi.amap.com/theme/v1.3/images/newpc/poi-1.png) no-repeat;
      cursor: pointer;
    }

    .amap-marker .marker-marker-bus-from {
      background-position: -334px -180px;
    }

    #panel {
      position: absolute;
      background-color: white;
      max-height: 80%;
      overflow-y: auto;
      top: 254px;
      left: 5%;
      width: 250px;
      border: solid 1px silver;
      -webkit-overflow-scrolling: touch;
      transition: all 0.3s;
      &.is-mobile {
        top: 100px;
      }
      .panel-handle {
        display: block;
        padding: 0 10px;
        cursor: pointer;
        background: #F56C6C;
        color: #fff;
      }
      &.slide-up {
        overflow: hidden;
        max-height: 25px;
      }
      &:empty {
        display: none;
      }
    }
  }
</style>
<script>
  import Vue from 'vue'
  import userInfo from './../components/user-info';
  import HouseList from './../components/house-list';

  const asyncComponent = require('./../components/async-component.js').default;

  export default {
    components: {
      HouseList
    },
    data() {
      return {
        form: {
          rentType: undefined,
          fromPrice: undefined,
          toPrice: undefined,
          source: undefined
        },
        inputVisible: false,
        rentTypeArr: [
          {
            label: '全部',
            value: undefined
          },
          {
            label: '未知',
            value: 0
          },
          {
            label: '合租',
            value: 1
          },
          {
            label: '单间',
            value: 2
          },
          {
            label: '整套出租',
            value: 3
          },
          {
            label: '公寓',
            value: 4
          },
        ],
        source: [],
        keywordTag: '',
        keywordArr: [],
        cities: [],
        toggleHouseListUp: true,
        mapHouseList: [],
        houseList: [],
        zoom: 12,
        info: undefined,
        markers: [],
        loading: true,
        cityName: '',
        myPosition: undefined,
        transferFn: undefined,
        iconTips: [
          {
            src: require('./../images/Blue.png'),
            text: '0-1000'
          },
          {
            src: require('./../images/PaleGreen.png'),
            text: '1000-2000'
          },
          {
            src: require('./../images/LightGreen.png'),
            text: '2000-3000'
          },
          {
            src: require('./../images/PaleYellow.png'),
            text: '3000-4000'
          },
          {
            src: require('./../images/OrangeYellow.png'),
            text: '4000-5000'
          },
          {
            src: require('./../images/PaleRed.png'),
            text: '5000-6000'
          },
          {
            src: require('./../images/Red.png'),
            text: '6000-7000'
          },
          {
            src: require('./../images/Pink.png'),
            text: '7000-8000'
          },
          {
            src: require('./../images/Violet.png'),
            text: '8000-9000'
          },
          {
            src: require('./../images/Black.png'),
            text: '9000+'
          },
        ],
        activeCityName: '',
        showRight: true,
        inMap: false,
        rightLinks: [
          {
            text: '上海互助租房',
            link: '?intervalDay=14&source=huzhuzufang&cityname=上海'
          },
          {
            text: '58同城诚信房源',
            link: '?intervalDay=14&source=&cityname=上海'
          },
          {
            text: '豆瓣租房(上海)',
            link: '?intervalDay=14&source=douban&cityname=上海'
          },
          {
            text: '豆瓣租房(北京)',
            link: '?intervalDay=14&source=douban&cityname=北京'
          },
          {
            text: '豆瓣租房(深圳)',
            link: '?intervalDay=14&source=douban&cityname=深圳'
          },
          {
            text: '豆瓣租房(广州)',
            link: '?intervalDay=14&source=douban&cityname=广州'
          }
        ],
        keyword: '',
        commuting: 1,
        transferWays: false,
        toggleUp: false,
        times: 60,
        waysMethod: '',
        map: undefined,
        lnglat: undefined,
        arrivalRange: undefined,
        polygons: [],
        positionPicker: undefined,
        loginVisible: false,
        searching: false,
        loginType: undefined,
        done: null,
        collection: false,
        makerInfo: undefined,
        view: undefined,
        location: '上海'
      }
    },
    computed: {
      sourceFilter() {
        let arr = this.source || [];
        if (arr.length > 6) {
          return arr.slice(0, 6)
        }
        return arr;
      },
      otherSource() {
        let arr = this.source || [];
        if (arr.length > 6) {
          return arr.slice(6, arr.length)
        }
        return arr;
      },
      mobileType() {
        let query = this.$route.query;
        if (query.mobileType && query.mobileType === 'list') {
          return 'list'
        } else {
          return undefined;
        }
      },
      keywordClean() {
        return !!this.keyword
      },
      isMobile() {
        return this.$store.state.isMobile
      },
      user() {
        return !!this.$store.state.userInfo
      }
    },
    watch: {
      '$route.query': function (params) {
        if ((!params.mobileType || params.mobileType !== 'list') && this.isMobile) {
          this.loading = true;
          this.init();
        }
      }
    },
    methods: {
      selectSource(source){
        if(this.form.source === source){return}
        this.form.source = source;
        this.refresh();
      },
      resetFilter() {
        this.form = {
          rentType: undefined,
          fromPrice: undefined,
          toPrice: undefined,
          source: ''
        };
        this.refresh();
      },
      async cityChange(cityName) {
        const data = await this.$v2.get(`/cities/${cityName}`);
        this.source = data.data;
        this.form.source = '';
      },
      removeKeyword(item) {
        this.keywordArr.splice(this.keywordArr.indexOf(item), 1);
      },
      transformParams() {
        let params = Object.assign({}, this.form);
        if (this.keywordArr.length) {
          params.keyword = this.keywordArr.join(',');
        }
        params.city = this.location;
        return params;
      },
      refresh() {
        let query = this.$route.query;
        let params = Object.assign({}, query, this.transformParams());
        this.$router.push({query: params});
        setTimeout(() => {
          this.init(this.location);
        }, 200);
      },
      cityLocation(item) {
        this.location = item;
        this.cityChange(item);
        this.refresh();
      },
      keywordConfirm() {
        let inputValue = this.keywordTag;
        if (inputValue) {
          let has = this.keywordArr.find(item => {
            return item === inputValue
          });
          if (!has) {
            this.keywordArr.push(inputValue);
          }
        }
        this.inputVisible = false;
        this.keywordTag = '';
      },
      showInput() {
        this.inputVisible = true;
        this.$nextTick(_ => {
          this.$refs.saveTagInput.$refs.input.focus();
        })
      },
      changeQuery(q) {
        const query = this.$route.query;
        const params = Object.assign({}, query, q);
        this.$router.push({
          query: params
        })
      },
      toMap() {
        this.view = undefined;
        this.changeQuery({
          mobileType: undefined
        });
        this.inMap = false;
      },
      async toList() {
        let com = require('./../components/mobile/house-list').default;
        try {
          let params = {
            ...this.$route.query,
            city: this.cityName,
            houseCount: 180,
            page: 1
          };
          await asyncComponent(com, {
            props: {
              list: this.houseList,
              inMap: true,
              params
            }
          }, (template) => {
            this.inMap = true;
            this.view = template;
            this.changeQuery({
              mobileType: 'list'
            })
          });
          this.view = undefined;
          this.inMap = false;
          this.changeQuery({
            mobileType: undefined
          })
        } catch (e) {
          this.view = undefined;
          this.inMap = false;
          this.changeQuery({
            mobileType: undefined
          });
          return
        }
      },
      whereAmI() {
        this.positionPicker.start(this.lnglat)
      },
      next() {
        const query = this.$route.query;
        let page = 0;
        if (!query.page) {
          page = 1;
        } else {
          page = (+query.page) + 1;
        }
        const params = Object.assign({}, query, {page});
        this.$router.push({
          query: params
        })
      },
      handleMobileBg(e) {
        if (e.target === e.currentTarget) {
          this.makerInfo = undefined;
        }
      },
      async getCities() {
        const data = await this.$v2.get('/cities?fields=id,city,sources&index=0&count=15');
        this.cities = data.data.map(item => {
          return item.city
        })
      },
      navTo(item) {
        this.transfer(item.geocodes.location, this.map, this);
        this.makerInfo = undefined;
      },
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
        this.makerInfo = undefined;
        if (this.isMobile) {
          alert(data.message)
        } else {
          this.$message.success(data.message);
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
      async preview(item) {
        let self = this;
        let com = require('../components/preview-image').default;
        try {

          await asyncComponent(com, {
            props: {
              images: item.pictures,
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
      },
      search(type) {
        let self = this;
        self.searching = true;
        self.arrivalRange = new AMap.ArrivalRange();
        self.arrivalRange.search(self.lnglat, self.times, function (status, result) {
          self.map.remove(self.polygons);
          self.polygons = [];
          if (result.bounds) {
            for (var i = 0; i < result.bounds.length; i++) {
              var polygon = new AMap.Polygon({
                fillColor: "#3366FF",
                fillOpacity: "0.4",
                strokeColor: "#00FF00",
                strokeOpacity: "0.5",
                strokeWeight: 1
              });
              polygon.setPath(result.bounds[i]);
              self.polygons.push(polygon);
            }
            self.map.add(self.polygons);
            self.map.setFitView();
            self.map.setZoomAndCenter(13, self.lnglat);
            self.searching = false;
            if (gtag && type !== 'auto') {
              gtag('event', '出行到达圈查询(地点)', {
                'event_category': self.keyword
              });
              gtag('event', '出行到达圈查询(时间)', {
                'event_category': self.times
              });

              let text = '地铁加公交';
              if (self.waysMethod === 'SUBWAY') {
                text = '地铁';
              } else if (self.waysMethod === '') {
                text = '公交';
              }
              gtag('event', '出行到达圈查询(方式)', {
                'event_category': text
              });

            }
          } else {
            console.log('未知错误')
            // self.$message.error('未知错误');
            self.searching = false;
          }
        }, {
          policy: self.waysMethod
        });
      },
      houseListClick(item) {
        if (item.geocodes && item.geocodes.location) {
          this.map.setZoomAndCenter(16, item.geocodes.location);
          // this.positionPicker.start(item.geocodes.location)
        }
      },
      toggleRight() {
        this.showRight = !this.showRight
      },
      async getList() {
        let houseCount = undefined;
        if (this.isMobile) {
          houseCount = 180;
        }
        let params = {
          ...this.$route.query,
          city: this.cityName.replace(/市/ig, ''),
          houseCount
        };
        delete params.cityname;
        let data = await this.$v2.post('/houses', params);
        this.houseList = data.data;
        return data;
      },
      transfer(position, map, self) {
        if (this.transferFn) {
          this.transferFn.clear();
        }
        this.transferFn = new AMap.Transfer({
          hideMarkers: false,
          city: this.cityName, // 必须值，搭乘公交所在城市
          map: map, // 可选值，搜索结果的标注、线路等均会自动添加到此地图上
          panel: "panel", // 可选值，显示搜索列表的容器,
          extensions: "all", // 可选值，详细信息
          poliy: AMap.TransferPolicy.LEAST_DISTANCE // 驾驶策略：最省时模式
        });

        this.transferFn.search([this.myPosition.lng, this.myPosition.lat], [position.lng, position.lat], function (status, result) {
          self.transferWays = true;
          let panelHandle = document.querySelector('.panel-handle');
          self.toggleUp = false;
          if (!panelHandle) {
            const panelComponent = Vue.extend({
              render(h) {
                return h('span', {
                  class: ['panel-handle'],
                  on: {
                    click() {
                      self.toggleUp = !self.toggleUp
                    }
                  }
                }, !self.toggleUp ? '收起' : '展开')
              }
            });

            const component = new panelComponent().$mount();
            document.querySelector(`#panel`).appendChild(component.$el)

          }

        });

      },
      installAmapUI(map, self) {
        return new Promise((resolve, reject) => {
          AMapUI.loadUI(['misc/PositionPicker'], function (PositionPicker) {
            let positionPicker = new PositionPicker({
              mode: 'dragMarker',//设定为拖拽地图模式，可选'dragMap'、'dragMarker'，默认为'dragMap'
              zIndex: 100000,
              map: map, //依赖地图对象
            });


            positionPicker.on('success', function (positionResult) {
              self.myPosition = positionResult.position;
              self.positionPicker = positionPicker;
              self.lnglat = positionResult.position;
              self.keyword = positionResult.address
            });

            // positionPicker.start();

            resolve(positionPicker);
          });
        })
      },
      installPlugin(map, self) {
        return new Promise((resolve, reject) => {
          AMap.plugin(['AMap.ToolBar', 'AMap.Driving', 'AMap.LineSearch', `AMap.StationSearch`, 'AMap.Transfer', 'AMap.Walking', 'AMap.Riding', 'AMap.Geolocation', 'AMap.Autocomplete', 'AMap.PlaceSearch'], function () {//异步同时加载多个插件
            let toolbar = new AMap.ToolBar();
            map.addControl(toolbar);

            let options = {
              'showMarker': false,//是否显示定位按钮
            };


            resolve()

            // let geolocation = new AMap.Geolocation(options);
            // map.addControl(geolocation);
            // geolocation.getCurrentPosition((status, result) => {
            //     if (status === 'complete') {
            //         self.myPosition = result.position;
            //         let marker = new AMap.Marker({ //添加自定义点标记
            //             map: map,
            //             zIndex: 100000,
            //             position: [result.position.lng, result.position.lat], //基点位置
            //             offset: new AMap.Pixel(-17, -42), //相对于基点的偏移位置
            //             draggable: true,  //是否可拖动
            //             content: '<div class="marker-route marker-marker-bus-from"></div>'   //自定义点标记覆盖物内容
            //         });
            //         marker.on('dragend', function (ev) {
            //             self.myPosition = ev.lnglat;
            //         })
            //     }
            // });


          });
        })
      },
      getActiveCityName(self) {
        return new Promise((resolve, reject) => {
          let citysearch = new AMap.CitySearch();
          citysearch.getLocalCity(function (status, result) {
            if (status === 'complete' && result.info === 'OK') {
              if (result && result.city && result.bounds) {
                let cityinfo = result.city;
                let citybounds = result.bounds;
                resolve(cityinfo)
              }
            } else {
              this.$message.error(result.info)
            }
          });

        })
      },
      getLocation(code, item) {
        return new Promise((resolve, reject) => {
          code.getLocation(item.location, (status, result) => {
            resolve({
              status,
              result
            })
          })
        });
      },
      addMaker(map, data, code, self) {
        return new Promise(async (resolve, reject) => {
          let infoWindow = new AMap.InfoWindow({
            offset: new AMap.Pixel(0, -30),
            content: ``  //传入 dom 对象，或者 html 字符串
          });
          let list = await Promise.all(data.map(function (item) {
            return new Promise(async (resolve, reject) => {
              if (!item) {
                resolve(item)
              }
              try {
                let status = undefined;
                let result = undefined;

                if (item.latitude && item.longitude) {
                  status = 'complete';
                  result = {
                    info: 'ok',
                    geocodes: [{
                      location: {
                        lng: +item.longitude,
                        lat: +item.latitude,
                        O: +item.latitude,
                        P: +item.longitude
                      }
                    }]
                  }
                } else {
                  let location = await self.getLocation(code, item);
                  status = location.status;
                  result = location.result;
                }

                if (status === "complete" && result.info && result.info.toLocaleLowerCase() === 'ok') {

                  let icon = 'https://webapi.amap.com/theme/v1.3/markers/n/mark_b.png';
                  if (item.icon) {
                    icon = require('./../images/' + (item.icon));
                  }

                  let marker = new AMap.Marker({
                    map: map,
                    title: item.title,
                    icon,
                    position: [result.geocodes[0].location.lng, result.geocodes[0].location.lat]
                  });


                  map.add(marker);

                  let displayMoney = item.price > 0 ? "租金：" + item.price : "";

                  let sourceContent = item.source ? " 来源：" + item.source : "";
                  let title = item.title;
                  if (!title) {
                    title = item.location;
                  }

                  let params = {
                    ...item,
                    displayMoney,
                    sourceContent,
                    title,
                    geocodes: result.geocodes[0],
                    marker
                  };

                  self.mapHouseList.push(params);

                  marker.on('click', function (e) {

                    if (self.isMobile) {
                      self.makerInfo = params
                    } else {
                      const makerInfoComponent = Vue.extend({
                        render(h) {
                          return h('div', {
                            class: ['marker-info']
                          }, [
                            h('a', {
                              attrs: {
                                target: '_blank',
                                href: `/#/detail/${item.id}`
                              },
                              class: ['marker-link'],
                              domProps: {
                                innerHTML: `房源: ${title}`
                              }
                            }),
                            h('a', {
                              attrs: {
                                target: '_blank',
                                href: `/#/detail/${item.id}`
                              },
                              class: ['marker-link'],
                              domProps: {
                                innerHTML: `${displayMoney}`
                              }
                            }),
                            h('a', {
                              attrs: {
                                target: '_blank',
                                href: `/#/detail/${item.id}`
                              },
                              class: ['marker-link'],
                              domProps: {
                                innerHTML: `${sourceContent}`
                              }
                            }),
                            h('span', {
                              style: {
                                display: item.pictures && item.pictures.length ? 'block' : 'none',
                              },
                              class: ['marker-info', 'marker-link'],
                              on: {
                                click: async function (e) {
                                  self.preview(item)
                                }
                              }
                            }, [
                              h('i', {
                                class: ['el-icon-picture']
                              }),
                              '查看图片'
                            ]),
                            h('span', {
                              class: ['marker-info', 'marker-link'],
                              on: {
                                click: async function (e) {
                                  self.collect(item)
                                }
                              }
                            }, [
                              h('i', {
                                class: ['el-icon-star-on']
                              }),
                              '收藏',
                            ]),
                            h('span', {
                              class: ['marker-info', 'marker-link'],
                              on: {
                                click: function (e) {
                                  self.transfer(result.geocodes[0].location, map, self);
                                  infoWindow.close();
                                }
                              }
                            }, [
                              h('i', {
                                class: ['el-icon-location'],
                              }),
                              '开始导航'
                            ])
                          ])
                        }
                      });

                      const component = new makerInfoComponent().$mount();

                      infoWindow.setContent(component.$el);
                      infoWindow.open(map, e.target.getPosition());
                    }

                  });

                  resolve(marker)
                } else {
                  resolve(item);
                  //reject(new Error('找不到坐标'))
                }
                // code.getLocation(item.location, (status, result) => {
                // })
              } catch (e) {
                console.log(e);
                reject(item);
                throw e
              }
            })
          }));
          resolve(list)
        })
      },
      keywordSelect(map, positionPicker) {
        let auto = new AMap.Autocomplete({
          input: "keyword"
        });
        let placeSearch = new AMap.PlaceSearch({
          map: map
        });
        let self = this;

        AMap.event.addListener(auto, "select", select);//注册监听，当选中某条记录时会触发
        function select(e) {
          positionPicker.start(e.poi.location);
          self.lnglat = e.poi.location;
          if (self.isMobile) {
            self.search()
          }
          // placeSearch.setCity(e.poi.adcode);
          // placeSearch.search(e.poi.name);  //关键字查询查询
        }
      },
      getCityCenter(positionPicker, code, self, name, cb) {
        let location = undefined;
        return new Promise((resolve, reject) => {
          code.getLocation(name, (status, result) => { // 城市中心点
            if (status === "complete" && result.info === 'OK') {
              location = result.geocodes[0].location;
              positionPicker.start(location);
              self.lnglat = location;
              // self.keyword = result.geocodes[0].formattedAddress;
            } else {

              if (cb) {
                cb();
              } else {
                location = self.map.getBounds().getSouthWest();
                self.lnglat = location;
                // self.keyword = self.cityName;
                positionPicker.start(location)
              }

            }
            resolve()
          });
        })
      },

      async init(location) {
        // const loading = this.$loading({
        //   lock: true,
        //   text: '正在加载数据,若等待时间过长,请重新刷新页面',
        //   spinner: 'el-icon-loading',
        //   background: 'rgba(0, 0, 0, 0.7)'
        // });
        this.getCities();
        try {
          const query = this.$route.query;
          let cityName = query.city;
          if (!cityName) {
            cityName = await this.getActiveCityName(this);
          }
          if (location) {
            this.activeCityName = this.cityName = location;
          } else {
            this.activeCityName = this.cityName = cityName;
          }

          this.mapHouseList = [];
          let map = new AMap.Map('map-container', {
            zoom: this.zoom,
            resizeEnable: true,
          });
          this.map = map;
          map.clearMap();
          map.setCity(this.cityName);
          let self = this;

          await this.installPlugin(map, self);
          let positionPicker = await this.installAmapUI(map, self);

          let code = new AMap.Geocoder({
            city: this.cityName,
            radius: 1000
          });


          if (this.$store.state.userInfo && this.$store.state.userInfo.workAddress) {
            await this.getCityCenter(positionPicker, code, self, this.$store.state.userInfo.workAddress, async () => {
              await this.getCityCenter(positionPicker, code, self, this.cityName);
            });
          } else {
            await this.getCityCenter(positionPicker, code, self, this.cityName);
          }

          this.keywordSelect(map, positionPicker);

          let info = await this.getList();
          if (this.mobileType === 'list' && this.isMobile) {
            this.toList();
            return;
          }
          let data = info.data;

          if (process.env.NODE_ENV === "development") {
            data.length = 20;
          }

          // this.showRight = true;


          // setTimeout(() => {
          //   loading.close();
          // }, 1000 * 20);
          await this.addMaker(map, data, code, self);
          setTimeout(() => {
            this.search('auto');
          });
          // loading.close();
          this.loading = false;
        } catch (e) {
          this.$message.error('初始化地图失败,请再次刷新');
          throw e
        }
      },

      appendScript(url) {
        return new Promise((resolve) => {
          let jsapi = document.createElement('script');
          jsapi.charset = 'utf-8';
          jsapi.src = url;
          document.head.appendChild(jsapi);
          jsapi.onload = () => {
            resolve()
          };
          jsapi.onerror = () => {
            this.$message.error('地图初始化失败,请重新刷新页面')
          }
        })
      }
    },
    created() {
      let query = this.$route.query;
      let keyword = query.keyword;
      if (keyword) {
        this.keywordArr = keyword.split(',')
      }
    },
    async mounted() {
      this.loading = true;

      const query = this.$route.query;
      let cityName = query.city;
      if (cityName) {
        this.location = cityName;
        this.cityChange(cityName)
      } else {
        if (returnCitySN) {
          let str = returnCitySN.cname;
          str = str.match(/省(\S*)市/)[1];
          this.location = str;
          this.cityChange(str);
          this.$router.replace({query: {city: str}});
          window.location.reload();
          return;
        }
      }

      let key = `8a971a2f88a0ec7458d43b8bc03b6462`;
      let plugin = `AMap.ArrivalRange,AMap.Scale,AMap.Geocoder,AMap.Transfer,AMap.Autocomplete,AMap.CitySearch,AMap.Walking`.split();
      plugin.push(`AMap.ToolBar`);
      let url = `https://webapi.amap.com/maps?v=1.4.8&key=${key}&plugin=${plugin.join()}`;

      userInfo(this);
      await this.appendScript(url);
      await this.appendScript(`//webapi.amap.com/ui/1.0/main.js?v=1.0.11`);

      gtag('event', '进入地图页');
      this.init();

    }
  }
</script>
