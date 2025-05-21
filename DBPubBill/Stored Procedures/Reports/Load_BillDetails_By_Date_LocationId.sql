CREATE PROCEDURE [dbo].[Load_BillDetails_By_Date_LocationId]
    @FromDate DATETIME,
    @ToDate DATETIME,
    @LocationId INT
AS
BEGIN
    IF @LocationId = 0
    BEGIN
        SELECT *
        FROM dbo.Bill_Overview v
        WHERE BillDateTime BETWEEN @FromDate AND @ToDate;
    END

    ELSE
    BEGIN
        SELECT *
        FROM dbo.Bill_Overview v
        WHERE BillDateTime BETWEEN @FromDate AND @ToDate
          AND LocationId = @LocationId;
    END
END