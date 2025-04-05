CREATE PROCEDURE [dbo].[Insert_ProductCategory]
	@Id INT,
	@Name VARCHAR(50),
	@ProductGroupId INT,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[ProductCategory] (Name, ProductGroupId, Status)
		VALUES (@Name, @ProductGroupId, @Status);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[ProductCategory]
		SET Name = @Name, ProductGroupId = @ProductGroupId, Status = @Status
		WHERE Id = @Id;
	END
END;