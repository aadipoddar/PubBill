CREATE PROCEDURE [dbo].[Insert_RunningBill]
	@Id INT OUTPUT,
	@UserId INT,
	@LocationId INT,
	@DiningAreaId INT,
	@DiningTableId INT,
	@PersonId INT,
	@TotalPeople INT,
	@AdjAmount MONEY,
	@AdjReason VARCHAR(250),
	@Remarks VARCHAR(250),
	@Total MONEY,
	@PaymentModeId INT,
	@BillStartDateTime DATETIME
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[RunningBill]
		(
			UserId,
			LocationId,
			DiningAreaId,
			DiningTableId,
			PersonId,
			TotalPeople,
			AdjAmount,
			AdjReason,
			Remarks,
			Total,
			PaymentModeId
		) VALUES
		(
			@UserId,
			@LocationId,
			@DiningAreaId,
			@DiningTableId,
			@PersonId,
			@TotalPeople,
			@AdjAmount,
			@AdjReason,
			@Remarks,
			@Total,
			@PaymentModeId
		);
		
		SET @Id = SCOPE_IDENTITY();
	END

	ELSE
	BEGIN
		UPDATE [dbo].[RunningBill]
		SET
			UserId = @UserId,
			LocationId = @LocationId,
			DiningAreaId = @DiningAreaId,
			DiningTableId = @DiningTableId,
			PersonId = @PersonId,
			TotalPeople = @TotalPeople,
			AdjAmount = @AdjAmount,
			AdjReason = @AdjReason,
			Remarks = @Remarks,
			Total = @Total,
			PaymentModeId = @PaymentModeId
		WHERE Id = @Id;
	END

	SELECT @Id AS Id;
END