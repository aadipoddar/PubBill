namespace PubBillLibrary.Data;

public static class RunningBillData
{
	public static async Task<int> InsertRunningBill(RunningBillModel billModel) =>
			(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertRunningBill, billModel)).FirstOrDefault();

	public static async Task InsertRunningBillDetail(RunningBillDetailModel billDetailModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertRunningBillDetail, billDetailModel);
}
