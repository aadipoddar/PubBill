CREATE PROCEDURE [dbo].[Update_Product]
	@Id INT,
	@Name VARCHAR(250),
	@Prize MONEY,
	@ProductCategoryId INT,
	@Status BIT
AS
BEGIN
	UPDATE [Product]
	SET
		Name = @Name,
		Prize = @Prize,
		ProductCategoryId = @ProductCategoryId,
		Status = @Status
	WHERE Id = @Id;
END;