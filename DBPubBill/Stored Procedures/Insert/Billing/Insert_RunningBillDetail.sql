CREATE PROCEDURE [dbo].[Insert_RunningBillDetail]
	@Id INT,
	@RunningBillId INT,
	@ProductId INT,
	@Quantity INT,
	@Rate MONEY,
	@BaseTotal MONEY,
	@Instruction VARCHAR(250),
	@Discountable BIT,
	@SelfDiscount BIT,
	@DiscPercent DECIMAL(5,2),
	@DiscAmount MONEY,
	@AfterDiscount MONEY,
	@CGSTPercent DECIMAL(5,2),
	@CGSTAmount MONEY,
	@SGSTPercent DECIMAL(5,2),
	@SGSTAmount MONEY,
	@IGSTPercent DECIMAL(5,2),
	@IGSTAmount MONEY,
	@Total MONEY,
	@Cancelled BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[RunningBillDetail]
		(
			RunningBillId,
			ProductId,
			Quantity,
			Rate,
			BaseTotal,
			Instruction,
			Discountable,
			SelfDiscount,
			DiscPercent,
			DiscAmount,
			AfterDiscount,
			CGSTPercent,
			CGSTAmount,
			SGSTPercent,
			SGSTAmount,
			IGSTPercent,
			IGSTAmount,
			Total,
			Cancelled
		) VALUES
		(
			@RunningBillId,
			@ProductId,
			@Quantity,
			@Rate,
			@BaseTotal,
			@Instruction,
			@Discountable,
			@SelfDiscount,
			@DiscPercent,
			@DiscAmount,
			@AfterDiscount,
			@CGSTPercent,
			@CGSTAmount,
			@SGSTPercent,
			@SGSTAmount,
			@IGSTPercent,
			@IGSTAmount,
			@Total,
			@Cancelled
		);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[RunningBillDetail]
		SET
			RunningBillId = @RunningBillId,
			ProductId = @ProductId,
			Quantity = @Quantity,
			Rate = @Rate,
			BaseTotal = @BaseTotal,
			Instruction = @Instruction,
			Discountable = @Discountable,
			SelfDiscount = @SelfDiscount,
			DiscPercent = @DiscPercent,
			DiscAmount = @DiscAmount,
			AfterDiscount = @AfterDiscount,
			CGSTPercent = @CGSTPercent,
			CGSTAmount = @CGSTAmount,
			SGSTPercent = @SGSTPercent,
			SGSTAmount = @SGSTAmount,
			IGSTPercent = @IGSTPercent,
			IGSTAmount = @IGSTAmount,
			Total = @Total,
			Cancelled = @Cancelled
		WHERE Id = @Id;
	END
END