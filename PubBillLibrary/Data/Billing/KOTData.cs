namespace PubBillLibrary.Data.Billing;

public static class KOTData
{
	public static async Task InsertKOTBillDetail(KOTBillDetailModel billDetailModel) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertKOTBillDetail, billDetailModel);

	public static async Task<List<KOTBillDetailModel>> LoadKOTBillDetailByRunningBillId(int RunningBillId) =>
		await SqlDataAccess.LoadData<KOTBillDetailModel, dynamic>(StoredProcedureNames.LoadKOTBillDetailByRunningBillId, new { RunningBillId });

	public static async Task DeleteKOTBillDetail(int RunningBillId) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.DeleteKOTBillDetail, new { RunningBillId });
}
