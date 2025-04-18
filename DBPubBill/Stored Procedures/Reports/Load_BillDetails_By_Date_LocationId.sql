CREATE PROCEDURE [dbo].[Load_BillDetails_By_Date_LocationId]
    @FromDate DATE,
    @ToDate DATE,
    @LocationId INT
AS
BEGIN
    SELECT *
    FROM dbo.Bill_Overview v
    WHERE BillDateTime BETWEEN @FromDate AND @ToDate
        AND LocationId = @LocationId;
END