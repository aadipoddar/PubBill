namespace PubBill.Inventory.Items;

internal static class StockClosing
{
	public static async Task Closing()
	{
		var bills = await GetBills();
		if (bills is null || bills.Count == 0)
			return;

		var itemQuantities = await GetBillItemQuantities(bills);
		var rawMaterialQuantities = await GetRawMaterialQuantities(itemQuantities);
		await InsertStock(rawMaterialQuantities);
	}

	private static async Task<List<BillOverviewModel>> GetBills()
	{
		var lastClosingDateTime = (await StockData.LoadStockLastClosing()).TransactionDT;

		if (lastClosingDateTime == DateTime.MinValue)
			lastClosingDateTime = DateTime.Now.AddYears(-10);

		if (lastClosingDateTime > DateTime.Now || lastClosingDateTime.Date == DateTime.Now.Date)
			return null;

		return await BillReportData.LoadBillDetailsByDateLocationId(lastClosingDateTime, DateTime.Now.AddDays(1), 0);
	}

	private static async Task<List<ItemQantityModel>> GetBillItemQuantities(List<BillOverviewModel> bills)
	{
		List<ItemQantityModel> itemQantities = [];

		foreach (var bill in bills)
		{
			var billDetails = await BillData.LoadBillDetailByBillId(bill.BillId);

			foreach (var billDetail in billDetails)
			{
				var existingItem = itemQantities.FirstOrDefault(x => x.ItemId == billDetail.Id);

				if (existingItem is not null)
					existingItem.Quantity += billDetail.Quantity;

				else
					itemQantities.Add(new()
					{
						ItemId = billDetail.Id,
						Quantity = billDetail.Quantity
					});
			}
		}

		return itemQantities;
	}

	private static async Task<List<ItemQantityModel>> GetRawMaterialQuantities(List<ItemQantityModel> itemQantities)
	{
		List<ItemQantityModel> rawMaterialQuantities = [];

		foreach (var itemQantity in itemQantities)
		{
			var itemRecipe = await RecipeData.LoadRecipeByProduct(itemQantity.ItemId);

			if (itemRecipe is null)
				continue;

			var recipeDetails = await RecipeData.LoadRecipeDetailByRecipe(itemRecipe.Id);

			foreach (var recipe in recipeDetails)
			{
				var existingRawMaterial = rawMaterialQuantities.FirstOrDefault(x => x.ItemId == recipe.RawMaterialId);
				if (existingRawMaterial is not null)
					existingRawMaterial.Quantity += itemQantity.Quantity * recipe.Quantity;

				else
					rawMaterialQuantities.Add(new()
					{
						ItemId = recipe.RawMaterialId,
						Quantity = itemQantity.Quantity * recipe.Quantity
					});
			}
		}

		return rawMaterialQuantities;
	}

	private static async Task InsertStock(List<ItemQantityModel> rawMaterialQuantities)
	{
		foreach (var rawMaterial in rawMaterialQuantities)
			await StockData.InsertStock(new StockModel()
			{
				Id = 0,
				RawMaterialId = rawMaterial.ItemId,
				Quantity = -rawMaterial.Quantity,
				PurchaseId = null,
				Type = StockType.Sale.ToString(),
				TransactionDT = DateTime.Now
			});
	}

	class ItemQantityModel
	{
		public int ItemId { get; set; }
		public decimal Quantity { get; set; }
	}
}