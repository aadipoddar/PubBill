CREATE PROCEDURE [dbo].[Insert_Bill]
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
	@EntryPaid INT,
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
			DiscPercent,
			DiscReason,
			ServicePercent,
			EntryPaid,
			Remarks
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
			@EntryPaid,
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
			DiscPercent = @DiscPercent,
			DiscReason = @DiscReason,
			ServicePercent = @ServicePercent,
			EntryPaid = @EntryPaid,
			Remarks = @Remarks
		WHERE Id = @Id;
	END

	SELECT @Id AS Id;
END