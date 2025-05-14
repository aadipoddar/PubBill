CREATE PROCEDURE [dbo].[Insert_DiningAreaKitchen]
	@Id INT,
	@DiningAreaId INT,
	@KitchenId INT,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[DiningAreaKitchen] ([DiningAreaId], [KitchenId], [Status])
		VALUES (@DiningAreaId, @KitchenId, @Status);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[DiningAreaKitchen]
		SET [DiningAreaId] = @DiningAreaId,
			[KitchenId] = @KitchenId,
			[Status] = @Status
		WHERE [Id] = @Id;
	END
END;