-- MySQL dump 10.13  Distrib 5.7.17, for macos10.12 (x86_64)
--
-- Host: codelover.win    Database: housecrawler
-- ------------------------------------------------------
-- Server version	5.7.20

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `ApartmentHouseInfos`
--

DROP TABLE IF EXISTS `ApartmentHouseInfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ApartmentHouseInfos` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `HouseTitle` varchar(2048) DEFAULT NULL COMMENT '标题',
  `HouseOnlineURL` varchar(512) DEFAULT NULL COMMENT '房间URL',
  `HouseLocation` varchar(2048) DEFAULT NULL COMMENT '地理位置（一般用于定位）',
  `DisPlayPrice` varchar(64) DEFAULT NULL COMMENT '价钱（可能非纯数字）',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `HousePrice` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `LocationCityName` varchar(64) DEFAULT NULL,
  `DataCreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `Source` varchar(200) DEFAULT NULL,
  `HouseText` varchar(8000) DEFAULT NULL,
  `DataChange_LastTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `IsAnalyzed` tinyint(1) DEFAULT '0',
  `Status` int(11) DEFAULT '0',
  `PicURLs` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `house` (`HouseOnlineURL`),
  KEY `idx_LocationCityName` (`LocationCityName`) USING BTREE,
  KEY `idx_PubTime` (`PubTime`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=7418864 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `CCBHouseInfos`
--

DROP TABLE IF EXISTS `CCBHouseInfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `CCBHouseInfos` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `HouseTitle` varchar(2048) DEFAULT NULL COMMENT '标题',
  `HouseOnlineURL` varchar(512) DEFAULT NULL COMMENT '房间URL',
  `HouseLocation` varchar(2048) DEFAULT NULL COMMENT '地理位置（一般用于定位）',
  `DisPlayPrice` varchar(64) DEFAULT NULL COMMENT '价钱（可能非纯数字）',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `HousePrice` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `LocationCityName` varchar(64) DEFAULT NULL,
  `DataCreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `Source` varchar(200) DEFAULT NULL,
  `HouseText` varchar(8000) DEFAULT NULL,
  `DataChange_LastTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `IsAnalyzed` tinyint(1) DEFAULT '0',
  `Status` int(11) DEFAULT '0',
  `PicURLs` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `house` (`HouseOnlineURL`),
  KEY `idx_LocationCityName` (`LocationCityName`) USING BTREE,
  KEY `idx_DataChange_LastTime` (`DataChange_LastTime`) USING BTREE,
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_HousePrice` (`HousePrice`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5985082 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `CrawlerConfigurations`
--

DROP TABLE IF EXISTS `CrawlerConfigurations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `CrawlerConfigurations` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `ConfigurationName` varchar(50) DEFAULT NULL COMMENT '配置名称',
  `ConfigurationValue` mediumtext COMMENT '配置值，一般是json',
  `DataCreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `IsEnabled` tinyint(1) DEFAULT NULL COMMENT '是否有效',
  `ConfigurationKey` int(11) DEFAULT NULL COMMENT 'key',
  `DataChange_LastTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`ID`),
  KEY `idx_key` (`ConfigurationKey`) USING BTREE,
  KEY `idx_ConfigurationName` (`ConfigurationName`) USING BTREE,
  KEY `datatime` (`DataCreateTime`)
) ENGINE=InnoDB AUTO_INCREMENT=916 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `CrawlerLogs`
--

DROP TABLE IF EXISTS `CrawlerLogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `CrawlerLogs` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `LogType` int(11) DEFAULT NULL,
  `LogContent` mediumtext,
  `DataChange_LastTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `LogTitle` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `idx_DataChange_LastTime` (`DataChange_LastTime`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=4866 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `DoubanHouseInfos`
--

DROP TABLE IF EXISTS `DoubanHouseInfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `DoubanHouseInfos` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `HouseTitle` varchar(2048) DEFAULT NULL COMMENT '标题',
  `HouseOnlineURL` varchar(512) DEFAULT NULL COMMENT '房间URL',
  `HouseLocation` varchar(2048) DEFAULT NULL COMMENT '地理位置（一般用于定位）',
  `DisPlayPrice` varchar(64) DEFAULT NULL COMMENT '价钱（可能非纯数字）',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `HousePrice` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `LocationCityName` varchar(64) DEFAULT NULL,
  `DataCreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `Source` varchar(200) DEFAULT NULL,
  `HouseText` varchar(8000) DEFAULT NULL,
  `DataChange_LastTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `IsAnalyzed` tinyint(1) DEFAULT '0',
  `Status` int(11) DEFAULT '0',
  `PicURLs` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `house` (`HouseOnlineURL`),
  KEY `idx_LocationCityName` (`LocationCityName`) USING BTREE,
  KEY `idx_DataChange_LastTime` (`DataChange_LastTime`) USING BTREE,
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_HousePrice` (`HousePrice`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=6222304 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `HouseDatas`
--

DROP TABLE IF EXISTS `HouseDatas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `HouseDatas` (
  `ID` bigint(20) NOT NULL,
  `RefID` bigint(20) DEFAULT NULL,
  `JsonData` text,
  `HouseURL` varchar(512) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `unique_HouseURL` (`HouseURL`),
  KEY `idx_HouseURL` (`HouseURL`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `HouseInfos`
--

DROP TABLE IF EXISTS `HouseInfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
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
  `Source` varchar(200) DEFAULT NULL,
  `HouseText` varchar(8000) DEFAULT NULL,
  `DataChange_LastTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `IsAnalyzed` tinyint(1) DEFAULT '0',
  `Status` int(11) DEFAULT '0',
  `PicURLs` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `house` (`HouseOnlineURL`),
  KEY `idx_LocationCityName` (`LocationCityName`) USING BTREE,
  KEY `idx_DataChange_LastTime` (`DataChange_LastTime`) USING BTREE,
  KEY `idx_DataCreateTime` (`DataCreateTime`) USING BTREE,
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_HousePrice` (`HousePrice`) USING BTREE,
  KEY `idx_onlineurl` (`HouseOnlineURL`(191))
) ENGINE=InnoDB AUTO_INCREMENT=1994775 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `MoguHouseInfos`
--

DROP TABLE IF EXISTS `MoguHouseInfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `MoguHouseInfos` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `HouseTitle` varchar(2048) DEFAULT NULL COMMENT '标题',
  `HouseOnlineURL` varchar(512) DEFAULT NULL COMMENT '房间URL',
  `HouseLocation` varchar(2048) DEFAULT NULL COMMENT '地理位置（一般用于定位）',
  `DisPlayPrice` varchar(64) DEFAULT NULL COMMENT '价钱（可能非纯数字）',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `HousePrice` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `LocationCityName` varchar(64) DEFAULT NULL,
  `DataCreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `Source` varchar(200) DEFAULT NULL,
  `HouseText` varchar(8000) DEFAULT NULL,
  `DataChange_LastTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `IsAnalyzed` tinyint(1) DEFAULT '0',
  `Status` int(11) DEFAULT '0',
  `PicURLs` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `house` (`HouseOnlineURL`),
  KEY `idx_LocationCityName` (`LocationCityName`) USING BTREE,
  KEY `idx_PubTime` (`PubTime`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=7730162 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `MutualHouseInfos`
--

DROP TABLE IF EXISTS `MutualHouseInfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `MutualHouseInfos` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `HouseTitle` varchar(2048) DEFAULT NULL COMMENT '标题',
  `HouseOnlineURL` varchar(512) DEFAULT NULL COMMENT '房间URL',
  `HouseLocation` varchar(2048) DEFAULT NULL COMMENT '地理位置（一般用于定位）',
  `DisPlayPrice` varchar(64) DEFAULT NULL COMMENT '价钱（可能非纯数字）',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `HousePrice` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `LocationCityName` varchar(64) DEFAULT NULL,
  `DataCreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `Source` varchar(200) DEFAULT NULL,
  `HouseText` varchar(8000) DEFAULT NULL,
  `DataChange_LastTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `IsAnalyzed` tinyint(1) DEFAULT '0',
  `Status` int(11) DEFAULT '0',
  `PicURLs` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `house` (`HouseOnlineURL`),
  KEY `idx_LocationCityName` (`LocationCityName`) USING BTREE,
  KEY `idx_DataChange_LastTime` (`DataChange_LastTime`) USING BTREE,
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_HousePrice` (`HousePrice`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2078629 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `UserCollections`
--

DROP TABLE IF EXISTS `UserCollections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `UserCollections` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserID` bigint(20) NOT NULL DEFAULT '0',
  `HouseID` bigint(20) NOT NULL DEFAULT '0',
  `Source` varchar(45) DEFAULT NULL,
  `HouseCity` varchar(50) DEFAULT NULL,
  `DataCreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `DataChange_LastTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` int(11) DEFAULT '0',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `house_source_user` (`HouseID`,`Source`,`UserID`),
  KEY `idx_UserID` (`UserID`) USING BTREE,
  KEY `idx_HouseID` (`HouseID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `UserInfos`
--

DROP TABLE IF EXISTS `UserInfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `UserInfos` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `Email` varchar(100) DEFAULT '',
  `UserName` varchar(100) DEFAULT '',
  `Password` varchar(40) DEFAULT '',
  `ActivatedCode` varchar(100) DEFAULT '',
  `ActivatedTime` datetime DEFAULT NULL,
  `RetrievePasswordToken` varchar(500) DEFAULT NULL,
  `TokenTime` datetime DEFAULT NULL,
  `DataCreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `DataChange_LastTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `Status` int(11) DEFAULT '0',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `email` (`Email`),
  UNIQUE KEY `username` (`UserName`),
  KEY `idx_UserName` (`UserName`),
  KEY `idx_DataChange_LastTime` (`DataChange_LastTime`),
  KEY `idx_email` (`Email`),
  KEY `idx_code` (`ActivatedCode`)
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ZuberHouseInfos`
--

DROP TABLE IF EXISTS `ZuberHouseInfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ZuberHouseInfos` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `HouseTitle` varchar(1024) DEFAULT NULL COMMENT '标题',
  `HouseOnlineURL` varchar(512) DEFAULT NULL COMMENT '房间URL',
  `HouseLocation` varchar(2048) DEFAULT NULL COMMENT '地理位置（一般用于定位）',
  `DisPlayPrice` varchar(64) DEFAULT NULL COMMENT '价钱（可能非纯数字）',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `HousePrice` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `LocationCityName` varchar(64) DEFAULT NULL,
  `DataCreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `Source` varchar(200) DEFAULT NULL,
  `HouseText` varchar(8000) DEFAULT NULL,
  `DataChange_LastTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `IsAnalyzed` tinyint(1) DEFAULT '0',
  `Status` int(11) DEFAULT '0',
  `PicURLs` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `house` (`HouseOnlineURL`),
  KEY `idx_LocationCityName` (`LocationCityName`) USING BTREE,
  KEY `idx_DataChange_LastTime` (`DataChange_LastTime`) USING BTREE,
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_HousePrice` (`HousePrice`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=3884298 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-05-12 22:48:59
