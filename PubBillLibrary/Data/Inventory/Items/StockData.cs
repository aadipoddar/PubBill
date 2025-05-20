namespace PubBillLibrary.Data.Inventory.Items;

public static class StockData
{
	public static async Task InsertStock(StockModel stock) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertStock, stock);

	public static async Task<List<StockModel>> LoadStockByPurchase(int PurchaseId) =>
		await SqlDataAccess.LoadData<StockModel, dynamic>(StoredProcedureNames.LoadStockByPurchase, new { PurchaseId });
}
