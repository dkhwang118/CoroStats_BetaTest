CREATE PROCEDURE [dbo].[InitializeDB]
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
			[Name] varchar(50) NOT NULL,
			[Value] int NOT NULL
		)

		INSERT INTO [dbo].[Settings] (Name, Value)
		VALUES ('FirstTimeStartup', 0) 
	END	
RETURN 0