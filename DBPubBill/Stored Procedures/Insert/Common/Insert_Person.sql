CREATE PROCEDURE [dbo].[Insert_Person]
	@Id INT OUTPUT,
	@Name VARCHAR(250),
	@Number VARCHAR(10),
	@Loyalty BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Person] (Name, Number, Loyalty)
		VALUES (@Name, @Number, @Loyalty);

		SET @Id = SCOPE_IDENTITY();
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Person]
		SET Name = @Name, Number = @Number, Loyalty = @Loyalty
		WHERE Id = @Id;
	END

	SELECT @Id AS Id;
END