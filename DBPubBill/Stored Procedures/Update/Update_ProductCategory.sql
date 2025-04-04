CREATE PROCEDURE [dbo].[Update_ProductCategory]
	@Id INT,
	@Name VARCHAR(50),
	@ProductGroupId INT,
	@Status BIT
AS
BEGIN
	UPDATE [ProductCategory]
	SET Name = @Name, ProductGroupId = @ProductGroupId, Status = @Status
	WHERE Id = @Id;
END;