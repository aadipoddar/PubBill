﻿using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

using MessagingToolkit.QRCode.Codec;

using NumericWordsConversion;

namespace PubBill.Billing.Bill;

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

	internal static async Task<FlowDocument> Print(RunningBillModel runningBillModel, int entryPaid)
	{
		FlowDocument document = new()
		{
			PageWidth = PageWidthThermal,
			PagePadding = new Thickness(PagePaddingLeftThermal, PagePaddingTopThermal, PagePaddingRightThermal, PagePaddingBottomThermal),
			ColumnWidth = double.MaxValue
		};

		await AddHeader(document, runningBillModel.LocationId);

		if (runningBillModel.PersonId is not null)
			await AddPersonInformation(document, runningBillModel.PersonId ?? 0);

		await AddBillDetails(document, runningBillModel);
		await AddItemDetails(document, runningBillModel);
		await AddTotalDetails(document, runningBillModel, entryPaid);

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

	private static async Task AddBillDetails(FlowDocument document, RunningBillModel runningBillModel)
	{
		var user = await CommonData.LoadTableDataById<UserModel>(TableNames.User, runningBillModel.UserId);
		var diningTable = await CommonData.LoadTableDataById<DiningTableModel>(TableNames.DiningTable, runningBillModel.DiningTableId);

		document.Blocks.Add(ThermalParagraphs.RegularParagraph($"Bill No: {runningBillModel.Id}"));
		document.Blocks.Add(ThermalParagraphs.RegularParagraph($"Date: {DateTime.Now:dd/MM/yy HH:mm}"));
		document.Blocks.Add(ThermalParagraphs.RegularParagraph($"User: {user.Name}"));
		document.Blocks.Add(ThermalParagraphs.RegularParagraph($"Table: {diningTable.Name}"));

		document.Blocks.Add(ThermalParagraphs.SeparatorParagraph());
	}

	private static async Task AddItemDetails(FlowDocument document, RunningBillModel runningBillModel)
	{
		var runningBillItems = await RunningBillData.LoadRunningBillDetailByRunningBillId(runningBillModel.Id);

		Table itemsTable = new()
		{
			Margin = new Thickness(0, 5, 0, 5)
		};

		itemsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Auto) });
		itemsTable.Columns.Add(new TableColumn { Width = new GridLength(22) });
		itemsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Auto) });
		itemsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Auto) });

		TableRowGroup itemsGroup = new();

		TableRow headerRow = new();
		headerRow.Cells.Add(new TableCell(ThermalParagraphs.TableHeaderParagraph("Item", TextAlignment.Left)));
		headerRow.Cells.Add(new TableCell(ThermalParagraphs.TableHeaderParagraph("Qty")));
		headerRow.Cells.Add(new TableCell(ThermalParagraphs.TableHeaderParagraph("Rate", TextAlignment.Right)));
		headerRow.Cells.Add(new TableCell(ThermalParagraphs.TableHeaderParagraph("Amt", TextAlignment.Right)));
		itemsGroup.Rows.Add(headerRow);

		foreach (var item in runningBillItems)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductModel>(TableNames.Product, item.ProductId);
			ThermalParagraphs.AddTableRow(itemsGroup, product.Name, item.Quantity, (int)product.Rate, (int)item.BaseTotal);
		}

		itemsTable.RowGroups.Add(itemsGroup);
		document.Blocks.Add(itemsTable);

		document.Blocks.Add(ThermalParagraphs.SeparatorParagraph());
	}

	private static async Task AddTotalDetails(FlowDocument document, RunningBillModel runningBillModel, int entryPaid)
	{
		var runningBillItems = await RunningBillData.LoadRunningBillDetailByRunningBillId(runningBillModel.Id);

		document.Blocks.Add(ThermalParagraphs.RegularParagraph($"Total Qty: {runningBillItems.Where(x => !x.Cancelled).Sum(x => x.Quantity)}"));

		Table subTotalsTable = new()
		{
			Margin = new Thickness(0, 5, 0, 5)
		};

		subTotalsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
		subTotalsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

		TableRowGroup itemsGroup = new();
		ThermalParagraphs.AddTableRow(itemsGroup, "Sub Total:", BillWindowHelper.CalculateBaseTotal(runningBillItems).FormatIndianCurrency());

		if (BillWindowHelper.CalculateDiscountAmount(runningBillItems) > 0)
			ThermalParagraphs.AddTableRow(itemsGroup, "Discount:", BillWindowHelper.GetDiscountString(runningBillItems));

		if (BillWindowHelper.CalculateProductSGST(runningBillItems) > 0)
			ThermalParagraphs.AddTableRow(itemsGroup, "SGST:", BillWindowHelper.GetSGSTString(runningBillItems));

		if (BillWindowHelper.CalculateProductCGST(runningBillItems) > 0)
			ThermalParagraphs.AddTableRow(itemsGroup, "CGST:", BillWindowHelper.GetCGSTString(runningBillItems));

		if (BillWindowHelper.CalculateProductIGST(runningBillItems) > 0)
			ThermalParagraphs.AddTableRow(itemsGroup, "IGST:", BillWindowHelper.GetIGSTString(runningBillItems));

		if (runningBillModel.ServicePercent > 0)
			ThermalParagraphs.AddTableRow(itemsGroup, "Service Charge:", BillWindowHelper.GetServiceString(runningBillItems, runningBillModel.ServicePercent));

		if (entryPaid > 0)
			ThermalParagraphs.AddTableRow(itemsGroup, "Entry Paid:", ((decimal)entryPaid).FormatIndianCurrency());

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

		var total = BillWindowHelper.CalculateBillTotal(runningBillItems, runningBillModel.ServicePercent, entryPaid);
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

		AddQRCode(document, total);
	}

	private static void AddQRCode(FlowDocument document, decimal total)
	{
		document.Blocks.Add(ThermalParagraphs.RegularParagraph("Scan to Pay", TextAlignment.Center));

		// Generate the UPI ID
		string upiId = $"{UpiId}&am={total}&cu=INR";

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
		document.Blocks.Add(ThermalParagraphs.FooterParagraph($"Printed DT: {DateTime.Now:dd/MM/yy HH:mm}", true));
		document.Blocks.Add(ThermalParagraphs.FooterParagraph(FooterLine, true));
	}
}
