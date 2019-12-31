define(function () {

    //获取URL中的参数
    var getQueryString = function (key) {
        var reg = new RegExp("(^|&)" + key + "=([^&]*)(&|$)");
        var result = window.location.search.substr(1).match(reg);
        return result ? decodeURIComponent(result[2]) : null;
    }

    var isMobile = function isMobile() {
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

    return {
        getQueryString: getQueryString,
        isMobile: isMobile
    };
})



