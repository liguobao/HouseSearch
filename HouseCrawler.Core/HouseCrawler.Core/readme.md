建表SQL：


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
) ENGINE=InnoDB AUTO_INCREMENT=435 DEFAULT CHARSET=utf8mb4;


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
) ENGINE=InnoDB AUTO_INCREMENT=1419 DEFAULT CHARSET=utf8mb4;
