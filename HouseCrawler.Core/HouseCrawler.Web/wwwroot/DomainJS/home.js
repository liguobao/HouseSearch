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

require(['domready!', 'jquery', 'AMUI', 'mapController', 'city', 'commuteGo','transfer'], function (doc, $, AMUI, mapController, city, commuteGo,transfer) {
    //city.initAllCityInfo();
    mapController.init();

    $("input[name='locationType']").bind('click', mapController.locationMethodOnChange)

    $("input[name='vehicle']").bind('click', commuteGo.go)

    $('#Get58Data').bind('click', function(e) {
        e.preventDefault();
     
        mapController.Get58DataClick();
        e.stopPropagation();
    });


    if (!isMobile()) {
        $('#search-offcanvas').offCanvas({ effect: 'overlay' });
        $("#btnCloseTransfer").hide();
        $("#divStatement").hide();
        $("#divWorkTransfer").show();
        $("#divGradientList").show();
        $("#divStatement").show();
    } else {
        $("#divWorkTransfer").hide();
        $("#divGradientList").hide();
        $("#btnCloseTransfer").show();
        $("#mobileWorkLocation").show();
    }


    $("#btnCloseTransfer").on("click", function () {
        $("#transfer-panel").hide();
    });

    var page = 1;
    $('#btnNext').bind('click', function(e) {
        e.preventDefault();
        mapController.getHouses(page++);
        e.stopPropagation();
    });


    $('body').on('click', "[name='house-star']", function () {
        var $this = $(this);
        var houseId = $this.attr("house-id");
        var source = $this.attr("source");
        $.ajax({
            type: "post",
            url: './AddUserCollection',
            data: { houseId: houseId, source: source },
            success: function (result) {
                if (result.success) {
                    alert(result.message);
                } else {
                    alert(result.error);
                }
            }
        });
    });

    $('body').on('click', "[name='house-transfer']", function () {
        var $this = $(this);
        var location = $this.attr("location");
        transfer.addAddress(location);
    });



    $(".amap-sug-result").css("z-index", 9999);
})


function isMobile() {
    try {
        if (document.getElementById("bdmark") != null) { return false; }
        var urlhash = window.location.hash;
        if (!urlhash.match("fromapp")) {
            if ((navigator.userAgent.match(/(iPhone|iPod|Android|ios|iPad|Mobile)/i))) {
                return true;
            }
        }
    } catch (err) { }
    return false;
}
