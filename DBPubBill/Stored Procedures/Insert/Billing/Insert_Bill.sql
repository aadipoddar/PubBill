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
	@Remarks VARCHAR(250),
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
			Remarks
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
			@Remarks
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
			Remarks = @Remarks
		WHERE Id = @Id;
	END

	SELECT @Id AS Id;
END