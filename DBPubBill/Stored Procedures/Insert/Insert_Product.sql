CREATE PROCEDURE [dbo].[Insert_Product]
	@Id INT,
	@Name VARCHAR(250),
	@Code VARCHAR(50),
	@Rate MONEY,
	@ProductCategoryId INT,
	@Status BIT
AS
BEGIN
	INSERT INTO [Product] (Name, Code, Rate, ProductCategoryId, Status)
	VALUES (@Name, @Code, @Rate, @ProductCategoryId, @Status);
END;