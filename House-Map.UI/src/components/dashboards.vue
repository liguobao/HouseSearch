<template>
  <div class="dashboards"
       :class="{'is-mobile':isMobile}"
  >
    <template v-if="!loading">
      <div class="search" v-if="type === 'all'">
        <el-input
            placeholder="城市名"
            prefix-icon="el-icon-search"
            @input="search"
            v-model="key">
        </el-input>
      </div>
      <ul v-if="cityList && cityList.length">
        <li v-for="item in cityList" :key="item.id" :class="{'is-mobile': isMobile}">
          <a target="_blank"
             @click="to({city:item.city})"
             href="javascript:;"
             :title="item.city" class="title highlight">{{item.city}}</a>
          <div class="source-wrap">
            <a target="_blank"
               :key="source.id"
               href="javascript:;"
               @click="to({city:item.city,source:source.source,intervalDay:14,houseCount:600})"
               class="highlight" v-for="source in item.sources" :title="source.displaySource">
              {{source.displaySource}}
              <template v-if="source.houseCount > 0">
                ({{source.houseCount}})
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
  .is-mobile.dashboards {
    .title {
      font-size: 16px;
    }
    .source-wrap {
      font-size: 12px;
    }
    li {
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

  .search{
    width: 60%;
    margin: 15px auto;
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
      token: {},
      navTo: {},
      filterCity: {
        default: ''
      }
    },
    computed: {
      dataType() {
        return this.type
      },
      cityList() {
        let cities = this.cities;
        if (this.filterCity) {
          cities = cities.filter(item => {
            return this.filterCity.indexOf(item.city) !== -1
          })
        }
        if(this.key) {
          cities = cities.filter(item => {
            return this.key.indexOf(item.city) !== -1
          })
        }
        return cities
      }
    },
    data() {
      return {
        cities: undefined,
        loading: false,
        key:undefined
      }
    },
    methods: {
      async getData() {
        this.loading = true;
        if (this.dataType === 'all') {
          const res = await this.$v2.get('/cities');
          let data = res.data;
          this.cities = data;
        } else {
          const userId = this.$store.state.userInfo.id;
          const res = await this.$v2.get(`/users/${userId}/collections/city-source`);
          const data = res.data;
          this.cities = data;
        }
        this.loading = false;
      },
      search(key) {

      },
      to(parmas) {
        let query = parmas;
        if(this.dataType === 'user') {
          query.collectionType = true
        }
        this.navTo(query)
      }
    },
    created() {
      this.getData()
    },
  }
</script>