namespace PubBillLibrary.Data;

public static class BillData
{
	public static async Task<int> InsertBill(CartModel billModel) =>
			(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertBill, billModel)).FirstOrDefault();

	public static async Task InsertBillDetail(BillDetailModel billDetailModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertBillDetail, billDetailModel);
}
