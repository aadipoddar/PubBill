namespace PubBillLibrary.Data.Products;

public static class TaxData
{
	public static async Task InsertTax(TaxModel tax) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertTax, tax);
}
