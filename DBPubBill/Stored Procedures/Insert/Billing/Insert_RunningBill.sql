CREATE PROCEDURE [dbo].[Insert_RunningBill]
	@Id INT OUTPUT,
	@UserId INT,
	@LocationId INT,
	@DiningAreaId INT,
	@DiningTableId INT,
	@PersonId INT NULL,
	@TotalPeople INT,
	@DiscPercent DECIMAL(5,2),
	@DiscReason VARCHAR(250),
	@ServicePercent DECIMAL(5,2),
	@Remarks VARCHAR(250),
	@BillStartDateTime DATETIME,
	@BillId INT NULL,
	@Status BIT
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
			DiscPercent,
			DiscReason,
			ServicePercent,
			Remarks,
			BillId,
			[Status]
		) VALUES
		(
			@UserId,
			@LocationId,
			@DiningAreaId,
			@DiningTableId,
			@PersonId,
			@TotalPeople,
			@DiscPercent,
			@DiscReason,
			@ServicePercent,
			@Remarks,
			@BillId,
			@Status
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
			DiscPercent = @DiscPercent,
			DiscReason = @DiscReason,
			ServicePercent = @ServicePercent,
			Remarks = @Remarks,
			BillId = @BillId,
			[Status] = @Status
		WHERE Id = @Id;
	END

	SELECT @Id AS Id;
END