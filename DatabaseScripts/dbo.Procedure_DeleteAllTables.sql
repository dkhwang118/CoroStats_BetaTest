CREATE PROCEDURE [dbo].[Procedure_DeleteAllTables]

AS

		DROP TABLE IF EXISTS [dbo].[Settings]
		DROP TABLE IF EXISTS [dbo].[WHO_Region]
		DROP TABLE IF EXISTS [dbo].[CountryInfo]
		DROP TABLE IF EXISTS [dbo].[CountryRegion]
		DROP TABLE IF EXISTS [dbo].[CoronavirusDate]
		DROP TABLE IF EXISTS [dbo].[NewCoronavirusCasesByDate]
		DROP TABLE IF EXISTS [dbo].[NewCoronavirusDeathsByDate]
		DROP TABLE IF EXISTS [dbo].[TotalValues]

RETURN 0
