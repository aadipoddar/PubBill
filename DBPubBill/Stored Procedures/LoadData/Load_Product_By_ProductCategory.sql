CREATE PROCEDURE [dbo].[Load_Product_By_ProductCategory]
	@ProductCategoryID INT
AS
BEGIN
	SELECT *
	FROM dbo.Product
	WHERE ProductCategoryID = @ProductCategoryID;
END