namespace PubBillLibrary.Data.Billing;

public static class BillData
{
	public static async Task<int> InsertBill(BillModel bill) =>
			(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertBill, bill)).FirstOrDefault();

	public static async Task InsertBillDetail(BillDetailModel billDetail) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertBillDetail, billDetail);

	public static async Task InsertBillPaymentDetail(BillPaymentDetailModel billPaymentDetail) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertBillPaymentDetail, billPaymentDetail);

	public static async Task<List<BillDetailModel>> LoadBillDetailByBillId(int BillId) =>
		await SqlDataAccess.LoadData<BillDetailModel, dynamic>(StoredProcedureNames.LoadBillDetailByBillId, new { BillId });

	public static async Task<List<BillPaymentDetailModel>> LoadBillPaymentDetailByBillId(int BillId) =>
		await SqlDataAccess.LoadData<BillPaymentDetailModel, dynamic>(StoredProcedureNames.LoadBillPaymentDetailByBillId, new { BillId });
}
