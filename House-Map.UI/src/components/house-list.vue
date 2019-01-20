<template>
  <div>
    <ul class="list" v-if="houseList && houseList.length" ref="list" :style="{'max-height': height ? height :'89vh'}">
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
        <div class="left">
          <template v-if="item.pictures && item.pictures.length">
            <transition name="el-fade-in">
              <img
                   v-lazy="item.pictures[0]"
                  />
            </transition>
          </template>
        </div>
        <div class="right">
          <div class="content">
            <a class="title" href="javascript:;"  :title="item.title ?
              item.title : item.location">{{item.title ?
              item.title : item.location}}</a>
            <div class="price" v-if="item.price > 0">
              {{item.price}}<span> /月</span>
            </div>
          </div>
          <!--<div class="source">-->
            <!--来源: {{item.displaySource}}-->
          <!--</div>-->
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
  .content{
    .title{
      white-space:normal;
    }
  }
</style>
<script>
  export default {
    props: {
      height:{},
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