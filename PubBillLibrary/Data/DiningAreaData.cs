namespace PubBillLibrary.Data;

public static class DiningAreaData
{
	public static async Task InsertDiningArea(DiningAreaModel diningArea) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertDiningArea, diningArea);

	public static async Task<List<DiningAreaModel>> LoadDiningAreaByLocation(int LocationId) =>
			await SqlDataAccess.LoadData<DiningAreaModel, dynamic>(StoredProcedureNames.LoadDiningAreaByLocation, new { LocationId });
}
