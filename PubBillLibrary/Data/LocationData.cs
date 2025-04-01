namespace PubBillLibrary.Data;

public class LocationData
{
	public static async Task InsertLocation(LocationModel locationModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertLocation, locationModel);

	public static async Task UpdateLocation(LocationModel locationModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.UpdateLocation, locationModel);
}
