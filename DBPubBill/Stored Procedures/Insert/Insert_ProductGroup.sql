CREATE PROCEDURE [dbo].[Insert_ProductGroup]
	@Id INT,
	@Name VARCHAR(50),
	@Status BIT
AS
BEGIN
	INSERT INTO [dbo].[ProductGroup] (Name, Status)
	VALUES (@Name, @Status);
END;