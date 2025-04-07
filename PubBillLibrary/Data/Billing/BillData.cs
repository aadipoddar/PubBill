namespace PubBillLibrary.Data.Billing;

public static class BillData
{
	public static async Task<int> InsertBill(BillModel billModel) =>
			(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertBill, billModel)).FirstOrDefault();

	public static async Task InsertBillDetail(BillDetailModel billDetailModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertBillDetail, billDetailModel);
}
