




-- -----------------------------------------------------------
-- Entity Designer DDL Script for MySQL Server 4.1 and higher
-- -----------------------------------------------------------
-- Date Created: 01/29/2016 12:40:22
-- Generated from EDMX file: C:\Users\m.tosti\Programmazione .NET\WS_Project_test_mqtt.net\WS_Project\ModelDB.edmx
-- Target version: 3.0.0.0
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- NOTE: if the constraint does not exist, an ignorable error will be reported.
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------
SET foreign_key_checks = 0;
    DROP TABLE IF EXISTS `DatiMeteo`;
SET foreign_key_checks = 1;

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

CREATE TABLE `DatiMeteo`(
	`ID` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`Data` datetime NOT NULL, 
	`Temperatura` double NOT NULL, 
	`Umidità` double NOT NULL, 
	`Aria` longtext NOT NULL, 
	`Pressione` longtext NOT NULL, 
	`Pioggia` double NOT NULL);

ALTER TABLE `DatiMeteo` ADD PRIMARY KEY (ID);






-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
