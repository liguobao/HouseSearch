[![License: LGPL v3](https://img.shields.io/badge/License-LGPL%20v3-blue.svg)](http://www.gnu.org/licenses/lgpl-3.0)

# 微信小程序

- 关注【人生删除指南】微信公众号体验
- 小程序中搜索“HouseMap2048”

# [地图搜租房(https://house-map.cn)](https://house-map.cn)

- 微信小程序正在开发中，欢迎关注【人生删除指南】微信公众号提前体验。

## 这是什么?爬虫+高德地图驱动的找租房平台

- 爬虫全天不间断获取公开租房信息,汇总处理分析后落地到数据库中.

- 使用高德地图 API 直接在地图上展示房源位置,方便查看租房地理位置,同时提供住址到公司的路线计算（公交+地图 or 步行导航）以及预估耗时.

- 通过实时爬虫获取公开租房信息，直接在高德地图上直观展示房源位置+基础信息，同时提供住址到公司的路线计算（公交+地图 or 步行导航），已实现【豆瓣租房小组】、【豆瓣租房小程序】、【Zuber 合租】、【蘑菇租房】、【CCB 建融家园】、【58 同城品牌公寓】、【Hi 住租房】、【房多多】、【贝壳租房】、【v2ex 租房帖子】、【上海互助租房】等房源信息数据爬取，部分房源价格支持筛选功能。

- 支持个人收藏房源信息,以便筛选自己合适的房子.

## [使用教程](/使用教程.md)

## [日常更新](/日常更新.md)

## [一些技巧](/一些技巧.md)

## 项目代码介绍

### 前端[House-Map.UI](/House-Map.UI)

- vue.js 冻爷[Erane](https://github.com/Erane/) 已全部完成

### 后端 [House-Map.Crawler(当前维护版本)](/House-Map.Crawler)

- 基于 .NET Core 2.2, 使用了 dapper ,RestSharp , Jieba.net...

- 数据库使用 MySQL, 缓存使用 Redis，关键字查询使用 elasticSearch

- Crawler 项目为爬虫逻辑;API 项目为 Web API

- CI 自动化发布使用 Jenkins +Docker(这部分有兴趣可以看下:[手把手教你用 Jenkins 做 Docker 自动化发布](https://zhuanlan.zhihu.com/p/36509817))

- 2019 年已废弃 Jenkins,全面改用 Gitlab CI + k8s

- 数据库表结构脚本 [Dump20200103_house_map.sql](/Dump20200103_house_map.sql), 房源爬虫配置脚本[北京地区 beijing-config.sql](/beijing-config.sql)

## 感谢各位 dalao 鼎力支持(排名不分前后)

- 冻爷[Erane](https://github.com/Erane/)

- 小玉[piratf](https://github.com/piratf)

- 衣衣[CodeForCSharp](https://github.com/CodeForCSharp)

- ladyruo[ladyruo](https://github.com/ladyruo)

- 曾经的兔兔[TiriSane](https://github.com/TiriSane)
