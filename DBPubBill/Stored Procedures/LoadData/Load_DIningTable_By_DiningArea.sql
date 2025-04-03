CREATE PROCEDURE [dbo].[Load_DIningTable_By_DiningArea]
	@DiningAreaId INT
AS
BEGIN
	SELECT *
	FROM [dbo].[DiningTable]
	WHERE DiningAreaId = @DiningAreaId;
END;