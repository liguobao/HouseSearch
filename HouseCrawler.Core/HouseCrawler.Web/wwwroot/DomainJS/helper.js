define(function () {

    //获取URL中的参数
    var getQueryString = function (key) {
        var reg = new RegExp("(^|&)" + key + "=([^&]*)(&|$)");
        var result = window.location.search.substr(1).match(reg);
        return result ? decodeURIComponent(result[2]) : null;
    }
    return {
        getQueryString: getQueryString,
    };
})
