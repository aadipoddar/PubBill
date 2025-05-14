namespace PubBillLibrary.Data.Kitchen;

public static class KOTData
{
	public static async Task InsertKOTBillDetail(KOTBillDetailModel kOTBillDetail) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertKOTBillDetail, kOTBillDetail);

	public static async Task<List<KOTBillDetailModel>> LoadKOTBillDetailByRunningBillId(int RunningBillId) =>
		await SqlDataAccess.LoadData<KOTBillDetailModel, dynamic>(StoredProcedureNames.LoadKOTBillDetailByRunningBillId, new { RunningBillId });
}
