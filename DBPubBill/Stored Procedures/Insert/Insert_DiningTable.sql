CREATE PROCEDURE [dbo].[Insert_DiningTable]
	@Id INT,
	@Name VARCHAR(50),
	@DiningAreaId INT,
	@Running BIT = 0,
	@Status BIT = 1
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO DiningTable (Name, DiningAreaId, Running, Status)
		VALUES (@Name, @DiningAreaId, @Running, @Status);
	END

	ELSE
	BEGIN
		UPDATE DiningTable
		SET Name = @Name, DiningAreaId = @DiningAreaId, Running = @Running, Status = @Status
		WHERE Id = @Id;
	END
END;