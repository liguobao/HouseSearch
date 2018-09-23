<template>
    <div class="map">
        <div class="container" id="map-container">
            <span class="highlight-text">特此声明:房源信息来自网络，本网站不对其真实性负责。首次载入无数据可尝试【F5】强制刷新.问题反馈：codelover@qq.com</span>
            <div class="icon-tips">
                <ul>
                    <li class="btn">
                        <el-button type="success" size="medium" @click="toggleRight">上班导航</el-button>
                    </li>
                    <li v-for="item in iconTips">
                        <img :src="item.src"/>
                        <span>{{item.text}}</span>
                    </li>
                </ul>
            </div>
            <div id="panel"></div>
        </div>
        <div class="right" :class="{'show-right' : showRight}">
            <section class="link">
                <router-link class="to" :to="item.link" v-for="item in rightLinks">{{item.text}}</router-link>
            </section>
            <section>
                <p class="text">您当前所在的城市: {{activeCityName}}</p>
            </section>
            <section>
                <p class="text sub-title">
                    选择工作地点：
                </p>
                <div class="el-input el-input--medium el-input--suffix" :class="{clean: keywordClean}">
                    <input type="text" v-model="keyword" autocomplete="off" placeholder="请输入内容" id="keyword"
                           class="el-input__inner keyword">
                    <span class="el-input__suffix keyword-clean">
                        <span class="el-input__suffix-inner">
                            <i class="el-input__icon el-icon-circle-close" @click="keyword = undefined"></i>
                        </span>
                    </span>
                </div>
            </section>
            <section>
                <p class="text sub-title">
                    选择通勤方式：
                </p>
                <el-radio-group v-model="commuting" class="commuting">
                    <el-radio :label="3" class="text">公交+地铁</el-radio>
                    <el-radio :label="6" class="text">地铁</el-radio>
                    <el-radio :label="9" class="text">步行</el-radio>
                </el-radio-group>
            </section>
            <section>
                <p class="text sub-title">
                    参考代码：
                </p>
                <a href="https://github.com/liguobao/58HouseSearch" target="_blank" class="to">Github源码（无耻求收藏）</a>
                <a href="" target="_blank" class="to">互助租房-微博</a>
                <a href="" target="_blank" class="to">高德API+Python解决租房问题</a>
            </section>
            <section>
                <p class="text sub-title">
                    问题反馈：
                </p>
                <a href="https://github.com/liguobao/58HouseSearch/issues" target="_blank" class="to">改进建议: Github Issues</a>
                <span class="to">联系邮箱: codelover@qq.com</span>
            </section>
        </div>
    </div>
</template>
<style scoped lang="scss">
    .highlight-text {
        position: absolute;
        color: red;
        left: 5px;
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
            .nav{
                display: block;
                color: #ccc;
            }
            .sub-title{
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
        .commuting{
            display: flex;
            justify-content: flex-start;
            align-items: center;
            .text{
                margin-bottom: 0;
            }
        }
    }
</style>
<style lang="scss">
    .map {
        height: 100vh;
        width: 100%;
        display: flex;
        flex-direction: row;
        .container {
            height: 100%;
            flex: auto;
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
            top: 6%;
            left: 5%;
            width: 250px;
            border: solid 1px silver;
        }
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
                commuting: 1
            }
        },
        computed: {
            keywordClean() {
                return !!this.keyword
            }
        },
        watch: {
          '$route.query':function (params) {
              this.init()
          }
        },
        methods: {
            toggleRight() {
                this.showRight = !this.showRight
            },
            async getList() {
                return await this.$ajax.post('/houses', {
                    cityName: this.cityName
                });
            },
            transfer(position, map) {
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
            addMaker(map, data, code, self) {
                return new Promise(async (resolve, reject) => {
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
                                        icon = require('./../images/' + (item.locationMarkBG));
                                    }

                                    let marker = new AMap.Marker({
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
                                                                self.transfer(result.geocodes[0].location, map)
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
                })
            },
            keywordSelect(map, positionPicker) {
                let auto = new AMap.Autocomplete({
                    input: "keyword"
                });
                let placeSearch = new AMap.PlaceSearch({
                    map: map
                });

                AMap.event.addListener(auto, "select", select);//注册监听，当选中某条记录时会触发
                function select(e) {
                    positionPicker.start(e.poi.location);
                    // placeSearch.setCity(e.poi.adcode);
                    // placeSearch.search(e.poi.name);  //关键字查询查询
                }
            },
            getCityCenter(positionPicker,code) {
                return new Promise((resolve, reject) => {
                    code.getLocation(this.cityName, (status, result) => { // 城市中心点
                        if (status === "complete" && result.info === 'OK') {
                            positionPicker.start(result.geocodes[0].location)
                        } else {
                            positionPicker.start(map.getBounds().getSouthWest())
                        }
                        resolve()
                    });
                })
            },
            async init() {
                this.loading = false;

                const query = this.$route.query;
                let cityName = query.cityname;
                if (!cityName) {
                    cityName = await this.getActiveCityName(this);
                }
                this.activeCityName = this.cityName = cityName;

                let map = new AMap.Map('map-container', {
                    zoom: this.zoom,
                    resizeEnable: true,
                });
                map.clearMap();
                map.setCity(this.cityName);
                let self = this;

                await this.installPlugin(map, self);
                let positionPicker = await this.installAmapUI(map, self);

                let code = new AMap.Geocoder({
                    city: this.cityName,
                    radius: 1000
                });

                await this.getCityCenter(positionPicker,code);
                this.keywordSelect(map, positionPicker);

                let info = await this.getList();
                let data = info.data;
                // data.length = 20;
                // this.showRight = true;
                await this.addMaker(map, data, code, self);

            },

            appendScript(url, cb) {
                return new Promise((resolve, reject) => {
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
        async mounted() {
            this.loading = true;
            let key = `8a971a2f88a0ec7458d43b8bc03b6462`;
            let plugin = `AMap.ArrivalRange,AMap.Scale,AMap.Geocoder,AMap.Transfer,AMap.Autocomplete,AMap.CitySearch,AMap.Walking`.split();
            plugin.push(`AMap.ToolBar`);
            let url = `https://webapi.amap.com/maps?v=1.4.8&key=${key}&plugin=${plugin.join()}`;

            await this.appendScript(url);
            await this.appendScript(`//webapi.amap.com/ui/1.0/main.js?v=1.0.11`);


            this.init()
        }
    }
</script>
