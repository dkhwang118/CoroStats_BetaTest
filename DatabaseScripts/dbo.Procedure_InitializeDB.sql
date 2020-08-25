﻿CREATE PROCEDURE [dbo].[Procedure_InitializeDB]
	@settingsParam1 int = 0,
	@settingsParam2 varchar(50) = FirstTimeStartup,
	@settingsParam3 int = 0
AS
	IF OBJECT_ID (N'dbo.Settings', N'U') IS NOT NULL
	BEGIN
		PRINT 'DB is initialized'
	END
	ELSE
	BEGIN
		PRINT 'DB is not initialized'
		/* Initialize DB with tables and base values */
		CREATE TABLE [dbo].[Settings] (
			[Id] int IDENTITY(1,1) PRIMARY KEY,
			[Name] varchar(64) NOT NULL,
			[Value] int NOT NULL
		)

		INSERT INTO [dbo].[Settings] (Name, Value)
		VALUES ('FirstTimeStartup', 0)
		
		CREATE TABLE [dbo].[WHO_Region] (
			[RegionId] int IDENTITY(1,1) PRIMARY KEY,
			[WHO_Region] varchar(16) NOT NULL
		)

		CREATE TABLE [dbo].[CountryInfo] (
			[CountryId] int IDENTITY(1,1) PRIMARY KEY,
			[CountryCode] varchar(8) NOT NULL,
			[Name] varchar(64) NOT NULL,
			[WHO_RegionId] int NOT NULL,
			CONSTRAINT FK_CountryInfo_WHORegion_WHO_RegionId FOREIGN KEY (WHO_RegionId)
			REFERENCES [dbo].[WHO_Region] (RegionId),
			[Population] int,
			[TotalCoronavirusCases] int NOT NULL,
			[TotalCoronavirusDeaths] int NOT NULL
		)

		CREATE TABLE [dbo].[CountryRegion] (
			[CountryRegionId] int IDENTITY(1,1),
			CONSTRAINT PK_CountryRegion_CountryRegionId PRIMARY KEY CLUSTERED (CountryRegionId),
			[Name] varchar(64) NOT NULL,
			[CountryId] int NOT NULL,
			CONSTRAINT FK_CountryRegion_CountryInfo_CountryId FOREIGN KEY (CountryId)
			REFERENCES [dbo].[CountryInfo] (CountryId)
		)

		CREATE TABLE [dbo].[CoronavirusDate] (
			[DateId] int IDENTITY(1,1) PRIMARY KEY,
			[Date] Date NOT NULL
		)

		CREATE TABLE [dbo].[NewCoronavirusCasesByDate] (
			[CountryId] int NOT NULL,
			CONSTRAINT FK_CountryInfo_NewCoronavirusCasesByDate_CountryId FOREIGN KEY (CountryId)
			REFERENCES [dbo].[CountryInfo] (CountryId),
			[DateId] int NOT NULL,
			CONSTRAINT FK_CoronavirusDate_NewCoronavirusCasesByDate_DateId FOREIGN KEY (DateId)
			REFERENCES [dbo].[CoronavirusDate] (DateId),
			CONSTRAINT PK_NewCoronavirusCasesByDate_CountryId_DateId PRIMARY KEY CLUSTERED (CountryId, DateId),
			[NewCases] int NOT NULL
		)

		CREATE TABLE [dbo].[NewCoronavirusDeathsByDate] (
			[CountryId] int NOT NULL,
			CONSTRAINT FK_CountryInfo_NewCoronavirusDeathsByDate_CountryId FOREIGN KEY (CountryId)
			REFERENCES [dbo].[CountryInfo] (CountryId),
			[DateId] int NOT NULL,
			CONSTRAINT FK_CoronavirusDate_NewCoronavirusDeathsByDate_DateId FOREIGN KEY (DateId)
			REFERENCES [dbo].[CoronavirusDate] (DateId),
			CONSTRAINT PK_NewCoronavirusCasesByDate_CountryId_DateId PRIMARY KEY CLUSTERED (CountryId, DateId),
			[NewDeaths] int NOT NULL
		)

		CREATE TABLE [dbo].[TotalValues] (
			[ValueId] int IDENTITY(1,1) NOT NULL,
			CONSTRAINT PK_TotalValues_ValueId PRIMARY KEY CLUSTERED (ValueId),
			[TotalCoronavirusCases] int NOT NULL,
			[TotalCoronavirusDeaths] int NOT NULL,
			[TotalCoronavirusRecoveredCases] int
		)


	END	
RETURN 0