CREATE PROCEDURE [dbo].[Update_Person]
	@Id INT,
	@Name VARCHAR(250),
	@Number VARCHAR(10),
	@Loyalty BIT
AS
BEGIN
	UPDATE [dbo].[Person]
	SET
		Name = @Name,
		Number = @Number,
		Loyalty = @Loyalty
	WHERE Id = @Id
END;