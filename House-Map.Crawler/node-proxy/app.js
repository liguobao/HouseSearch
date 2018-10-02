const express = require('express')
var request = require('request')
var aes = require('aes-js')
var axios = require('axios')
const app = express()
const port = 3000
key = 'Ddg54q;sg]^3lka(72%2./as+d^823sD'

app.get('/topics', (req, res) => {
    var city = req.query.city ? req.query.city : "上海";
    var page = req.query.page ? req.query.page : 1;
    var limit = req.query.limit ? req.query.limit : 10;
    var options = getTopicsSearch(city, page, limit)
    request(options, function (error, response, body) {
        if (error) {
            console.log(error);
        }
        var jsonData = decryptBody(body);
        res.send(jsonData)
    })
})


app.get('/topics/:topicId', (req, res) => {
    var topicId = req.params["topicId"];
    var options = getTopic(topicId);
    request(options, function (error, response, body) {
        if (error) {
            console.log(error);
        }
        var jsonData = decryptBody(body);
        res.send(jsonData)
    })
})



app.listen(port, () => console.log(`app listening on port ${port}!`))

function getTopic(topicId) {
    return {
        method: 'GET',
        url: 'https://fang.douban.com/api/topics/' + topicId,
        headers: {
            host: 'fang.douban.com',
            'user-agent': 'Mozilla/5.0 (Linux; Android 8.0.0; MIX Build/OPR1.170623.032; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/69.0.3497.100 Mobile Safari/537.36 MicroMessenger/6.7.3.1360(0x26070333) NetType/WIFI Language/zh_CN Process/appbrand0',
            'content-type': 'application/json',
            cookie: 'bid=',
            referer: 'https://servicewechat.com/wxaf9e2c0b8829cf6c/70/page-frame.html',
            authorization: 'Bearer ',
            charset: 'utf-8'
        }
    };
}

function decryptBody(body) {
    var reg = new RegExp('"', "g");
    body = body.replace(reg, "");
    var ecb = new aes.ModeOfOperation.ecb(aes.utils.utf8.toBytes(key));
    var source = aes.utils.hex.toBytes(body);
    var data = '';
    try {
        data = ecb.decrypt(source);
    } catch (error) {
        console.log(JSON.stringify(error));
        return "";
    }
    var buff = aes.padding.pkcs7.strip(data);
    return aes.utils.utf8.fromBytes(buff);
}

function getTopicsSearch(city, page, limit) {
    return {
        method: 'GET',
        url: 'https://fang.douban.com/api/topics',
        qs: {
            city: city,
            district_tags: '[]',
            subway_tags: '[]',
            rent_type: '',
            house_type: '',
            bedroom_type: '',
            rent_fee: '[]',
            sort: '',
            query_text: '',
            page: page,
            limit: limit
        },
        headers: {
            host: 'fang.douban.com',
            'user-agent': 'Mozilla/5.0 (Linux; Android 8.0.0; MIX Build/OPR1.170623.032; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/69.0.3497.91 Mobile Safari/537.36 MicroMessenger/6.7.2.1340(0x26070233) NetType/WIFI Language/zh_CN',
            'content-type': 'application/json',
            cookie: 'bid=',
            referer: 'https://servicewechat.com/wxaf9e2c0b8829cf6c/63/page-frame.html',
            authorization: 'Bearer ',
            charset: 'utf-8'
        }
    };
}
