require.config({
    baseUrl: '/DomainJS/',
    paths: {
        jquery: "lib/jquery-1.11.3.min",
        "AMUI": "lib/amazeui.2.7.1.min",
        "jquery.range": "lib/jquery.range",
        "es5": "lib/es5",
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

 
    $.ajax({
        type: "post",
        url: "../Commom/GetPVCount",
        data: { },
        success: function (result)
        {
            if (result.IsSuccess){
                $("#lblPVCount").text(result.PVCount);
            }else {
                $("#lblPVCount").text(0);
                console.log(result.Error);
            }
        }
    });

    $('#search-offcanvas').offCanvas({ effect: 'overlay' });

    $(".amap-sug-result").css("z-index", 9999);
})
