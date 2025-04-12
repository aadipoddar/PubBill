CREATE PROCEDURE [dbo].[Insert_Product]
	@Id INT,
	@Name VARCHAR(250),
	@Code VARCHAR(50),
	@ProductCategoryId INT,
	@Rate MONEY,
	@TaxId INT,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Product] (Name, Code, ProductCategoryId, TaxId, Rate, Status)
		VALUES (@Name, @Code, @ProductCategoryId, @TaxId, @Rate, @Status);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Product]
		SET Name = @Name, Code = @Code, ProductCategoryId = @ProductCategoryId, Rate = @Rate, TaxId = @TaxId, Status = @Status
		WHERE Id = @Id;
	END
END;