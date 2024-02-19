<template>
  <el-dialog
    title="地图搜租房"
    width="100%"
    center
    :fullscreen="true"
    :visible="visible"
    :append-to-body="appendToBody"
    :before-close="cancel"
    class="house-list"
    :show-close="!inMap"
    :class="{'in-map' : inMap}"
  >
    <div class="house-list-wrap">
      <div class="filter-nav">
        <span :class="{active:!filterType}" @click="sort('')">默认排序</span>
        <span
          :class="{active:filterType === 'top' || filterType === 'bottom'}"
          @click="sort(filterType === 'top'?'bottom':'top')"
        >
          价格排序
          <i v-if="filterType === 'bottom'" class="el-icon-caret-bottom"></i>
          <i v-else class="el-icon-caret-top"></i>
        </span>
      </div>
      <ul class="list" v-if="list && list.length" ref="list">
        <li v-for="item in list" :key="`${item.id}-${item.source}`">
          <div class="left">
            <template v-if="item.pictures && item.pictures.length">
              <!--<transition name="el-fade-in">-->
              <!--<i v-show="!imagesLoadingMap[item.pictures[0]]"-->
              <!--class="el-icon-loading loading-icon"></i>-->
              <!--</transition>-->
              <transition name="el-fade-in">
                <img v-lazy="item.pictures[0]" class="lazyImage">
              </transition>
              <!--<transition name="el-fade-in">-->
              <!--<img :src="item.pictures[0]"-->
              <!--v-show="imagesLoadingMap[item.pictures[0]]"-->
              <!--@load="imageLoading(item.pictures[0],imagesLoadingMap)"/>-->
              <!--</transition>-->
            </template>
          </div>
          <div class="right">
            <div class="content">
              <!--<a class="title" :href="`/#/detail/${item.id}`" target="_blank">{{item.title ?-->
              <!--item.title : item.location}}</a>-->
              <router-link :to="`/detail/${item.id}`" tag="a" class="title">
                <el-tag size="mini" :type="tagType(item)">{{tagText(item)}}</el-tag>
                {{item.title ?
                item.title : item.location}}
              </router-link>
              <div class="price" v-if="item.price > 0">
                {{item.price}}
                <span>/月</span>
              </div>
              <div class="labels" v-html="labels(item.labels)"></div>
            </div>
            <div class="source">来源: {{item.source}}</div>
          </div>
        </li>
        <li v-if="loading" class="text-center loading">
          <i class="el-icon-loading"></i>
        </li>
        <li v-if="over" class="text-center loading">
          <span>没有更多数据了</span>
        </li>
      </ul>
      <div v-else class="text-center">暂无数据</div>
    </div>
  </el-dialog>
</template>
<style lang="scss" scoped>
.loading {
  background: transparent !important;
  justify-content: center;
  margin-bottom: 0 !important;
  min-height: auto !important;
  font-size: 12px;
}

.text-center {
  text-align: center;
  padding: 5px 0;
}

.house-list {
  &.in-map {
    margin-top: 40px;
    .list {
      max-height: 76vh;
    }
  }
  /deep/ .el-dialog__body {
    padding: 0;
    background: rgb(248, 248, 248);
  }
}

.labels {
  font-size: 12px;
  padding-bottom: 15px;
}
</style>
<style lang="scss" scoped>
@import "./../../scss/house-list";
.house-list-wrap {
  display: flex;
  flex-direction: column;
  .filter-nav {
    font-size: 12px;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 5px 0;
    border-bottom: 1px solid #dcdfe6;
    span {
      cursor: pointer;
      padding: 0 5px;
      position: relative;
      transition: color 0.2s;
      -webkit-user-select: none;
      -moz-user-select: none;
      -ms-user-select: none;
      user-select: none;
      &:hover,
      &.active {
        color: #409eff;
      }
      &:not(:last-of-type):after {
        content: "";
        position: absolute;
        right: 0;
        top: 50%;
        transform: translateY(-50%);
        width: 1px;
        height: 60%;
        background: #dcdfe6;
      }
    }
  }
}
</style>
<script>
import { tagText, tagType } from "../house-type";

export default {
  props: {
    appendToBody: {
      default: false
    },
    isMobile: {
      default: false
    },
    inMap: {
      default: false
    },
    _houseList: {
      default() {
        return [];
      }
    },
    params: {}
  },
  computed: {
    list() {
      let list = this.houseList.map(item => item);

      if (this.filterType === "top" || this.filterType === "bottom") {
        if (this.filterType === "top") {
          list.sort((a, b) => {
            return a.price - b.price;
          });
        } else {
          list.sort((a, b) => {
            return b.price - a.price;
          });
        }
      } else {
        list = this.houseList.map(item => item);
      }
      return list;
    }
  },
  data() {
    return {
      filterType: undefined,
      imagesLoadingMap: {},
      visible: true,
      houseList: this._houseList,
      loading: false,
      query: this.params,
      over: false,
      observer: undefined
    };
  },
  methods: {
    sort(type) {
      this.filterType = type;
    },
    tagText(item) {
      return tagText(item);
    },
    tagType(item) {
      return tagType(item);
    },
    init() {
      let list = this.$refs.list;
      if (!list) {
        setTimeout(() => {
          this.init();
        }, 200);
      } else {
        // this.lazyImage();
        list.addEventListener("scroll", this.lazyLoad);
      }
    },
    async lazyLoad(e) {
      if (this.query && !this.loading && !this.over) {
        let target = e.target;
        let scrollTop = target.scrollTop;
        let offsetHeight = target.offsetHeight;
        let scrollHeight = target.scrollHeight;
        if (scrollTop + offsetHeight + 200 >= scrollHeight) {
          this.loading = true;
          this.getHouseList();
        }
      }
    },
    lazyImage() {
      const config = {
        rootMargin: "50px 0px",
        threshold: 0.01
      };
      let self = this;

      function intersection(entries) {
        entries.forEach(entry => {
          if (entry.intersectionRatio > 0) {
            self.observer.unobserve(entry.target);
            // console.log(entry.target)
            // entry.target.setAttribute('src',entry.target.getAttribute('data-src'))
          }
        });
      }

      let images = document.querySelectorAll(".lazyImage");

      if (!this.observer) {
        this.observer = new IntersectionObserver(intersection, config);
      }
      images.forEach(image => {
        this.observer.observe(image);
      });
    },
    async getHouseList() {
      let params = this.query;
      let page = params.page ? params.page : 0;
      page += 1;
      let query = {
        ...params,
        page
      };
      const data = await this.$ajax.post("v2/houses", query);
      this.query = query;
      let list = data.data;
      this.loading = false;
      if (list && list.length) {
        this.houseList.push(...list);
        // this.lazyImage();
      } else {
        this.over = true;
      }
    },
    close() {
      this.visible = false;
    },
    cancel() {
      this.close();
      this.$emit("cancel", false);
    },
    labels(label) {
      let html = "";
      if (label) {
        let words = label.split("|");
        if (words.length > 1) {
          html += `${words[0]}<br>`;
          words.shift();
        }

        html += `${words.join(",")}`;
      }
      return html;
    }
  },
  created() {},
  mounted() {
    this.init();
  },
  destroyed() {
    let list = this.$refs.list;
    list && list.removeEventListener("scroll", this.lazyLoad);
  }
};
</script>