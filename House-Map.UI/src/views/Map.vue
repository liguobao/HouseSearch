<template>
    <div class="map">
        <div class="container" id="map-container">
        </div>
        <template v-if="!isMobile">
            <div class="card">
                <h4>出行到达圈查询</h4>
                <div class="card-item">
                    <span class="card-name">上班地点</span>
                    <el-input
                            id="keyword"
                            class="card-value"
                            size="mini"
                            type="text"
                            placeholder="请输入内容"
                            v-model="keyword"
                            clearable>
                    </el-input>
                </div>
                <div class="card-item">
                    <span class="card-name">时长(分钟)</span>
                    <el-slider class="card-value" :min="1" :max="60" v-model="times" size="mini"></el-slider>
                    <span class="card-times">{{times}}</span>
                </div>
                <div class="card-item">
                    <span class="card-name">出行方式</span>
                    <el-select v-model="waysMethod" placeholder="请选择出行方式" class="card-value" size="mini">
                        <el-option
                                label="地铁+公交"
                                value="">
                        </el-option>
                        <el-option
                                label="地铁"
                                value="SUBWAY">
                        </el-option>
                        <el-option
                                label="公交"
                                value="BUS">
                        </el-option>
                    </el-select>
                </div>
                <div class="card-item btn">
                    <el-button type="primary" size="mini" @click="search" :loading="searching" >查询</el-button>
                    <el-button type="info" size="mini" :loading="searching" @click="next">下一页</el-button>
                    <!--<el-button size="mini">清空</el-button>-->
                </div>
            </div>
            <span class="highlight-text">特此声明:房源信息来自网络，本网站不对其真实性负责。首次载入无数据可尝试【F5】强制刷新.问题反馈：codelover@qq.com</span>
            <div class="icon-tips">
                <ul>
                    <!--<li class="btn">-->
                    <!--<el-button type="success" size="medium" @click="toggleRight">上班导航</el-button>-->
                    <!--</li>-->
                    <li v-for="item in iconTips">
                        <img :src="item.src"/>
                        <span>{{item.text}}</span>
                    </li>
                </ul>
            </div>
        </template>
        <template v-else>
            <div class="filter">
                <div class="filter-item">
                    <span>上班地点: </span>
                    <el-input
                            id="keyword"
                            class="card-value"
                            size="mini"
                            placeholder="请输入内容"
                            v-model="keyword"
                            clearable>
                    </el-input>
                </div>
                <div class="filter-item">
                    <el-button type="primary" size="mini" :loading="searching" @click="next">下一页</el-button>
                </div>
            </div>
            <div class="mobile-bg" v-if="makerInfo" @click="handleMobileBg">
                <div class="content">
                    <section>
                        <span class="content-name">房源: </span>
                        <a :href="makerInfo.houseOnlineURL" target="_blank"
                           class="content-value">{{makerInfo.title}}</a>
                    </section>
                    <section v-if="makerInfo.displayMoney">
                        <span class="content-value">{{makerInfo.displayMoney}}</span>
                    </section>
                    <section>
                        <span class="content-value">{{makerInfo.sourceContent}}</span>
                    </section>
                    <section>
                        <span class="content-name btn" @click="collect(makerInfo)"><i
                                class="el-icon-star-on"></i>收藏</span>
                    </section>
                    <section>
                        <span class="content-name btn" @click="navTo(makerInfo)"><i
                                class="el-icon-location"></i>开始导航</span>
                    </section>
                </div>
            </div>
        </template>
        <div id="panel" v-show="transferWays" :class="{'slide-up' : toggleUp,'is-mobile' : isMobile}">
            <span class="panel-handle" @click="toggleUp = !toggleUp">{{!toggleUp ? '收起' : '展开'}}</span>
        </div>
        <span @click="whereAmI" class="where-am-i" :class="{'is-mobile' : isMobile}">
            <i class="el-icon-location"></i>
            我在哪
        </span>
        <el-dialog
                title="地图搜租房"
                :width="isMobile ? '100%' : '700px'"
                center
                :visible="loginVisible"
                :before-close="() => {toggleDialog('loginVisible')}"
        >
            <login ref="login" @close="toggleDialog" :done="done" :login-type="loginType"></login>
        </el-dialog>

        <!--<div class="right" :class="{'show-right' : showRight}">-->
        <!--<section class="link">-->
        <!--<router-link class="to" :to="item.link" v-for="item in rightLinks">{{item.text}}</router-link>-->
        <!--</section>-->
        <!--<section>-->
        <!--<p class="text">您当前所在的城市: {{activeCityName}}</p>-->
        <!--</section>-->
        <!--<section>-->
        <!--<p class="text sub-title">-->
        <!--选择工作地点：-->
        <!--</p>-->
        <!--<div class="el-input el-input&#45;&#45;medium el-input&#45;&#45;suffix" :class="{clean: keywordClean}">-->
        <!--<input type="text" v-model="keyword" autocomplete="off" placeholder="请输入内容" id="keyword"-->
        <!--class="el-input__inner keyword">-->
        <!--<span class="el-input__suffix keyword-clean">-->
        <!--<span class="el-input__suffix-inner">-->
        <!--<i class="el-input__icon el-icon-circle-close" @click="keyword = undefined"></i>-->
        <!--</span>-->
        <!--</span>-->
        <!--</div>-->
        <!--</section>-->
        <!--<section>-->
        <!--<p class="text sub-title">-->
        <!--选择通勤方式：-->
        <!--</p>-->
        <!--<el-radio-group v-model="commuting" class="commuting">-->
        <!--<el-radio :label="3" class="text">公交+地铁</el-radio>-->
        <!--<el-radio :label="6" class="text">地铁</el-radio>-->
        <!--<el-radio :label="9" class="text">步行</el-radio>-->
        <!--</el-radio-group>-->
        <!--</section>-->
        <!--<section>-->
        <!--<p class="text sub-title">-->
        <!--参考代码：-->
        <!--</p>-->
        <!--<a href="https://github.com/liguobao/58HouseSearch" target="_blank" class="to">Github源码（无耻求收藏）</a>-->
        <!--<a href="" target="_blank" class="to">互助租房-微博</a>-->
        <!--<a href="" target="_blank" class="to">高德API+Python解决租房问题</a>-->
        <!--</section>-->
        <!--<section>-->
        <!--<p class="text sub-title">-->
        <!--问题反馈：-->
        <!--</p>-->
        <!--<a href="https://github.com/liguobao/58HouseSearch/issues" target="_blank" class="to">改进建议: Github Issues</a>-->
        <!--<span class="to">联系邮箱: codelover@qq.com</span>-->
        <!--</section>-->
        <!--</div>-->
    </div>
</template>
<style scoped lang="scss">
    .highlight-text {
        position: fixed;
        color: red;
        left: 5px;
        top: 2px;
        font-weight: bold;
        z-index: 40;
    }

    .icon-tips {
        position: fixed;
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
            .nav {
                display: block;
                color: #ccc;
            }
            .sub-title {
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
        .commuting {
            display: flex;
            justify-content: flex-start;
            align-items: center;
            .text {
                margin-bottom: 0;
            }
        }
    }

    .card {
        display: flex;
        flex-direction: column;
        word-break: break-word;
        background: #fff;
        box-shadow: 0 2px 6px 0 rgba(114, 124, 245, .5);
        border-radius: 5px;
        position: fixed;
        left: 80px;
        top: 40px;
        z-index: 30;
        padding: 12px;
        h4 {
            color: grey;
            font-size: 13px;
            font-weight: normal;
        }
        .card-item {
            display: flex;
            font-size: 12px;
            align-items: center;
            &.btn {
                margin-top: 10px;
                text-align: center;
                justify-content: center;
            }
            .card-times {
                margin-left: 10px;
            }
            .card-value {
                flex: auto;
            }
            .card-name {
                min-width: 70px;
            }
        }
    }

    .filter{
        position: fixed;
        z-index: 40;
        left: 0;
        top: 0;
        width: 100%;
        background: rgba(0,0,0,0.7);
        padding: 10px;
        display: flex;
        align-items: center;
        .filter-item{
            display: flex;
            flex-direction: row;
            font-size: 12px;
            align-items: center;
            &:nth-of-type(2n){
                margin-left: 5px;
            }
            span{
                color: #fff;
                width: 70px;
            }
        }
    }
    .mobile-bg {
        position: fixed;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.7);
        z-index: 1000;
        left: 0;
        top: 0;
        .content {
            padding: 10px;
            height: 100%;
            max-height: 100%;
            overflow: auto;
            -webkit-overflow-scrolling: touch;
            width: 45%;
            position: absolute;
            right: 0;
            top: 0;
            background: #fff;
            border-top-left-radius: 3px;
            border-bottom-left-radius: 3px;
            .content-name {
                &.btn {
                    display: block;
                }
            }
            section {
                padding: 15px 0;
                border-bottom: 1px solid #eee;
                font-size: 14px;
            }
        }
    }
    .where-am-i{
        position: fixed;
        z-index: 60;
        right: 20px;
        bottom: 20px;
        text-align: center;
        font-size: 12px;
        background: #fff;
        border-radius: 4px;
        padding: 10px;
        cursor: pointer;
        &.is-mobile{
            right: 18%;
        }
        i{
            display: block;
        }
    }
</style>
<style lang="scss">
    .map {
        height: 100vh;
        width: 100%;
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
            top: 254px;
            left: 5%;
            width: 250px;
            border: solid 1px silver;
            -webkit-overflow-scrolling: touch;
            transition: all 0.3s;
            &.is-mobile {
                top: 10%;
            }
            .panel-handle {
                display: block;
                padding: 0 10px;
                cursor: pointer;
            }
            &.slide-up {
                max-height: 25px;
            }
            &:empty {
                display: none;
            }
        }
    }
</style>
<script>
    import Vue from 'vue'
    import Login from './../components/login';

    export default {
        components: {
            Login
        },
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
                commuting: 1,
                transferWays: false,
                toggleUp: false,
                times: 60,
                waysMethod: '',
                map: undefined,
                lnglat: undefined,
                arrivalRange: undefined,
                polygons: [],
                positionPicker: undefined,
                loginVisible: false,
                searching: false,
                loginType: undefined,
                done: null,
                collection: false,
                makerInfo: undefined
            }
        },
        computed: {
            keywordClean() {
                return !!this.keyword
            },
            isMobile() {
                return this.$store.state.isMobile
            },
            user() {
                return !!this.$store.state.userInfo
            }
        },
        watch: {
            '$route.query': function (params) {
                this.loading = true;
                this.init()
            }
        },
        methods: {
            whereAmI() {
                this.positionPicker.start(this.lnglat)
            },
            next() {
                const query = this.$route.query;
                let page = 1;
                if(!query.page) {
                    page = 2;
                }else {
                    page = (+query.page) + 1;
                }
                const params = Object.assign({},query,{page});
                this.$router.push({
                    query: params
                })
            },
            handleMobileBg(e) {
                if (e.target === e.currentTarget) {
                    this.makerInfo = undefined;
                }
            },
            navTo(item) {
                this.transfer(item.geocodes.location, this.map, this);
                this.makerInfo = undefined;
            },
            async collect(item) {
                let self = this;
                if (!self.user) {
                    await new Promise(async (resolve1, reject1) => {
                        self.loginVisible = true;
                        self.done = resolve1;
                    });
                }
                if (this.collection) {
                    return
                }
                this.collection = true;
                const userId = this.$store.state.userInfo.id;
                const data = await this.$ajax.post(`/users/${userId}/collections`, {
                    userId,
                    houseID: item.id,
                    source: item.source
                });
                this.collection = false;
                this.makerInfo = undefined;
                if (this.isMobile) {
                    alert(data.message)
                } else {
                    this.$message.success(data.message);
                }
            },
            toggleDialog(key, val, type) {
                if (key === 'loginVisible') {
                    if (type) {
                        this.loginType = type;
                    } else {
                        this.loginType = undefined;
                    }
                }
                this[key] = val || false;
            },
            search() {
                let self = this;
                self.searching = true;
                self.arrivalRange = new AMap.ArrivalRange();
                self.arrivalRange.search(self.lnglat, self.times, function (status, result) {
                    self.map.remove(self.polygons);
                    self.polygons = [];
                    if (result.bounds) {
                        for (var i = 0; i < result.bounds.length; i++) {
                            var polygon = new AMap.Polygon({
                                fillColor: "#3366FF",
                                fillOpacity: "0.4",
                                strokeColor: "#00FF00",
                                strokeOpacity: "0.5",
                                strokeWeight: 1
                            });
                            polygon.setPath(result.bounds[i]);
                            self.polygons.push(polygon);
                        }
                        self.map.add(self.polygons);
                        self.map.setFitView();
                        self.map.setZoomAndCenter(13, self.lnglat);
                        self.searching = false;
                    } else {
                        self.$message.error('未知错误');
                        self.searching = false;
                    }
                }, {
                    policy: self.waysMethod
                });
            },
            toggleRight() {
                this.showRight = !this.showRight
            },
            async getList() {
                let houseCount = undefined;
                if(this.isMobile) {
                    houseCount = 100;
                }
                return await this.$ajax.post('/houses', {
                    ...this.$route.query,
                    cityName: this.cityName,
                    houseCount
                });
            },
            transfer(position, map, self) {
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
                    self.transferWays = true;
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
                            self.positionPicker = positionPicker;
                            self.lnglat = positionResult.position;
                            self.keyword = positionResult.address
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
                            if (!item) {
                                reject(item)
                            }
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

                                        if (self.isMobile) {
                                            self.makerInfo = {
                                                ...item,
                                                displayMoney,
                                                sourceContent,
                                                title,
                                                geocodes: result.geocodes[0]
                                            }
                                        } else {
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
                                                            class: ['marker-info', 'marker-link'],
                                                            on: {
                                                                click: async function (e) {
                                                                    self.collect(item)
                                                                }
                                                            }
                                                        }, [
                                                            h('i', {
                                                                class: ['el-icon-star-on']
                                                            }),
                                                            '收藏',
                                                        ]),
                                                        h('span', {
                                                            class: ['marker-info', 'marker-link'],
                                                            on: {
                                                                click: function (e) {
                                                                    self.transfer(result.geocodes[0].location, map, self);
                                                                    infoWindow.close();
                                                                }
                                                            }
                                                        }, [
                                                            h('i', {
                                                                class: ['el-icon-location'],
                                                            }),
                                                            '开始导航'
                                                        ])
                                                    ])
                                                }
                                            });

                                            const component = new makerInfoComponent().$mount();

                                            infoWindow.setContent(component.$el);
                                            infoWindow.open(map, e.target.getPosition());
                                        }

                                    });
                                    resolve(marker)
                                } else {
                                    resolve(item);
                                    //reject(new Error('找不到坐标'))
                                }
                            })
                        })
                    }));
                    resolve()
                })
            },
            keywordSelect(map, positionPicker) {
                let auto = new AMap.Autocomplete({
                    input: "keyword"
                });
                let placeSearch = new AMap.PlaceSearch({
                    map: map
                });
                let self = this;

                AMap.event.addListener(auto, "select", select);//注册监听，当选中某条记录时会触发
                function select(e) {
                    positionPicker.start(e.poi.location);
                    self.lnglat = e.poi.location;
                    if(self.isMobile) {
                        self.search()
                    }
                    // placeSearch.setCity(e.poi.adcode);
                    // placeSearch.search(e.poi.name);  //关键字查询查询
                }
            },
            getCityCenter(positionPicker, code, self) {
                let location = undefined;
                return new Promise((resolve, reject) => {
                    code.getLocation(this.cityName, (status, result) => { // 城市中心点
                        if (status === "complete" && result.info === 'OK') {
                            location = result.geocodes[0].location;
                            positionPicker.start(location);
                            self.lnglat = location;
                            // self.keyword = result.geocodes[0].formattedAddress;
                        } else {
                            location = self.map.getBounds().getSouthWest();
                            self.lnglat = location;
                            // self.keyword = self.cityName;
                            positionPicker.start(location)
                        }
                        resolve()
                    });
                })
            },
            async init() {
                const loading = this.$loading({
                    lock: true,
                    text: '正在加载数据,若等待时间过长,请重新刷新页面',
                    spinner: 'el-icon-loading',
                    background: 'rgba(0, 0, 0, 0.7)'
                });
                try {
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
                    this.map = map;
                    map.clearMap();
                    map.setCity(this.cityName);
                    let self = this;

                    await this.installPlugin(map, self);
                    let positionPicker = await this.installAmapUI(map, self);

                    let code = new AMap.Geocoder({
                        city: this.cityName,
                        radius: 1000
                    });

                    await this.getCityCenter(positionPicker, code, self);
                    this.keywordSelect(map, positionPicker);

                    let info = await this.getList();
                    let data = info.data;
                    data.length = 20;
                    // this.showRight = true;


                    await this.addMaker(map, data, code, self);
                    loading.close();
                    this.loading = false;
                } catch (e) {
                    this.$message.error('初始化地图失败,请再次刷新');
                    throw e
                }
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
