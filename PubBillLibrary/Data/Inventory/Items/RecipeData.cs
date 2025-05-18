namespace PubBillLibrary.Data.Inventory.Items;

public static class RecipeData
{
	public static async Task<int> InsertRecipe(RecipeModel recipeModel) =>
		(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertRecipe, recipeModel)).FirstOrDefault();

	public static async Task InsertRecipeDetail(RecipeDetailModel recipeDetailModel) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertRecipeDetail, recipeDetailModel);
}
