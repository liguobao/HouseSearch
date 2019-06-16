[![License: LGPL v3](https://img.shields.io/badge/License-LGPL%20v3-blue.svg)](http://www.gnu.org/licenses/lgpl-3.0)

# [地图搜租房(https://house-map.cn)](https://house-map.cn/#/)

## 这是什么?爬虫+高德地图驱动的找租房平台

- 爬虫全天不间断获取公开租房信息,汇总处理分析后落地到数据库中.

- 使用高德地图API直接在地图上展示房源位置,方便查看租房地理位置,同时提供住址到公司的路线计算（公交+地图 or 步行导航）以及预估耗时.

- 通过实时爬虫获取公开租房信息，直接在高德地图上直观展示房源位置+基础信息，同时提供住址到公司的路线计算（公交+地图 or 步行导航），已实现【豆瓣租房小组】、【豆瓣租房小程序】、【Zuber合租】、【蘑菇租房】、【CCB建融家园】、【58同城品牌公寓】、【Hi住租房】、【房多多】、【贝壳租房】、【v2ex租房帖子】、【上海互助租房】等房源信息数据爬取，部分房源价格支持筛选功能。

- 支持个人收藏房源信息,以便筛选自己合适的房子.

## [使用教程](/使用教程.md)

## [日常更新](/日常更新.md)

## [一些技巧](/一些技巧.md)

## 项目代码介绍

## 前端[House-Map.UI](/House-Map.UI)

- vue.js 冻爷[Erane](https://github.com/Erane/) 已全部完成

### 后端 [House-Map.Crawler(当前维护版本)](/House-Map.Crawler)

- 基于dotnet core 2.0,使用了 dapper ,RestSharp , Jieba.net...

- 数据库使用 MySQL, 缓存使用redis

- Crawler项目为爬虫逻辑,API项目为Web API,逻辑分离

- CI自动化发布使用Jenkins +Docker(这部分有兴趣可以看下:[手把手教你用Jenkins做Docker自动化发布](https://zhuanlan.zhihu.com/p/36509817)) PS:已废弃,全面改用Gitlab CI + k8s

- appsetting.json配置和初始化MySQL脚本

appsetting.json配置如下:
  
```json
    {
    "MySQLConnectionString": "server=mysql地址;port=端口号;database=数据库名字;uid=账号;pwd=密码;charset='utf-8';Allow User Variables=True;Connection Timeout=30;SslMode=None;",
    "RedisConnectionString": "redis数据库地址:端口,name=名字,keepAlive=1800,syncTimeout=10000,connectTimeout=360000,password=访问密码,ssl=False,abortConnect=False,responseTimeout=360000,defaultDatabase=1",
    "EmailAccount": "QQ邮箱账号",
    "EmailPassword": "QQ邮箱密码",
    "EmailSMTPDomain": "smtp.qq.com",
    "EmailSMTPPort": 587,
    "SenderAddress": "QQ邮箱账号",
    "ReceiverAddress": "QQ邮箱账号",
    "ReceiverName": "liguobao-test",
    "EncryptionConfigCIV": "加密向量,16个16进制数字",
    "EncryptionConfigCKEY": "加密秘钥,16个16进制数字"
}

```log
    
数据库初始化脚本:[HouseCrawler.Core/Dump20180512-House-Structure.sql](/HouseCrawler.Core/Dump20180512-House-Structure.sql)

数据库爬虫配置数据:[HouseCrawler.Core/Dump20180512-House-Config.sql](HouseCrawler.Core/Dump20180512-House-Config.sql)


```

1. [House-Map.Crawler/API(当前维护版本)](/House-Map.Crawler/API) 所有的API都在这里

2. [House-Map.Crawler/Crawler(当前维护版本)](/House-Map.Crawler/Crawler) 所有的Crawler都在这里, 通过改变环境变量控制启动那个爬虫


### [58HouseSearch.Core(停止维护)](/58HouseSearch.Core)

- dotnet core mvc + 实时爬虫 + 地图定位展示

### [58HouseSearch(停止维护)](58HouseSearch)

- dotnet MVC 4 + 实时爬虫 + 地图定位展示

## 感谢各位dalao鼎力支持(排名不分前后)

- 冻爷[Erane](https://github.com/Erane/)

- 小玉[piratf](https://github.com/piratf)

- 衣衣[CodeForCSharp](https://github.com/CodeForCSharp)

- ladyruo[ladyruo](https://github.com/ladyruo)

- 曾经的兔兔[TiriSane](https://github.com/TiriSane)
