CREATE PROCEDURE [dbo].[Load_Person_By_Number]
	@Number varchar(10)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM Person WHERE Number = ISNULL(@Number, '')
END