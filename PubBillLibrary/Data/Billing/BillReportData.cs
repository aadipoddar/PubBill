namespace PubBillLibrary.Data.Billing;

public static class BillReportData
{
	public static async Task<List<BillOverviewModel>> LoadBillDetailsByDateLocationId(DateTime FromDate, DateTime ToDate, int LocationId) =>
		await SqlDataAccess.LoadData<BillOverviewModel, dynamic>(StoredProcedureNames.LoadBillDetailsByDateLocationId, new { FromDate, ToDate, LocationId });
}
