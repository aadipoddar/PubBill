CREATE PROCEDURE [dbo].[Insert_DiningTable]
	@Id INT,
	@Name VARCHAR(50),
	@DiningAreaId INT,
	@Status BIT = 1
AS
BEGIN
	INSERT INTO [dbo].[DiningTable] (Name, DiningAreaId, Status)
	VALUES (@Name, @DiningAreaId, @Status);
END