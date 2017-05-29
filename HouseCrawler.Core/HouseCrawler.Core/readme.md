建表SQL：

CREATE TABLE `CrawlerConfigurations` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `ConfigurationName` varchar(50) DEFAULT NULL,
  `ConfigurationValue` varchar(500) DEFAULT NULL,
  `DataCreateTime` datetime DEFAULT NULL,
  `IsEnabled` tinyint(1) DEFAULT NULL,
  `ConfigurationKey` int(11) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `idx_key` (`ConfigurationKey`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4;
SELECT * FROM housecrawler.CrawlerConfigurations;

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
  PRIMARY KEY (`Id`),
  KEY `idx_onlineurl` (`HouseOnlineURL`(191))
) ENGINE=InnoDB AUTO_INCREMENT=397 DEFAULT CHARSET=utf8mb4;
