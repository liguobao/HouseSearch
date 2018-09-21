<template>
  <div id="app">

    <div id="container"></div>
    <div id="panel"></div>
  </div>
</template>

<style lang="scss">
  #app {
    height: 100vh;
    width: 100%;
  }

  #container {
    height: 100%;
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
    top: 10px;
    right: 10px;
    width: 250px;
    border: solid 1px silver;
  }
</style>
<script>
  import Vue from 'vue'

  export default {
    data() {
      return {
        zoom: 12,
        info: undefined,
        markers: [],
        loading: true,
        cityName: '广州',
        myPosition: undefined,
        transferFn: undefined
      }
    },
    methods: {
      async getList() {
    return await this.$ajax.post('/houses', {
      cityName: this.cityName
    });
  },
  transfer(position,map) {
    if(this.transferFn) {
      this.transferFn.clear();
    }
    this.transferFn = new AMap.Transfer({
      city: this.cityName, // 必须值，搭乘公交所在城市
      map: map, // 可选值，搜索结果的标注、线路等均会自动添加到此地图上
      panel: "panel", // 可选值，显示搜索列表的容器,
      extensions: "all", // 可选值，详细信息
      poliy: AMap.TransferPolicy.LEAST_DISTANCE // 驾驶策略：最省时模式
    });


    this.transferFn.search([this.myPosition.lng, this.myPosition.lat], [position.lng, position.lat],function (status,result) {

    });

  },
  async init() {
    let map = new AMap.Map('container', {
      zoom: this.zoom,
      resizeEnable: true,
    });
    map.clearMap();
    let self = this;

    AMap.plugin(['AMap.ToolBar', 'AMap.Driving', 'AMap.LineSearch', `AMap.StationSearch`, 'AMap.Transfer', 'AMap.Walking', 'AMap.Riding', 'AMap.Geolocation'], function () {//异步同时加载多个插件
      var toolbar = new AMap.ToolBar();
      map.addControl(toolbar);

      var options = {
        'showMarker': false,//是否显示定位按钮
      };

      var geolocation = new AMap.Geolocation(options);
      map.addControl(geolocation);
      geolocation.getCurrentPosition((status, result) => {
        if (status === 'complete') {
        self.myPosition = result.position;
        var marker = new AMap.Marker({ //添加自定义点标记
          map: map,
          zIndex: 100000,
          position: [result.position.lng, result.position.lat], //基点位置
          offset: new AMap.Pixel(-17, -42), //相对于基点的偏移位置
          draggable: true,  //是否可拖动
          content: '<div class="marker-route marker-marker-bus-from"></div>'   //自定义点标记覆盖物内容
        });
        marker.on('dragend', function (ev) {
          self.myPosition = ev.lnglat;
        })
      } else {

      }
    })
    });


    let code = new AMap.Geocoder({
      city: this.cityName,
      radius: 1000
    });

    let info = await this.getList();
    let data = info.data;
    data.length = 20;



    let infoWindow = new AMap.InfoWindow({
      offset: new AMap.Pixel(0, -30),
      content: ``  //传入 dom 对象，或者 html 字符串
    });
    let list = await Promise.all(data.map(function (item) {
      return new Promise((resolve, reject) => {
        code.getLocation(item.houseLocation, (status, result) => {
        if (status === "complete" && result.info === 'OK') {

        let icon = 'https://webapi.amap.com/theme/v1.3/markers/n/mark_b.png';
        if (item.locationMarkBG) {
          icon = require('./image/' + (item.locationMarkBG));
        }

        var marker = new AMap.Marker({
          map: map,
          title: item.houseLocation,
          icon,
          position: [result.geocodes[0].location.lng, result.geocodes[0].location.lat]
        });

        map.add(marker);

        let displayMoney = item.DisPlayPrice ? "  租金：" + item.DisPlayPrice : "";
        let sourceContent = item.displaySource ? " 来源：" + item.displaySource : "";
        let title = item.houseTitle;
        if (title) {
          title = item.houseLocation;
        }

        marker.on('click', function (e) {

          const makerInfoComponent = Vue.extend({
            render(h) {
              return h('div', {
                class: ['marker-info']
              }, [
                h('a', {
                  attrs: {
                    target: '_blank',
                    href: item.houseOnlineURL
                  },
                  class: ['marker-link'],
                  domProps: {
                    innerHTML: `房源: ${title}`
                  }
                }),
                h('a', {
                  attrs: {
                    target: '_blank',
                    href: item.houseOnlineURL
                  },
                  class: ['marker-link'],
                  domProps: {
                    innerHTML: `${displayMoney}`
                  }
                }),
                h('a', {
                  attrs: {
                    target: '_blank',
                    href: item.houseOnlineURL
                  },
                  class: ['marker-link'],
                  domProps: {
                    innerHTML: `${sourceContent}`
                  }
                }),
                h('span', {
                  domProps: {
                    innerHTML: `开始导航`
                  },
                  class: ['marker-info', 'marker-link'],
                  on: {
                    click: function (e) {
                      self.transfer(result.geocodes[0].location,map)
                    }
                  }
                })
              ])
            }
          });

          const component = new makerInfoComponent().$mount();

          infoWindow.setContent(component.$el);
          infoWindow.open(map, e.target.getPosition());
        });

        resolve(marker)
      } else {
        resolve(item);
        //reject(new Error('找不到坐标'))
      }
    })
    })
    }));


  },
  go() {
    console.log(4)
  }
  },
  async created() {

  },
  mounted() {
    window.onload = async () => {
      this.loading = false;
      this.init();
    };

    this.loading = true;
    let key = `8a971a2f88a0ec7458d43b8bc03b6462`;
    let plugin = `AMap.ArrivalRange,AMap.Scale,AMap.Geocoder,AMap.Transfer,AMap.Autocomplete,AMap.CitySearch,AMap.Walking`.split();
    plugin.push(`AMap.ToolBar`)
    let url = `https://webapi.amap.com/maps?v=1.4.8&key=${key}&plugin=${plugin.join()}`;
    let jsapi = document.createElement('script');
    jsapi.charset = 'utf-8';
    jsapi.src = url;
    document.head.appendChild(jsapi);
  }
  }
</script>
