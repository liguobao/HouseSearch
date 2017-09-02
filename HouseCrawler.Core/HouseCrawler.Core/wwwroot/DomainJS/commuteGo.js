// 通勤方式选择相关
var commuteGo = define(['workLocation', 'vehicle'], function(workLocation, vehicle) {
    var bus = function(radio) {
        this.go(radio);
    };

    var subway = function(radio) {
        this.go(radio);
    };

    var walking = function(radio) {
        this.go(radio);
    };

    var go = function (e) {
        vehicle.type = e.toElement.value;
        console.log(vehicle.type);
        workLocation.load();
    }

    return {
        get vehicle() {
            return vehicle.type;
        },
        bus: bus,
        subway: subway,
        walking: walking,
        go: go
    };
});