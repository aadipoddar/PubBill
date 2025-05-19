﻿namespace PubBillLibrary.Models.Inventory.Items;

public class RecipeDetailModel
{
	public int Id { get; set; }
	public int RecipeId { get; set; }
	public int RawMaterialId { get; set; }
	public decimal Quantity { get; set; }
	public bool Status { get; set; }
}

public class RawMaterialRecipeModel
{
	public int RawMaterialId { get; set; }
	public int RawMaterialCategoryId { get; set; }
	public string RawMaterialName { get; set; }
	public decimal Quantity { get; set; }
}