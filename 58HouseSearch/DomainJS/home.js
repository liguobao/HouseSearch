require.config({
    baseUrl: '/DomainJS/lib/',
    paths: {
        jquery: "jquery-1.11.3.min",
        "AMUI": "amazeui.2.7.1.min",
        "jquery.range": "jquery.range",
        "es5": "es5",
        "mapController": "mapController",
        "addToolbar": "addToolbar",
    },
    shim: {
        "addToolbar": {
            deps: ["jquery"]
        },
        "jquery.range": {
            deps: ["jquery"]
        }
    }
});

"use strict";

require(['domready!', 'jquery', 'AMUI', 'mapController', 'city', 'commuteGo'], function (doc, $, AMUI, mapController, city, commuteGo) {
    city.initAllCityInfo();
    mapController.init();

    $("input[name='locationType']").bind('click', mapController.locationMethodOnChange)

    $("input[name='vehicle']").bind('click', commuteGo.go)

    $('#Get58Data').bind('click', function(e) {
        e.preventDefault();
     
        mapController.Get58DataClick();
        e.stopPropagation();
    });

    $.getJSON("pv.json", function(data) {
        $("#lblPVCount").text(data.PVCount);
    });

    $('#search-offcanvas').offCanvas({ effect: 'overlay' });

    $(".amap-sug-result").css("z-index", 9999);
})
