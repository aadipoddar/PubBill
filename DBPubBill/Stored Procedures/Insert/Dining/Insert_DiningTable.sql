CREATE PROCEDURE [dbo].[Insert_DiningTable]
	@Id INT,
	@Name VARCHAR(50),
	@DiningAreaId INT,
	@Status BIT = 1
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO DiningTable (Name, DiningAreaId, Status)
		VALUES (@Name, @DiningAreaId, @Status);
	END

	ELSE
	BEGIN
		UPDATE DiningTable
		SET Name = @Name, DiningAreaId = @DiningAreaId, Status = @Status
		WHERE Id = @Id;
	END
END;