namespace PubBillLibrary.Data;

public static class RunningBillData
{
	public static async Task<int> InsertRunningBill(RunningBillModel billModel) =>
			(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertRunningBill, billModel)).FirstOrDefault();

	public static async Task InsertRunningBillDetail(RunningBillDetailModel billDetailModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertRunningBillDetail, billDetailModel);

	public static async Task<List<RunningBillDetailModel>> LoadRunningBillDetailByRunningBillId(int RunningBillId) =>
		await SqlDataAccess.LoadData<RunningBillDetailModel, dynamic>(StoredProcedureNames.LoadRunningTabeDetailByRunningBillId, new { RunningBillId });

	public static async Task DeleteRunningBill(int Id) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.DeleteRunningBill, new { Id });

	public static async Task DeleteRunningBillDetail(int RunningBillId) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.DeleteRunningBillDetail, new { RunningBillId });
}
