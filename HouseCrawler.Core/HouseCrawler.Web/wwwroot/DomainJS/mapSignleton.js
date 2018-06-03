"use strict";

define(function() {
    var _map = new AMap.Map("container", {
        resizeEnable: true,
        zoomEnable: true,
        center: [121.297428, 31.1345],
        zoom: 13
    });
    var _workAddress = null;

    return {
        get map() {
            return _map;
        },
        get workAddress() {
            return _workAddress;
        },
        set workAddress(value) {
            _workAddress = value;
        }
    }
});
