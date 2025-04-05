------CREATE PROCEDURE [dbo].[Insert_Product]
	@Id INT,
	@Name VARCHAR(250),
	@Code VARCHAR(50),
	@Rate MONEY,
	@ProductCategoryId INT,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Product] (Name, Code, Rate, ProductCategoryId, Status)
		VALUES (@Name, @Code, @Rate, @ProductCategoryId, @Status);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Product]
		SET Name = @Name, Code = @Code, Rate = @Rate, ProductCategoryId = @ProductCategoryId, Status = @Status
		WHERE Id = @Id;
	END
END;