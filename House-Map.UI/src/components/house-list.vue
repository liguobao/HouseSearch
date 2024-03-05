<template>
  <div class="list-wrap">
       <div class="filter">
        <el-input placeholder="关键词" 
        prefix-icon="el-icon-search" 
        @input="searchKeyword" 
        v-model="keyword" style="margin: 10px;"></el-input>
       </div>
    <ul
      class="list"
      v-if="list && list.length"
      ref="list"
      :style="{'max-height': height ? height :'89vh'}"
    >
      <li @click="handleClick(item)" v-for="item in list" :key="`${item.id}-${item.source}`">
        <div class="left">
          <template v-if="item.pictures && item.pictures.length">
            <transition name="el-fade-in">
              <img v-lazy="item.pictures[0]">
            </transition>
          </template>
        </div>
        <div class="right">
          <div class="content">
            <span class="title" :title="item.title ?
              item.title : item.location">
              <el-tag size="mini" :type="tagType(item)">{{tagText(item)}}</el-tag>
              {{item.title ?
              item.title : item.location}}
            </span>
            <div class="publishDate">{{item.publishDate}}</div>
            <div class="price" v-if="item.price > 0">
              {{item.price}}
              <span>/月</span>
            </div>
          </div>
        </div>
      </li>
      <li v-if="loading" class="text-center loading">
        <i class="el-icon-loading"></i>
      </li>
      <li v-if="over" class="text-center loading">
        <span>没有更多数据了</span>
      </li>
    </ul>
    <div class="loading-list" v-else-if="loading">
      <i class="el-icon-loading"></i>
    </div>
    <div v-else class="text-center empty">暂无数据</div>
  </div>
</template>
<style lang="scss" scoped>
@import "./../scss/house-list";

.content {
  .title {
    white-space: normal;
  }
}

.list-wrap {
  height: 100%;
  width: 100%;
  position: relative;
  display: flex;
  flex-direction: column;

  .filter {
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

.empty {
  display: flex;
  align-items: center;
  justify-content: center;
  color: #aaaaaa;
  font-size: 16px;
  margin-top: 30px;
}

.list li.loading {
  display: block;
}

.loading-list {
  position: absolute;
  left: 0;
  top: 0;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #409eff;
  font-size: 26px;
}
</style>
<script>
import { tagText, tagType } from "./house-type";

export default {
  props: {
    height: {},
    houseList: {
      default() {
        return [];
      }
    }
  },
  computed: {
    // list() {
    //   let list = this.houseList.map(item=>item);
    //   if (this.filterType === 'top' || this.filterType === 'bottom') {
    //     if (this.filterType === 'top') {
    //       list.sort((a, b) => {
    //         return a.price - b.price
    //       })
    //     } else {
    //       list.sort((a, b) => {
    //         return b.price - a.price
    //       })
    //     }
    //   }else {
    //     list = this.houseList.map(item=>item);
    //   }
    //   return list;
    // }
  },
  watch: {
    houseList: function(n, o) {
      this.cut(n);
    }
  },
  data() {
    return {
      over: false,
      imagesLoadingMap: {},
      filterType: undefined,
      loading: false,
      list: [],
      cutArr: [],
      index: 0,
      keyword: undefined
    };
  },
  methods: {
    sort(type) {
      this.filterType = type;
    },
    searchKeyword(keyword) {
      this.keyword = keyword;
      if (keyword) {
        this.list = this.houseList.filter(item => {
          return (
            item.title.indexOf(keyword) > -1 || item.location.indexOf(keyword) > -1);
        });
      } else {
        this.cut(this.houseList);
      }
    
    },
    handleClick(item) {
      this.$emit("click", item);
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
        list.addEventListener("scroll", this.lazyLoad);
      }
    },
    async lazyLoad(e) {
      if (!this.loading && !this.over) {
        let target = e.target;
        let scrollTop = target.scrollTop;
        let offsetHeight = target.offsetHeight;
        let scrollHeight = target.scrollHeight;
        if (scrollTop + offsetHeight + 20 >= scrollHeight) {
          this.loading = true;
          this.getOtherList();
        }
      }
    },
    cut(data) {
      let cutArr = [];
      for (let i = 0, len = data.length; i < len; i += 50) {
        cutArr.push(data.slice(i, i + 50));
      }
      this.index = 0;
      if (cutArr.length) {
        this.list = cutArr[0];
      }
      this.cutArr = cutArr;
    },
    getOtherList() {
      let index = this.index + 1;
      if (this.cutArr[index]) {
        this.list.push(...this.cutArr[index]);
        this.index++;
      } else {
        this.over = true;
      }
      this.loading = false;
    }
  },
  mounted() {
    this.init();
  },
  destroyed() {
    let list = this.$refs.list;
    list && list.removeEventListener("scroll", this.lazyLoad);
  }
};
</script>
