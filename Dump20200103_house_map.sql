-- MySQL dump 10.13  Distrib 8.0.17, for Linux (x86_64)
--
-- Host: sh-cdb-3wke7jj8.sql.tencentcdb.com    Database: housemap
-- ------------------------------------------------------
-- Server version	5.7.18-txsql-log

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
SET @MYSQLDUMP_TEMP_LOG_BIN = @@SESSION.SQL_LOG_BIN;
SET @@SESSION.SQL_LOG_BIN= 0;

--
-- GTID state at the beginning of the backup 
--

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ '359f341b-cf84-11e8-a035-246e968e60ca:1-5396626,
3dcdf3da-cf84-11e8-8e4f-246e968ac352:1-7624,
c1023416-1187-11e9-b993-6c92bf5f0af2:1-18677778,
c9ab5a60-1187-11e9-885b-6c92bf5e2934:1-154';

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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_price` (`Price`) USING BTREE
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  `PublishTime` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`),
  KEY `idx_status` (`Status`),
  KEY `idx_publish_time` (`PublishTime`),
  KEY `idx_price` (`Price`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4;
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  KEY `idx_HouseID` (`HouseID`) USING BTREE
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
  `PicURLs` varchar(1024) DEFAULT '[]' COMMENT '图片URL',
  `RentType` int(11) DEFAULT '0' COMMENT '出租类型',
  `Longitude` varchar(20) DEFAULT NULL COMMENT '经度',
  `Latitude` varchar(20) DEFAULT NULL COMMENT '维度',
  `Tags` varchar(255) DEFAULT NULL,
  `Labels` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT '0',
  `UserId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  `JsonData` varchar(1000) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `username` (`UserName`),
  UNIQUE KEY `email` (`Email`,`QQOpenUID`),
  KEY `idx_UserName` (`UserName`),
  KEY `idx_DataChange_LastTime` (`DataChange_LastTime`),
  KEY `idx_email` (`Email`),
  KEY `idx_code` (`ActivatedCode`),
  KEY `idx_QQOpenUID` (`QQOpenUID`)
) ENGINE=InnoDB AUTO_INCREMENT=1532 DEFAULT CHARSET=utf8mb4;
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_OnlineURL` (`OnlineURL`),
  KEY `idx_PubTime` (`PubTime`) USING BTREE,
  KEY `idx_City` (`City`) USING BTREE,
  KEY `idx_Title` (`Title`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-01-03 18:46:36
