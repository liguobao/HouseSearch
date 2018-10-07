<template>
  <div>
    <ul class="list" v-if="houseList && houseList.length" ref="list">
      <li @click="handleClick(item)" v-for="item in houseList" :key="`${item.id}-${item.source}`">
        <!--<div class="left">-->
          <!--<template v-if="item.pictures && item.pictures.length">-->
            <!--<transition name="el-fade-in">-->
              <!--<i v-show="!imagesLoadingMap[item.pictures[0]]"-->
                 <!--class="el-icon-loading loading-icon"></i>-->
            <!--</transition>-->
            <!--<transition name="el-fade-in">-->
              <!--<img :src="item.pictures[0]"-->
                   <!--v-show="imagesLoadingMap[item.pictures[0]]"-->
                   <!--@load="imageLoading(item.pictures[0],imagesLoadingMap)"/>-->
            <!--</transition>-->
          <!--</template>-->
        <!--</div>-->
        <div class="right">
          <div class="content">
            <a class="title" href="javascript:;"  :title="item.houseTitle ?
              item.houseTitle : item.houseLocation">{{item.houseTitle ?
              item.houseTitle : item.houseLocation}}</a>
            <div class="price" v-if="item.disPlayPrice">
              {{item.disPlayPrice}}<span> /月</span>
            </div>
          </div>
          <div class="source">
            来源: {{item.displaySource}}
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
    <div v-else class="text-center">暂无数据</div>
  </div>
</template>
<style lang="scss" scoped>
  @import "./../scss/house-list";
</style>
<script>
  export default {
    props: {
      houseList: {
        default() {
          return []
        }
      }
    },
    data() {
      return {
        over: false,
        loading: false,
        imagesLoadingMap: {},
      }
    },
    methods: {
      handleClick(item) {
        this.$emit('click',item)
      }
    }
  }
</script>