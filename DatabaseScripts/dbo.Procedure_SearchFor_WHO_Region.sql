CREATE PROCEDURE [dbo].[Procedure_SearchFor_WHO_Region] 
	
	@WHO_Region varchar(16) 


AS 

	SELECT RegionId 
	FROM [dbo].WHO_Region 
	WHERE WHO_Region = @WHO_Region 

RETURN