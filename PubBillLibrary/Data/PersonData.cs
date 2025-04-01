namespace PubBillLibrary.Data;

public class PersonData
{
	public static async Task<int> InsertPerson(PersonModel personModel) =>
			(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertPerson, personModel)).FirstOrDefault();

	public static async Task UpdatePerson(PersonModel personModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.UpdatePerson, personModel);

	public static async Task<PersonModel> LoadPersonByNumber(string Number) =>
			(await SqlDataAccess.LoadData<PersonModel, dynamic>(StoredProcedureNames.LoadPersonByNumber, new { Number })).FirstOrDefault();
}