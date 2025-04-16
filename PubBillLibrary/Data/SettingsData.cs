namespace PubBillLibrary.Data;

public static class SettingsData
{
	public static async Task<string> LoadSettingsByKey(string Key) =>
			(await SqlDataAccess.LoadData<string, dynamic>(StoredProcedureNames.LoadSettingsByKey, new { Key })).FirstOrDefault();

	public static async Task UpdateSettings(SettingsModel settingsModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.UpdateSettings, settingsModel);

	public static async Task ResetSettings() =>
			await SqlDataAccess.ExecuteProcedure(StoredProcedureNames.ResetSettings);
}
