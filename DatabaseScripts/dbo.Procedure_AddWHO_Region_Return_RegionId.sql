CREATE PROCEDURE [dbo].[Procedure_AddWHO_Region_Return_RegionId] 
	
	@WHO_Region varchar(16) 


AS

	INSERT INTO [dbo].WHO_Region (WHO_Region) 
	output INSERTED.RegionId
	VALUES (@WHO_Region) 

RETURN