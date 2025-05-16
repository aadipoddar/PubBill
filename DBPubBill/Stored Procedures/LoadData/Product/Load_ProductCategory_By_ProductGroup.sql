CREATE PROCEDURE [dbo].[Load_ProductCategory_By_ProductGroup]
	@ProductGroupID INT
AS
BEGIN
	SELECT *
	FROM dbo.ProductCategory
	WHERE ProductGroupID = @ProductGroupID;
END