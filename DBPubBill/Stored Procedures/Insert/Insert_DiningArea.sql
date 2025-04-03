CREATE PROCEDURE [dbo].[Insert_DiningArea]
	@Id INT,
	@Name VARCHAR(50),
	@LocationId INT,
	@Status BIT = 1
AS
BEGIN
	INSERT INTO [dbo].[DiningArea] (Name, LocationId, Status)
	VALUES (@Name, @LocationId, @Status);
END