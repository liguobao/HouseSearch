define(function () {

    //获取URL中的参数
    var getQueryString=  function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }
    return {
        getQueryString: getQueryString,
    };
})
