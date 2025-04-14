namespace PubBillLibrary.Data;

public static class PubEntryData
{
	public static async Task<PubEntryTransactionModel> LaodTransactionByLocationPerson(int LocationId, string PersonNumber) =>
			(await SqlDataAccess.PubEntryLoadData<PubEntryTransactionModel, dynamic>(PubEntryStoredProcedures.LaodTransactionByLocationPerson, new { LocationId, PersonNumber })).FirstOrDefault();

	public static async Task<PubEntryAdvanceModel> LoadAdvanceByTransactionId(int TransactionId) =>
			(await SqlDataAccess.PubEntryLoadData<PubEntryAdvanceModel, dynamic>(PubEntryStoredProcedures.LoadAdvanceByTransactionId, new { TransactionId })).FirstOrDefault();

	public static async Task<List<PubEntryAdvanceDetailModel>> LoadAdvanceDetailByAdvanceId(int AdvanceId) =>
			await SqlDataAccess.PubEntryLoadData<PubEntryAdvanceDetailModel, dynamic>(PubEntryStoredProcedures.LoadAdvanceDetailByAdvanceId, new { AdvanceId });
}
