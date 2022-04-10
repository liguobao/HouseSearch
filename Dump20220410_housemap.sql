-- MySQL dump 10.13  Distrib 8.0.26, for macos11 (x86_64)
--
-- Host: house-map.cn    Database: housemap
-- ------------------------------------------------------
-- Server version	5.7.37-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(95) COLLATE utf8mb4_bin NOT NULL,
  `ProductVersion` varchar(32) COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `anjukehouse`
--

DROP TABLE IF EXISTS `anjukehouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `anjukehouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(20) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_district` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `anxuanhouse`
--

DROP TABLE IF EXISTS `anxuanhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `anxuanhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(766) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(4000) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `apartmenthouse`
--

DROP TABLE IF EXISTS `apartmenthouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `apartmenthouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(4000) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_price` (`Price`) USING BTREE,
  KEY `idx_city_District` (`City`,`District`),
  KEY `idx_create` (`CreateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `baixinghouse`
--

DROP TABLE IF EXISTS `baixinghouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `baixinghouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `baletuhouse`
--

DROP TABLE IF EXISTS `baletuhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `baletuhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `be_volunteer_record`
--

DROP TABLE IF EXISTS `be_volunteer_record`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `be_volunteer_record` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `City` varchar(20) CHARACTER SET utf8mb4 DEFAULT NULL,
  `UserName` varchar(50) CHARACTER SET utf8mb4 DEFAULT NULL,
  `WechatId` varchar(128) CHARACTER SET utf8mb4 DEFAULT NULL,
  `Email` varchar(128) CHARACTER SET utf8mb4 DEFAULT NULL,
  `UserId` bigint(20) NOT NULL,
  `Remark` varchar(128) CHARACTER SET utf8mb4 DEFAULT NULL,
  `CreateTime` datetime(6) NOT NULL,
  `UpdateTime` datetime(6) NOT NULL,
  `AuditStatus` varchar(20) CHARACTER SET utf8mb4 DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_user` (`UserId`),
  UNIQUE KEY `UK_wechat_id` (`WechatId`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `beikehouse`
--

DROP TABLE IF EXISTS `beikehouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `beikehouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ccbhouse`
--

DROP TABLE IF EXISTS `ccbhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ccbhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `ReportNum` int(11) DEFAULT '0',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `chengduhouse`
--

DROP TABLE IF EXISTS `chengduhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chengduhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `ReportNum` int(11) DEFAULT '0',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  `District` varchar(16) DEFAULT '',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `city_location`
--

DROP TABLE IF EXISTS `city_location`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `city_location` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `city` varchar(45) DEFAULT NULL,
  `groupName` varchar(45) DEFAULT NULL,
  `groupType` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `longitude` double DEFAULT '0',
  `latitude` double DEFAULT '0',
  `score` int(11) DEFAULT '0',
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `idx_city_name` (`city`,`groupName`,`name`,`score`),
  KEY `idx_city` (`city`),
  KEY `idx_type` (`city`)
) ENGINE=InnoDB AUTO_INCREMENT=5460 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cjiahouse`
--

DROP TABLE IF EXISTS `cjiahouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cjiahouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `config`
--

DROP TABLE IF EXISTS `config`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `config` (
  `Id` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `City` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Source` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PageCount` int(11) DEFAULT NULL,
  `Json` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `Score` int(11) DEFAULT '0',
  `HouseCount` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `Source` (`Source`),
  KEY `Source_City_Json` (`Source`,`City`,`Json`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `doubanhouse`
--

DROP TABLE IF EXISTS `doubanhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doubanhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(20) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` longtext COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_city_District` (`City`,`District`),
  KEY `idx_create_time` (`CreateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `doubanwechathouse`
--

DROP TABLE IF EXISTS `doubanwechathouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doubanwechathouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(4000) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `extend_data`
--

DROP TABLE IF EXISTS `extend_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `extend_data` (
  `id` bigint(20) NOT NULL,
  `data` text COLLATE utf8mb4_unicode_ci,
  `create_time` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `update_time` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fangduoduohouse`
--

DROP TABLE IF EXISTS `fangduoduohouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fangduoduohouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(40) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(40) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fangfeibuhouse`
--

DROP TABLE IF EXISTS `fangfeibuhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fangfeibuhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(20) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` longtext COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_city_District` (`City`,`District`),
  KEY `idx_create_time` (`CreateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fangtianxiahouse`
--

DROP TABLE IF EXISTS `fangtianxiahouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fangtianxiahouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(40) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(40) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ganjihouse`
--

DROP TABLE IF EXISTS `ganjihouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ganjihouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(20) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` longtext COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `PublishTime` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `governmenthouse`
--

DROP TABLE IF EXISTS `governmenthouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `governmenthouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(4000) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `hezuzhaoshiyouhouse`
--

DROP TABLE IF EXISTS `hezuzhaoshiyouhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `hezuzhaoshiyouhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(6000) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `hizhuhouse`
--

DROP TABLE IF EXISTS `hizhuhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `hizhuhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `hkhouse`
--

DROP TABLE IF EXISTS `hkhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `hkhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `ReportNum` int(11) DEFAULT '0',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `house_comment`
--

DROP TABLE IF EXISTS `house_comment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `house_comment` (
  `id` bigint(20) NOT NULL,
  `UserName` varchar(100) DEFAULT NULL,
  `HouseId` varchar(45) DEFAULT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  `Content` varchar(1024) DEFAULT NULL,
  `HouseURL` varchar(512) DEFAULT NULL,
  `HouseSource` varchar(20) DEFAULT NULL,
  `City` varchar(20) DEFAULT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_user` (`UserId`),
  KEY `idx_house` (`HouseId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `house_report`
--

DROP TABLE IF EXISTS `house_report`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `house_report` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `City` varchar(20) CHARACTER SET utf8mb4 DEFAULT NULL,
  `UserName` varchar(50) CHARACTER SET utf8mb4 DEFAULT NULL,
  `HouseId` varchar(45) CHARACTER SET utf8mb4 DEFAULT NULL,
  `UserId` bigint(20) NOT NULL,
  `Remark` varchar(128) CHARACTER SET utf8mb4 DEFAULT NULL,
  `ExtendData` longtext CHARACTER SET utf8mb4,
  `HouseURL` varchar(766) CHARACTER SET utf8mb4 DEFAULT NULL,
  `HouseSource` varchar(20) CHARACTER SET utf8mb4 DEFAULT NULL,
  `ReportType` varchar(20) CHARACTER SET utf8mb4 DEFAULT NULL,
  `CreateTime` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UC_user_house` (`HouseId`,`UserId`,`ReportType`)
) ENGINE=InnoDB AUTO_INCREMENT=163 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `housedata`
--

DROP TABLE IF EXISTS `housedata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `housedata` (
  `ID` varchar(45) NOT NULL,
  `JsonData` json DEFAULT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `OnlineURL` varchar(512) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `URL` (`OnlineURL`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `huzuhouse`
--

DROP TABLE IF EXISTS `huzuhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `huzuhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(4000) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `leyoujiahouse`
--

DROP TABLE IF EXISTS `leyoujiahouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `leyoujiahouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(20) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_district` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `moguhouse`
--

DROP TABLE IF EXISTS `moguhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `moguhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `notice`
--

DROP TABLE IF EXISTS `notice`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notice` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Content` longtext,
  `DataCreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `DataChange_LastTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `EndShowDate` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `nuanhouse`
--

DROP TABLE IF EXISTS `nuanhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nuanhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `userhouse` int(11) DEFAULT '0',
  `HezuzhaoshiyouHouse` int(11) DEFAULT '0',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(4000) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pinshiyouhouse`
--

DROP TABLE IF EXISTS `pinshiyouhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pinshiyouhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `qingkehouse`
--

DROP TABLE IF EXISTS `qingkehouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `qingkehouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(7000) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `shanghaihuzuhouse`
--

DROP TABLE IF EXISTS `shanghaihuzuhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shanghaihuzuhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(20) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` longtext COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `shpublichouse`
--

DROP TABLE IF EXISTS `shpublichouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shpublichouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(20) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_district` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `usercollection`
--

DROP TABLE IF EXISTS `usercollection`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usercollection` (
  `ID` varchar(45) NOT NULL,
  `UserID` bigint(20) DEFAULT NULL,
  `HouseID` varchar(45) NOT NULL,
  `Source` varchar(45) DEFAULT NULL,
  `City` varchar(50) DEFAULT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL,
  `Deleted` int(11) DEFAULT '0',
  `Title` varchar(500) DEFAULT NULL,
  `OnlineURL` varchar(500) DEFAULT NULL,
  `HouseJson` text,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `house_source_user` (`HouseID`,`UserID`),
  KEY `idx_UserID` (`UserID`) USING BTREE,
  KEY `idx_HouseID` (`HouseID`) USING BTREE,
  KEY `city_user` (`City`,`UserID`),
  KEY `user_city` (`UserID`,`City`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `userhouse`
--

DROP TABLE IF EXISTS `userhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `userhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(8120) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` double DEFAULT NULL COMMENT '经度',
  `Latitude` double DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `UserId` bigint(20) DEFAULT NULL,
  `WechatId` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `userinfos`
--

DROP TABLE IF EXISTS `userinfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `userinfos` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `Email` varchar(100) DEFAULT '',
  `UserName` varchar(100) DEFAULT '',
  `Password` varchar(40) DEFAULT '',
  `ActivatedCode` varchar(100) DEFAULT '',
  `ActivatedTime` datetime DEFAULT NULL,
  `RetrievePasswordToken` varchar(500) DEFAULT NULL,
  `DataCreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `DataChange_LastTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `Status` int(11) DEFAULT '0',
  `QQOpenUID` varchar(128) DEFAULT NULL,
  `QQ` varchar(45) DEFAULT NULL,
  `Mobile` varchar(45) DEFAULT NULL,
  `Intro` varchar(255) DEFAULT NULL,
  `WorkAddress` varchar(255) DEFAULT NULL,
  `WechatOpenID` varchar(128) DEFAULT NULL,
  `AvatarUrl` varchar(300) DEFAULT NULL,
  `JsonData` varchar(8000) DEFAULT NULL,
  `NickName` varchar(64) DEFAULT NULL,
  `UserType` int(11) DEFAULT '0' COMMENT '用户类型，0 普通用户，1 观察者（可举报），2 志愿者（可删除）',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `username` (`UserName`),
  UNIQUE KEY `email` (`Email`,`QQOpenUID`),
  KEY `idx_UserName` (`UserName`),
  KEY `idx_DataChange_LastTime` (`DataChange_LastTime`),
  KEY `idx_email` (`Email`),
  KEY `idx_code` (`ActivatedCode`),
  KEY `idx_QQOpenUID` (`QQOpenUID`)
) ENGINE=InnoDB AUTO_INCREMENT=28372 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `v2exhouse`
--

DROP TABLE IF EXISTS `v2exhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `v2exhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(4096) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `wellceehouse`
--

DROP TABLE IF EXISTS `wellceehouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `wellceehouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(8000) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `woaiwojiahouse`
--

DROP TABLE IF EXISTS `woaiwojiahouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `woaiwojiahouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(20) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` longtext COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `PublishTime` bigint(20) DEFAULT NULL,
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `xhjhouse`
--

DROP TABLE IF EXISTS `xhjhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `xhjhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(4000) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `xianyuhouse`
--

DROP TABLE IF EXISTS `xianyuhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `xianyuhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `HezuzhaoshiyouHouse` int(11) DEFAULT '0',
  `PinshiyouHouse` int(11) DEFAULT '0',
  `HizhuHouse` int(11) DEFAULT '0',
  `ReportNum` int(11) DEFAULT '0',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(4096) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `zhihuhouse`
--

DROP TABLE IF EXISTS `zhihuhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `zhihuhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `zhugehouse`
--

DROP TABLE IF EXISTS `zhugehouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `zhugehouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(8000) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ziroomhouse`
--

DROP TABLE IF EXISTS `ziroomhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ziroomhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(7000) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `zuberhouse`
--

DROP TABLE IF EXISTS `zuberhouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `zuberhouse` (
  `Id` varchar(45) NOT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `City` varchar(64) DEFAULT NULL,
  `Title` varchar(512) DEFAULT NULL COMMENT '标题',
  `Price` decimal(10,0) DEFAULT NULL COMMENT '价格（纯数字）',
  `Text` mediumtext COMMENT '文字描述',
  `Location` varchar(512) DEFAULT NULL COMMENT '地理位置,文字描述',
  `OnlineURL` varchar(512) DEFAULT NULL COMMENT 'URL',
  `PubTime` datetime DEFAULT NULL COMMENT '发布时间',
  `Source` varchar(200) DEFAULT NULL COMMENT '来源',
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `District` varchar(16) DEFAULT '',
  `ReportNum` int(11) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_city_District` (`City`,`District`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-04-10 19:23:55
