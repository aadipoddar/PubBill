namespace PubBillLibrary.Data.Inventory.Purchase;

public static class PurchaseData
{
	public static async Task<int> InsertPurchase(PurchaseModel purchase) =>
			(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertPurchase, purchase)).FirstOrDefault();

	public static async Task InsertPurchaseDetail(PurchaseDetailModel purchaseDetail) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertPurchaseDetail, purchaseDetail);

	public static async Task<List<PurchaseOverviewModel>> LoadPurchaseOverviewByDate(DateOnly BillDate) =>
		await SqlDataAccess.LoadData<PurchaseOverviewModel, dynamic>(StoredProcedureNames.LoadPurchaseOverviewByDate, new { BillDate });

	public static async Task<List<PurchaseDetailModel>> LoadPurchaseDetailByPurchase(int PurchaseId) =>
		await SqlDataAccess.LoadData<PurchaseDetailModel, dynamic>(StoredProcedureNames.LoadPurchaseDetailByPurchase, new { PurchaseId });
}