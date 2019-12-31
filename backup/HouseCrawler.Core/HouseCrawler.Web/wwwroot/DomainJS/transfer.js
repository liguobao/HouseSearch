define(['mapSignleton', 'city', 'vehicle'], function (mapSignleton, city, vehicle) {
    var _map = mapSignleton.map;
    var _infoWindow = new AMap.InfoWindow({
        offset: new AMap.Pixel(0, -30)
    });
    var _amapTransfer = null;

    var add = function (e, address) {
        _infoWindow.setContent(e.target.content);
        _infoWindow.open(_map, e.target.getPosition());
        if (_amapTransfer) _amapTransfer.clear();

        if (vehicle.type != 'WALKING') {
            _amapTransfer = new AMap.Transfer({
                map: _map,
                policy: AMap.TransferPolicy.LEAST_TIME,
                city: city.name,
                panel: 'transfer-panel'
            });
        } else {
            _amapTransfer = new AMap.Walking({
                map: _map,
                panel: "transfer-panel",
                city: city.name,
            });
        }

        _amapTransfer.search([{
                keyword: mapSignleton.workAddress
            },
            {
                keyword: address
            }
        ], function (status, result) {});
        $("#transfer-panel").show();
    }

    var addAddress = function (address) {
        if (_amapTransfer) _amapTransfer.clear();

        if (vehicle.type != 'WALKING') {
            _amapTransfer = new AMap.Transfer({
                map: _map,
                policy: AMap.TransferPolicy.LEAST_TIME,
                city: city.name,
                panel: 'transfer-panel'
            });
        } else {
            _amapTransfer = new AMap.Walking({
                map: _map,
                panel: "transfer-panel",
                city: city.name,
            });
        }

        _amapTransfer.search([{
                keyword: mapSignleton.workAddress
            },
            {
                keyword: address
            }
        ], function (status, result) {});
        $("#transfer-panel").show();
    }

    var showContent = function (e, address) {
        _infoWindow.setContent(e.target.content);
        _infoWindow.open(_map, e.target.getPosition());
        if (_amapTransfer) _amapTransfer.clear();
    }

    return {
        add: add,
        addAddress:addAddress,
        showContent: showContent
    }
});
