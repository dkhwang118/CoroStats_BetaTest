CREATE PROCEDURE [dbo].[Procedure_AddCountryInfo_SingleRow_NoPop]
	
	@CountryCode varchar(8),
	@Name varchar(64),
	@WHO_RegionId int, 
	@TotalCoronavirusCases int, 
	@TotalCoronavirusDeaths int 


AS

	INSERT INTO [dbo].CountryInfo (CountryCode, [Name], WHO_RegionId, TotalCoronavirusCases, TotalCoronavirusDeaths)
	VALUES (@CountryCode, @Name, @WHO_RegionId, @TotalCoronavirusCases, @TotalCoronavirusDeaths)

RETURN 0
