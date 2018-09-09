<template>
  <div class="dashboards"
       :class="{'is-mobile':isMobile}"
  >
    <template v-if="!loading">
      <ul v-if="cities && cities.length">
        <li v-for="item in cities" :key="item.id" :class="{'is-mobile': isMobile}">
          <a target="_blank" :href="`https://www.woyaozufang.live/Home/HouseList?cityname=${item.cityName}&token=${token}`"
             :title="item.cityName" class="title highlight">{{item.cityName}}</a>
          <div class="source-wrap">
            <a target="_blank"
               :key="source.id"
               :href="`https://www.woyaozufang.live/Home/HouseList?cityname=${item.cityName}&source=${source.source}&intervalDay=14&houseCount=600&token=${token}`"
               class="highlight" v-for="source in item.sources" :title="source.displaySource">
              {{source.displaySource}}
              <template v-if="source.houseSum < 9999">
                ({{source.houseSum}})
              </template>
              <template v-else>
                (9999+)
              </template>
            </a>
          </div>
        </li>
      </ul>
      <div v-else class="is-empty text-center">暂无数据</div>
    </template>
    <template v-else>
      <div class="text-center">
        <i class="el-icon-loading"></i>
      </div>
    </template>
  </div>
</template>
<style scoped lang="scss">
  .is-mobile.dashboards{
    .title{
      font-size: 16px;
    }
    .source-wrap{
      font-size: 12px;
    }
    li{
      width: 50%;
    }
  }
</style>
<style lang="scss" scoped>
  @keyframes toUp {
    0% {
      opacity: 0;
      transform: translateY(50%);
    }
    100% {
      opacity: 1;
      transform: translateY(0);
    }
  }

  .el-icon-loading {
    color: #409EFF;
  }

  .dashboards {
    max-height: 720px;
    overflow: auto;
    -webkit-overflow-scrolling: touch;
  }

  ul {
    display: flex;
    flex-wrap: wrap;
  }

  li {
    width: 150px;
    text-align: center;
    margin-bottom: 20px;

  }

  @for $i from 1 to 600 {
    li:not(.is-mobile):nth-of-type(#{$i}) {
      animation: toUp 0.5s (0.05s*$i) ease-out both;
    }
  }

  .highlight {
    color: #409EFF;
    transition: all 0.3s;
    &:hover {
      color: #095f8a;
    }
  }

  .title {
    font-size: 20px;
    font-weight: 600;
    display: block;
    margin-bottom: 10px;
  }

  .source-wrap {
    a {
      display: block;
      line-height: 1.8;
    }
  }
</style>
<script>
  export default {
    props: {
      type: {
        default: 'all'
      },
      isMobile: {},
      token: {}
    },
    computed: {
      dataType() {
        return this.type
      }
    },
    data() {
      return {
        cities: undefined,
        loading: false
      }
    },
    methods: {
      async getData() {
        this.loading = true;
        if (this.dataType === 'all') {
          const res = await this.$ajax.get('/houses/dashboard');
          const data = res.data;
          this.cities = data;
        } else {
          const userId = this.$store.state.userInfo.id;
          const res = await this.$ajax.get(`/users/${userId}/collections/dashboard`);
          const data = res.data;
          this.cities = data;
        }
        this.loading = false;
      }
    },
    created() {
      this.getData()
    }
  }
</script>