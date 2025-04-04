CREATE PROCEDURE [dbo].[Update_Product]
	@Id INT,
	@Name VARCHAR(250),
	@Code VARCHAR(50),
	@Rate MONEY,
	@ProductCategoryId INT,
	@Status BIT
AS
BEGIN
	UPDATE [Product]
	SET
		Name = @Name,
		Code = @Code,
		Rate = @Rate,
		ProductCategoryId = @ProductCategoryId,
		Status = @Status
	WHERE Id = @Id;
END;