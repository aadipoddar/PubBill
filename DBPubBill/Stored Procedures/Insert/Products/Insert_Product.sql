CREATE PROCEDURE [dbo].[Insert_Product]
	@Id INT,
	@Name VARCHAR(250),
	@Code VARCHAR(50),
	@ProductCategoryId INT,
	@Rate MONEY,
	@TaxId INT,
	@KitchenTypeId INT,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Product] (Name, Code, ProductCategoryId, TaxId, Rate, KitchenTypeId, Status)
		VALUES (@Name, @Code, @ProductCategoryId, @TaxId, @Rate, @KitchenTypeId, @Status);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Product]
		SET Name = @Name, Code = @Code, ProductCategoryId = @ProductCategoryId, Rate = @Rate, TaxId = @TaxId, KitchenTypeId = @KitchenTypeId, Status = @Status
		WHERE Id = @Id;
	END
END;