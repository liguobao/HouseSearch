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
    <div class="">

      <ul class="list" v-if="houseList && houseList.length" ref="list">
        <li v-for="item in houseList" :key="`${item.id}-${item.source}`">
          <div class="left">
            <template v-if="item.pictures && item.pictures.length">
              <!--<transition name="el-fade-in">-->
                <!--<i v-show="!imagesLoadingMap[item.pictures[0]]"-->
                   <!--class="el-icon-loading loading-icon"></i>-->
              <!--</transition>-->
              <transition name="el-fade-in">
                <img
                    v-lazy="item.pictures[0]"
                    class="lazyImage"
                />
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
              <router-link :to="`/detail/${item.id}`" tag="a" class="title">{{item.title ?
                item.title : item.location}}</router-link>
              <div class="price" v-if="item.price > 0">
                {{item.price}}<span> /月</span>
              </div>
              <div class="labels" v-html="labels(item.labels)">

              </div>
            </div>
            <div class="source">
              来源: {{item.source}}
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
        max-height: 87vh;
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
</style>
<script>
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
      list: {
        default() {
          return []
        }
      },
      params: {}
    },
    data() {
      return {
        imagesLoadingMap: {},
        visible: true,
        houseList: this.list,
        loading: false,
        query: this.params,
        over: false,
        observer: undefined
      }
    },
    methods: {
      init() {
        let list = this.$refs.list;
        if (!list) {
          setTimeout(() => {
            this.init();
          }, 200)
        } else {
          // this.lazyImage();
          list.addEventListener('scroll', (e) => {
            this.lazyLoad(e);
          })
        }
      },
      async lazyLoad(e) {
        if (this.query && !this.loading && !this.over) {
          let target = e.target;
          let scrollTop = target.scrollTop;
          let offsetHeight = target.offsetHeight;
          let scrollHeight = target.scrollHeight;
          if ((scrollTop + offsetHeight + 200) >= scrollHeight) {
            this.loading = true;
            this.getHouseList();
          }
        }
      },
      lazyImage() {
        const config = {
          rootMargin: '50px 0px',
          threshold: 0.01
        };
        let self = this;

        function intersection(entries) {
          entries.forEach(entry => {
            if (entry.intersectionRatio > 0) {
              self.observer.unobserve(entry.target);
              console.log(entry.target)
              // entry.target.setAttribute('src',entry.target.getAttribute('data-src'))
            }
          })
        }

        let images = document.querySelectorAll('.lazyImage');

        if (!this.observer) {
          this.observer = new IntersectionObserver(intersection, config);
        }
        images.forEach(image => {
          this.observer.observe(image)
        })
      },
      async getHouseList() {
        let params = this.query;
        let page = params.page ? params.page : 0;
        page += 1;
        let query = {
          ...params,
          page
        };
        const data = await this.$v2.post('/houses', query);
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
        this.$emit('cancel', false);
      },
      labels(label) {
        let html = '';
        if (label) {
          let words = label.split('|');
          if (words.length > 1) {
            html += `${words[0]}<br>`;
            words.shift();
          }

          html += `${words.join(',')}`;
        }
        return html
      }
    },
    created() {

    },
    mounted() {
      this.init();
    }
  }
</script>