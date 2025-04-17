using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

using MessagingToolkit.QRCode.Codec;

using NumericWordsConversion;

namespace PubBill.Billing.Bill.Printing;

internal static class ThermalBillReceipt
{
	#region Get Settings
	private static int PageWidthThermal => (int)Application.Current.Resources[SettingsKeys.PageWidthThermal];
	private static int PagePaddingTopThermal => (int)Application.Current.Resources[SettingsKeys.PagePaddingTopThermal];
	private static int PagePaddingBottomThermal => (int)Application.Current.Resources[SettingsKeys.PagePaddingBottomThermal];
	private static int PagePaddingLeftThermal => (int)Application.Current.Resources[SettingsKeys.PagePaddingLeftThermal];
	private static int PagePaddingRightThermal => (int)Application.Current.Resources[SettingsKeys.PagePaddingRightThermal];
	private static string HeaderLine1 => Application.Current.Resources[SettingsKeys.HeaderLine1].ToString();
	private static string HeaderLine2 => Application.Current.Resources[SettingsKeys.HeaderLine2].ToString();
	private static string HeaderLine3 => Application.Current.Resources[SettingsKeys.HeaderLine3].ToString();
	private static string FooterLine => Application.Current.Resources[SettingsKeys.FooterLine].ToString();
	private static string UpiId => Application.Current.Resources[SettingsKeys.UPIId].ToString();
	#endregion

	internal static async Task<FlowDocument> Print(BillModel billModel, decimal amount)
	{
		FlowDocument document = new()
		{
			PageWidth = PageWidthThermal,
			PagePadding = new Thickness(PagePaddingLeftThermal, PagePaddingTopThermal, PagePaddingRightThermal, PagePaddingBottomThermal),
			ColumnWidth = double.MaxValue
		};

		await AddHeader(document, billModel.LocationId);

		if (billModel.PersonId is not null)
			await AddPersonInformation(document, billModel.PersonId ?? 0);

		await AddBillDetails(document, billModel);
		await AddItemDetails(document, billModel);
		await AddTotalDetails(document, billModel);

		AddQRCode(amount, document);
		AddFooterDetails(document);

		return document;
	}

	private static async Task AddHeader(FlowDocument document, int locationId)
	{
		var location = await CommonData.LoadTableDataById<LocationModel>(TableNames.Location, locationId);

		document.Blocks.Add(ThermalParagraphs.HeaderParagraph($"** {location.Name} **"));

		document.Blocks.Add(ThermalParagraphs.SubHeaderParagraph($"{HeaderLine1}"));
		document.Blocks.Add(ThermalParagraphs.SubHeaderParagraph($"{HeaderLine2}"));
		document.Blocks.Add(ThermalParagraphs.SubHeaderParagraph($"{HeaderLine3}"));

		document.Blocks.Add(ThermalParagraphs.SeparatorParagraph());
	}

	private static async Task AddPersonInformation(FlowDocument document, int personId)
	{
		var person = await CommonData.LoadTableDataById<PersonModel>(TableNames.Person, personId);

		document.Blocks.Add(ThermalParagraphs.RegularParagraph($"Name: {person.Name}"));
		document.Blocks.Add(ThermalParagraphs.RegularParagraph($"Number: {person.Number}"));

		if (person.Loyalty)
			document.Blocks.Add(ThermalParagraphs.RegularParagraph("Loyalty Member"));

		document.Blocks.Add(ThermalParagraphs.SeparatorParagraph());
	}

	private static async Task AddBillDetails(FlowDocument document, BillModel billModel)
	{
		var user = await CommonData.LoadTableDataById<UserModel>(TableNames.User, billModel.UserId);
		var diningTable = await CommonData.LoadTableDataById<DiningTableModel>(TableNames.DiningTable, billModel.DiningTableId);

		document.Blocks.Add(ThermalParagraphs.RegularParagraph($"Bill No: {billModel.Id}"));
		document.Blocks.Add(ThermalParagraphs.RegularParagraph($"Date: {billModel.BillDateTime:dd/MM/yy HH:mm}"));
		document.Blocks.Add(ThermalParagraphs.RegularParagraph($"User: {user.Name}"));
		document.Blocks.Add(ThermalParagraphs.RegularParagraph($"Table: {diningTable.Name}"));

		document.Blocks.Add(ThermalParagraphs.SeparatorParagraph());
	}

	private static async Task AddItemDetails(FlowDocument document, BillModel billModel)
	{
		var billItems = await BillData.LoadBillDetailByBillId(billModel.Id);

		Table itemsTable = new()
		{
			Margin = new Thickness(0, 5, 0, 5)
		};

		itemsTable.Columns.Add(new TableColumn { Width = new GridLength(22) });
		itemsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Auto) });
		itemsTable.Columns.Add(new TableColumn { Width = new GridLength(22) });
		itemsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Auto) });
		itemsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Auto) });

		TableRowGroup itemsGroup = new();

		TableRow headerRow = new();
		headerRow.Cells.Add(new TableCell(ThermalParagraphs.TableHeaderParagraph("No.")));
		headerRow.Cells.Add(new TableCell(ThermalParagraphs.TableHeaderParagraph("Item")));
		headerRow.Cells.Add(new TableCell(ThermalParagraphs.TableHeaderParagraph("Qty")));
		headerRow.Cells.Add(new TableCell(ThermalParagraphs.TableHeaderParagraph("Rate")));
		headerRow.Cells.Add(new TableCell(ThermalParagraphs.TableHeaderParagraph("Amt")));
		itemsGroup.Rows.Add(headerRow);

		int i = 1;
		foreach (var item in billItems)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductModel>(TableNames.Product, item.ProductId);
			ThermalParagraphs.AddTableRow(itemsGroup, i, product.Name, item.Quantity, product.Rate, item.Quantity * product.Rate);
			i++;
		}

		itemsTable.RowGroups.Add(itemsGroup);
		document.Blocks.Add(itemsTable);

		document.Blocks.Add(ThermalParagraphs.SeparatorParagraph());
	}

	private static async Task AddTotalDetails(FlowDocument document, BillModel billModel)
	{
		var billItems = await BillData.LoadBillDetailByBillId(billModel.Id);

		Table subTotalsTable = new()
		{
			Margin = new Thickness(0, 5, 0, 5)
		};

		subTotalsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
		subTotalsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

		TableRowGroup itemsGroup = new();

		ThermalParagraphs.AddTableRow(itemsGroup, "Total Qty:", billItems.Where(x => !x.Cancelled).Sum(x => x.Quantity).ToString());
		ThermalParagraphs.AddTableRow(itemsGroup, "Sub Total:", billItems.Where(x => !x.Cancelled).Sum(x => x.Quantity * x.Rate).FormatIndianCurrency());

		if (billModel.DiscPercent > 0)
			ThermalParagraphs.AddTableRow(itemsGroup, "Discount:", BillWindowHelper.GetDiscountString(billModel, billItems));

		if (await BillWindowHelper.GetSGSTString(billItems) != "")
			ThermalParagraphs.AddTableRow(itemsGroup, "SGST:", await BillWindowHelper.GetSGSTString(billItems));

		if (await BillWindowHelper.GetCGSTString(billItems) != "")
			ThermalParagraphs.AddTableRow(itemsGroup, "CGST:", await BillWindowHelper.GetCGSTString(billItems));

		if (await BillWindowHelper.GetIGSTString(billItems) != "")
			ThermalParagraphs.AddTableRow(itemsGroup, "IGST:", await BillWindowHelper.GetIGSTString(billItems));

		if (billModel.ServicePercent > 0)
			ThermalParagraphs.AddTableRow(itemsGroup, "Service Charge:", await BillWindowHelper.GetServiceString(billModel, billItems));

		if (billModel.EntryPaid > 0)
			ThermalParagraphs.AddTableRow(itemsGroup, "Service Charge:", ((decimal)billModel.EntryPaid).FormatIndianCurrency());

		subTotalsTable.RowGroups.Add(itemsGroup);
		document.Blocks.Add(subTotalsTable);

		document.Blocks.Add(ThermalParagraphs.SeparatorParagraph());



		Table totalsTable = new()
		{
			Margin = new Thickness(0, 5, 0, 5)
		};

		totalsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
		totalsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

		TableRowGroup itemsGroup1 = new();

		var total = await BillWindowHelper.CalculateBillTotal(billModel, billItems);

		ThermalParagraphs.AddTableRow(itemsGroup1, "Total:", total.FormatIndianCurrency());

		totalsTable.RowGroups.Add(itemsGroup1);
		document.Blocks.Add(totalsTable);


		CurrencyWordsConverter numericWords = new(new()
		{
			Culture = Culture.Hindi,
			OutputFormat = OutputFormat.English
		});
		string words = numericWords.ToWords(Math.Round(total));
		document.Blocks.Add(ThermalParagraphs.FooterParagraph($"{(words == "" ? "Zero " : words)}Rupees Only", true));

		document.Blocks.Add(ThermalParagraphs.SeparatorParagraph());
	}

	private static void AddQRCode(decimal amount, FlowDocument document)
	{
		document.Blocks.Add(ThermalParagraphs.RegularParagraph("Scan to Pay"));

		// Generate the UPI ID
		string upiId = amount == 0
			? $"{UpiId}"
			: $"{UpiId}&am={amount}&cu=INR";

		// Generate the QR code
		QRCodeEncoder encoder = new();
		Bitmap bitmap = encoder.Encode(upiId);

		// Convert the Bitmap to a BitmapImage for WPF
		BitmapImage qrCodeImage = ConvertBitmapToBitmapImage(bitmap);

		// Add the QR code to the FlowDocument
		var image = new System.Windows.Controls.Image
		{
			Source = qrCodeImage,
			Width = 200,
			Height = 200,
			Margin = new Thickness(0, 10, 0, 10)
		};

		var block = new BlockUIContainer(image);
		document.Blocks.Add(block);

		document.Blocks.Add(ThermalParagraphs.SeparatorParagraph());
	}

	private static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
	{
		using MemoryStream memory = new();
		bitmap.Save(memory, ImageFormat.Png);
		memory.Position = 0;

		BitmapImage bitmapImage = new();
		bitmapImage.BeginInit();
		bitmapImage.StreamSource = memory;
		bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
		bitmapImage.EndInit();
		bitmapImage.Freeze();
		return bitmapImage;
	}

	private static void AddFooterDetails(FlowDocument document)
	{
		document.Blocks.Add(ThermalParagraphs.FooterParagraph($"Printed DT: {DateTime.Now:dd/MM/yy HH:mm}"));
		document.Blocks.Add(ThermalParagraphs.FooterParagraph(FooterLine, true));
	}
}
