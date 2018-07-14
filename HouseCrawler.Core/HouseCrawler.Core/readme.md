# HouseCrawler.Core

## dotnet core环境准备

- [安装dotnet core SDK](https://www.microsoft.com/net/download/macos)

- [安装visualstudio code](https://code.visualstudio.com/)

## 运行程序 

```sh
dotnet run;
```

或者直接使用VS code debug

## 项目文件说明

### Common

公共方法类都在这里,一些工具类也在这里

### Crawlers

爬虫类,爬虫逻辑基本都在这里

### Dapper

数据库访问Dapper,把N多的手写SQL全部封装成不同的Dapper,类似mybatis的mapping

### Controllers

MVC中的Controller,继承于Controller,REST API或者普通Controller都在这里面

### Jobs

使用简易定时任务框架Pomelo.AspNetCore.TimedJob实现的定时任务,继承Job类,重写run方法+配置运行时间即可

### Service

逻辑层代码,业务逻辑都应该封装在这里

### wwwroot

前端代码:CSS + JS + 其他静态资源,View会使用这些文件

### Views

一般Controller都有自己的对应的View文件夹,用于存放不同的Controller View

### Models

MVC中的M,一般是View中使用的Model,不复杂的话DBModel和ViewModel可以共用

### appsettings.json

配置文件,数据库连接字符串/邮箱账号密码都在里面

### Program.cs

程序主入口,指定端口号之类的直接在此处bind

### Startup.cs

各种启动环境 + 依赖注入配置 + 各种中间件注入

- ConfigureServices 依赖注入

- Configure 中间件引入

## 建表SQL：

CREATE TABLE `CrawlerConfigurations` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `ConfigurationName` varchar(50) DEFAULT NULL COMMENT '配置名称',
  `ConfigurationValue` varchar(500) DEFAULT NULL COMMENT '配置值，一般是json',
  `DataCreateTime` datetime DEFAULT NULL COMMENT '创建时间',
  `IsEnabled` tinyint(1) DEFAULT NULL COMMENT '是否有效',
  `ConfigurationKey` int(11) DEFAULT NULL COMMENT 'key',
  `DataChange_LastTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '时间戳',
  PRIMARY KEY (`ID`),
  KEY `idx_key` (`ConfigurationKey`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `HouseInfos` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `HouseTitle` varchar(2048) DEFAULT NULL COMMENT '标题',
  `HouseOnlineURL` varchar(512) DEFAULT NULL COMMENT '房间URL',
  `HouseLocation` varchar(2048) DEFAULT NULL COMMENT '地理位置（一般用于定位）',
  `DisPlayPrice` varchar(64) DEFAULT NULL COMMENT '价钱（可能非纯数字）',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `HousePrice` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `LocationCityName` varchar(64) DEFAULT NULL,
  `DataCreateTime` datetime DEFAULT NULL,
  `SoureceDaminURL` varchar(512) DEFAULT NULL,
  `HouseInfoscol` varchar(45) DEFAULT NULL,
  `HouseText` varchar(8000) DEFAULT NULL,
  `DataChange_LastTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `idx_onlineurl` (`HouseOnlineURL`(191))
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;
