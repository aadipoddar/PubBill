namespace PubBillLibrary.Data;

public static class LocationData
{
	public static async Task InsertLocation(LocationModel locationModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertLocation, locationModel);
}
