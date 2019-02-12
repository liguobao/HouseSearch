# HouseCrawler.Core

## dotnet core环境准备

- [安装dotnet core SDK](https://www.microsoft.com/net/download/macos)

- [安装visualstudio code](https://code.visualstudio.com/)

## 运行程序 

```sh
dotnet run;
```

或者直接使用VS code debug

## 项目文件说明

### Crawlers

爬虫代码,通用父类,具体实现子类

### Controllers

MVC中的Controller,继承于Controller,REST API或者普通Controller都在这里面

### Dapper

数据库访问Dapper,把手写SQL全部封装成不同的Dapper,类似mybatis的mapping

### Jobs

使用简易定时任务框架Pomelo.AspNetCore.TimedJob实现的定时任务,继承Job类,重写run方法+配置运行时间即可

### Service

逻辑层代码,业务逻辑都应该封装在这里

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
