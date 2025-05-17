using System.Windows;
using System.Windows.Documents;

namespace PubBill.Billing.KOT;

internal static class ThermalKOTReceipt
{
	#region Get Settings
	private static int PageWidthThermal => (int)Application.Current.Resources[SettingsKeys.PageWidthThermal];
	private static int PagePaddingTopThermal => (int)Application.Current.Resources[SettingsKeys.PagePaddingTopThermal];
	private static int PagePaddingBottomThermal => (int)Application.Current.Resources[SettingsKeys.PagePaddingBottomThermal];
	private static int PagePaddingLeftThermal => (int)Application.Current.Resources[SettingsKeys.PagePaddingLeftThermal];
	private static int PagePaddingRightThermal => (int)Application.Current.Resources[SettingsKeys.PagePaddingRightThermal];
	#endregion

	internal static async Task<FlowDocument> Print(List<KOTBillDetailModel> kotOrders)
	{
		FlowDocument document = new()
		{
			PageWidth = PageWidthThermal,
			PagePadding = new Thickness(PagePaddingLeftThermal, PagePaddingTopThermal, PagePaddingRightThermal, PagePaddingBottomThermal),
			ColumnWidth = double.MaxValue
		};

		await AddHeader(document, kotOrders.FirstOrDefault());
		await AddProductDetails(document, kotOrders);

		return document;
	}

	private static async Task AddHeader(FlowDocument document, KOTBillDetailModel kotOrder)
	{
		var runningBill = await CommonData.LoadTableDataById<RunningBillModel>(TableNames.RunningBill, kotOrder.RunningBillId);

		var location = await CommonData.LoadTableDataById<LocationModel>(TableNames.Location, runningBill.LocationId);
		document.Blocks.Add(ThermalParagraphs.HeaderParagraph($"** {location.Name} **"));

		document.Blocks.Add(ThermalParagraphs.SubHeaderParagraph($"{DateTime.Now:dd/MM/yy HH:mm}"));
		document.Blocks.Add(ThermalParagraphs.SubHeaderParagraph($"KOT Id: {kotOrder.Id}"));

		var diningArea = await CommonData.LoadTableDataById<DiningAreaModel>(TableNames.DiningArea, runningBill.DiningAreaId);
		document.Blocks.Add(ThermalParagraphs.SubHeaderParagraph($"Area: {diningArea.Name}"));

		var diningTable = await CommonData.LoadTableDataById<DiningTableModel>(TableNames.DiningTable, runningBill.DiningTableId);
		document.Blocks.Add(ThermalParagraphs.SubHeaderParagraph($"Table: {diningTable.Name}"));

		var user = await CommonData.LoadTableDataById<UserModel>(TableNames.User, runningBill.UserId);
		document.Blocks.Add(ThermalParagraphs.SubHeaderParagraph($"User: {user.Name}"));

		document.Blocks.Add(ThermalParagraphs.SeparatorParagraph());
	}

	private static async Task AddProductDetails(FlowDocument document, List<KOTBillDetailModel> kotOrders)
	{
		Table itemsTable = new()
		{
			Margin = new Thickness(0, 5, 0, 5)
		};

		itemsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Auto) });
		itemsTable.Columns.Add(new TableColumn { Width = new GridLength(22) });

		TableRowGroup itemsGroup = new();

		TableRow headerRow = new();
		headerRow.Cells.Add(new TableCell(ThermalParagraphs.TableHeaderParagraph("Name", TextAlignment.Left)));
		headerRow.Cells.Add(new TableCell(ThermalParagraphs.TableHeaderParagraph("Qty", TextAlignment.Right)));
		itemsGroup.Rows.Add(headerRow);

		foreach (var order in kotOrders)
		{
			var product = await CommonData.LoadTableDataById<ProductModel>(TableNames.Product, order.ProductId);

			ThermalParagraphs.AddTableRow(itemsGroup, product.Name, order.Quantity.ToString());
		}

		itemsTable.RowGroups.Add(itemsGroup);
		document.Blocks.Add(itemsTable);

		foreach (var order in kotOrders)
			if (!string.IsNullOrEmpty(order.Instruction))
				document.Blocks.Add(ThermalParagraphs.RegularParagraph($"Instruction: {order.Instruction}"));

		document.Blocks.Add(ThermalParagraphs.SeparatorParagraph());
	}
}
