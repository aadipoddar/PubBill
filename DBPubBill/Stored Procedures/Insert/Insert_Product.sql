CREATE PROCEDURE [dbo].[Insert_Product]
	@Id INT,
	@Name VARCHAR(250),
	@Prize MONEY,
	@ProductCategoryId INT,
	@Status BIT
AS
BEGIN
	INSERT INTO [Product] (Name, Prize, ProductCategoryId, Status)
	VALUES (@Name, @Prize, @ProductCategoryId, @Status);
END;