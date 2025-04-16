CREATE PROCEDURE [dbo].[Update_Settings]
	@Id INT,
	@Key VARCHAR(50),
	@Value VARCHAR(50)
AS
BEGIN

	UPDATE Settings
	SET
		[Value] = @Value
	WHERE [Key] = @Key

END