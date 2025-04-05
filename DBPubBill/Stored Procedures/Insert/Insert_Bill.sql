CREATE PROCEDURE [dbo].[Insert_Bill]
	@Id INT OUTPUT,
	@UserId INT,
	@LocationId INT,
	@DiningAreaId INT,
	@DiningTableId INT,
	@PersonId INT,
	@TotalPeople INT,
	@AdjAmount MONEY,
	@AdjReason VARCHAR(250),
	@Remakrs VARCHAR(250),
	@Total MONEY,
	@BillDateTime DATETIME
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Bill]
		(
			UserId,
			LocationId,
			DiningAreaId,
			DiningTableId,
			PersonId,
			TotalPeople,
			AdjAmount,
			AdjReason,
			Remakrs,
			Total
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
			@Remakrs,
			@Total
		);
		
		SET @Id = SCOPE_IDENTITY();
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Bill]
		SET
			UserId = @UserId,
			LocationId = @LocationId,
			DiningAreaId = @DiningAreaId,
			DiningTableId = @DiningTableId,
			PersonId = @PersonId,
			TotalPeople = @TotalPeople,
			AdjAmount = @AdjAmount,
			AdjReason = @AdjReason,
			Remakrs = @Remakrs,
			Total = @Total
		WHERE Id = @Id;
	END
END;