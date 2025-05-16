CREATE PROCEDURE [dbo].[Load_DiningArea_By_Location]
	@LocationId INT
AS
BEGIN
	SELECT *
	FROM [dbo].[DiningArea]
	WHERE LocationId = @LocationId;
END;