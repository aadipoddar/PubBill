CREATE PROCEDURE [dbo].[Insert_Person]
	@Id INT,
	@Name VARCHAR(250),
	@Number VARCHAR(10),
	@Loyalty BIT
AS
BEGIN
	INSERT INTO [dbo].[Person] (Name, Number, Loyalty)
	VALUES (@Name, @Number, @Loyalty)
END;