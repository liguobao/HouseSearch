"use strict";

// 地图控制器相关，封装了地图对象
var mapController = define(['jquery', 'AMUI', 'mapSignleton', 'marker',
        'polygon', 'workLocation', 'city', 'commuteGo', "helper"
    ],
    function ($, AMUI, mapSignleton, marker, polygon, workLocation, city, commuteGo, helper) {

        var _map = mapSignleton.map;
        var _amapTransfer = null;
        var _infoWindow = mapSignleton.infoWindow;
        var _workAddress = mapSignleton.workAddress;

        var addToolbar = function () {
            _map.plugin(["AMap.ToolBar"], function () {
                _map.addControl(new AMap.ToolBar());
            });
            if (location.href.indexOf('&guide=1') !== -1) {
                _map.setStatus({
                    scrollWheel: false
                })
            }
        }

        var GetDataByIndex = function (index, count, dataResource, page) {

            var dataInfo = [];
            var contentType = "application/x-www-form-urlencoded; charset=UTF-8";
            if (dataResource == "douban") {
                dataInfo = {
                    groupID: helper.getQueryString("groupID"),
                    index: index
                };
            } else if (dataResource == "houselist") {
                var source = helper.getQueryString("source") ? helper.getQueryString("source") : "";
                //默认出7天内的数据
                var intervalDay = helper.getQueryString("intervalDay") ? helper.getQueryString("intervalDay") : 14;
                var refresh = helper.getQueryString("refresh") ? helper.getQueryString("refresh") : false;
                var keyword = helper.getQueryString("keyword") ? helper.getQueryString("keyword") : "";
                var fromPrice = helper.getQueryString("fromPrice") ? helper.getQueryString("fromPrice") : 0;
                var toPrice = helper.getQueryString("toPrice") ? helper.getQueryString("toPrice") : 0;
                var searchCondition = {
                    cityName: helper.getQueryString("cityname"),
                    source: source,
                    houseCount: count,
                    intervalDay: intervalDay,
                    keyword: keyword,
                    refresh: refresh,
                    page: page,
                    toPrice: toPrice,
                    fromPrice: fromPrice,
                };
                dataInfo = JSON.stringify(searchCondition);
                contentType = "application/json;";
            } else if (dataResource === 'userCollection') {
                var source = helper.getQueryString("source") ? helper.getQueryString("source") : "";
                dataInfo = {
                    cityName: helper.getQueryString("cityname"),
                    source: source
                };
                index = count;
            } else {
                dataInfo = {
                    cnName: city.shortName,
                    index: index
                };
            }

            $.ajax({
                type: "post",
                url: getViewDefaultDataAction,
                data: dataInfo,
                contentType: contentType,
                success: function (result) {
                    if (result.isSuccess) {
                        var rent_locations = new Set();
                        result.houseInfos.forEach(function (item, index) {
                            rent_locations.add(item);
                        });
                        rent_locations.forEach(function (element, index) {
                            marker.add(element);
                        });
                    } else {
                        console.log(result.error);
                    }
                    if (index == count) {
                        $.AMUI.progress.done();
                    }
                    
                }
            });
        }



        var init = function () {

            // from addToolbar.js of amap.com
            addToolbar();

            _map.addControl(new AMap.Scale());

            // 箭头函数，保证在 event 中 this 环境正确
            // require r.js uglify 方法暂时不支持箭头函数语法，使用 bind 实现
            AMap.event.addListener(new AMap.Autocomplete({
                input: "work-location"
            }), "select", function (e) {
                workLocation.onSelected(e);
            }.bind(workLocation));

            AMap.event.addListener(new AMap.Autocomplete({
                input: "mobile-work-location"
            }), "select", function (e) {
                workLocation.onSelected(e);
            }.bind(workLocation));


            if (dataResource == "douban" || dataResource == "houselist") {
                _map.setCity(helper.getQueryString("cityname"));
            }

            showCityInfo(getHouses)

            initUserWorkAddress($);
        }

        var initUserWorkAddress = function () {
            if (getUserInfo) {
                $.ajax({
                    type: "post",
                    url: getUserInfo,
                    data: {},
                    success: function (result) {
                        if (result && result.success && result.data && result.data.workAddress) {
                            console.log(result.data);
                            mapSignleton.workAddress = result.data.workAddress;
                            $("#work-location").val(result.data.workAddress);
                            workLocation.load();
                        } else {
                            console.log(result);
                            workLocation.initPositionPicker();
                        }
                    }
                });
            }else{
                workLocation.initPositionPicker();
            }
        }

        var showCityInfo = function (ajaxGetter) {
            //实例化城市查询类
            var citysearch = new AMap.CitySearch();
            //自动获取用户IP，返回当前城市
            citysearch.getLocalCity(function (status, result) {
                if (status === 'complete' && result.info === 'OK') {
                    if (result && result.city && result.bounds) {
                        var cityinfo = result.city;
                        var citybounds = result.bounds;
                        city.name = cityinfo.substring(0, cityinfo.length - 1);
                        city.shortCut(city.name);
                        document.getElementById('IPLocation').innerHTML = '您当前所在城市：' + city.name;
                        //地图显示当前城市
                        $("#IPLocationCity").text("城市：" + city.name);
                        _map.setBounds(citybounds);
                    }
                } else {
                    document.getElementById('IPLocation').innerHTML = result.info;
                }
                //只要返回结果就可以下一步了
                ajaxGetter();
            });
        }


        // 点击获取房源信息
        var Get58DataClick = function () {

            $("#Get58Data").attr("disabled", true);
            $.AMUI.progress.start();

            if ($("input[name='locationType']:checked").val() == '0') {
                city.shortCut();
            }

            var costFrom = $("#costFrom").val();
            var costTo = $("#costTo").val();

            if (costFrom == "" || costTo == "") {
                alert("请输入价格区间...");
                $("#Get58Data").attr("disabled", false);
                $.AMUI.progress.done();
                return;
            }

            if (isNaN(costFrom) || isNaN(costTo)) {
                alert("请输入正确的整数......");
                $("#Get58Data").attr("disabled", false);
                $.AMUI.progress.done();
                return;
            }

            marker.clearArray();



            $.ajax({
                type: "post",
                url: getDataPageCount,
                data: {
                    costFrom: costFrom,
                    costTo: costTo,
                    cnName: city.shortName
                },
                success: function (result) {
                    if (result && result.isSuccess) {
                        var pageCount = result.pageCount;
                        console.log("数据总页数为：" + pageCount);

                        for (var index = 0; index < pageCount; index++) {
                            $.ajax({
                                type: "post",
                                url: getDataByPageIndexAction,
                                data: {
                                    costFrom: costFrom,
                                    costTo: costTo,
                                    cnName: city.shortName,
                                    index: index
                                },
                                success: function (result) {
                                    if (result.isSuccess) {
                                        var rent_locations = new Set();
                                        result.houseInfos.forEach(function (item, index) {
                                            rent_locations.add(item);
                                        });
                                        rent_locations.forEach(function (element, index) {
                                            marker.add(element);
                                        });
                                        //console.log("第" + result.PageIndex + "页加载完成。");
                                    } else {
                                        console.log(result.error);
                                    }

                                    if (result.pageIndex == pageCount - 1) {
                                        $.AMUI.progress.done();
                                        $("#Get58Data").attr("disabled", false);
                                    }

                                }
                            });
                        }

                    } else {
                        alert(result.Error);
                    }
                }
            });





        };



        var move2Location = function () {
            _map.on('moveend', getCity);

            function getCity() {
                _map.getCity(function (data) {
                    if (data['province'] && typeof data['province'] === 'string') {

                        var cityinfo = (data['city'] || data['province']);
                        city.name = cityinfo.substring(0, cityinfo.length - 1);
                        city.shortCut();
                        $("#IPLocationCity").text("城市：" + city.name);
                        document.getElementById('IPLocation').innerHTML = '地图中心所在城市：' + city.name;
                    }
                });
            }
        }

        var locationMethodOnChange = function () {
            if ($("input[name='locationType']:checked").val() == '1') {
                showCityInfo();
            } else {
                move2Location();
            }
        }

       

        var getHouses = function (page) {
            $.AMUI.progress.start();
            if (dataResource == "houselist" || dataResource == "userCollection") {
                var houseCount = 600;
                //为了避免数据太多在手机上无法查看，移动平台只出70条数据
                if (helper.isMobile()) {
                    houseCount = 180;
                } else {
                    houseCount = helper.getQueryString("houseCount") ? helper.getQueryString("houseCount") : 600;
                }
                marker.clearArray();
                GetDataByIndex(houseCount, houseCount, dataResource, page);
            } else {
                var pageCount = helper.getQueryString("PageCount");
                if (!pageCount) {
                    pageCount = dataResource == "douban" ? 5 : 15;
                }
                for (var i = 1; i <= pageCount; i++) {
                    GetDataByIndex(i, pageCount, dataResource);
                }
            }

        }

        return {
            init: init,
            Get58DataClick: Get58DataClick,
            showCityInfo: showCityInfo,
            marker: marker,
            locationMethodOnChange: locationMethodOnChange,
            workLocation: workLocation,
            getHouses: getHouses
        }
    });
