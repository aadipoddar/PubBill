CREATE PROCEDURE [dbo].[Load_Recipe_By_Product]
	@ProductID INT
AS
BEGIN
	SELECT
	*
	FROM Recipe
	WHERE ProductID = @ProductID
		AND Status = 1
END