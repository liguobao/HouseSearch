<template>
  <div class="map" v-loading="loading">
    <el-amap vid="amap" :zoom="zoom">
      <el-amap-marker vid="marker"
                      v-for="item in markers"
                      :label="item.label"
                      :title="item.title"
                      :visible="item.visible"
                      :position="item.position"
      >
      </el-amap-marker>
    </el-amap>
  </div>
</template>
<style lang="scss" scoped>
  .map {
    height: 100vh;
  }
</style>
<script>
  import Vue from 'vue';
  import amap from 'vue-amap';

  Vue.use(amap);

  amap.initAMapApiLoader({
    // 高德key
    key: '8a971a2f88a0ec7458d43b8bc03b6462',
    // 插件集合 （插件按需引入）
    plugin: ["AMap.ArrivalRange", "AMap.Scale", "AMap.Geocoder", "AMap.Transfer", "AMap.Autocomplete", "AMap.CitySearch", "AMap.Walking"],
  });

  export default {
    name: 'Map',
    data() {
      return {
        zoom: 13,
        label: {
          content: '高教花园'
        },
        loading: true,
        geocoder: null,
        markers: []
      }
    },
    computed: {},
    methods: {
      async init(cityName, query) {
        let geocoder = await new AMap.Geocoder({
          city: cityName,
          radius: 1000
        });
        this.geocoder = geocoder;
        this.getHouseList(query);
      },
      async getHouseList(options) {
        let params = Object.assign({}, options, {
          cityName: options.cityname
        });
        delete params.cityname;
        let data = await this.$ajax.post(`/houses`, params);
        let list = data.data;

        let activeCode = undefined;
        list.forEach(item => {
          // console.log(item.houseTitle)
          // console.log(item.houseOnlineURL)
          // console.log(item.disPlayPrice)
          this.geocoder.getLocation('' + item.houseTitle, (status, result) => {
            if (status === 'complete' && result.info === 'OK') {
              activeCode = result.geocodes[0];
              this.markers.push(result.geocodes[0])
              this.markers.push({
                visible: true,
                position: [activeCode.location.getLng(), activeCode.location.getLat()],
                label: {
                  content: ``
                }
              })
            }
          })
        })

        // list.forEach(item => {
        //
        // })
      }
    },
    created() {

    },
    async mounted() {
      this.loading = true;
      window.onload = () => {
        const query = this.$route.query;
        this.init(query.cityname, query);
        this.loading = false;
      }
    }
  }
</script>