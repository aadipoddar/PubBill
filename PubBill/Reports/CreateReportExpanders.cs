using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PubBill.Reports;

internal static class CreateReportExpanders
{
	internal static async Task LoadExpandersData(DateTime fromDateTime, DateTime toDateTime, Grid expanderGrid, bool initialLoad = false)
	{
		if (initialLoad) await InitalializeExpanders(expanderGrid);

		foreach (var location in await CommonData.LoadTableDataByStatus<LocationModel>(TableNames.Location))
		{
			var billOverviewModel = await BillReportData.LoadBillDetailsByDateLocationId(fromDateTime, toDateTime, location.Id);

			foreach (var child in expanderGrid.Children)
				if (child is Expander expander)
				{
					if (expander.Header is Grid headerGrid)
						foreach (var headerChild in headerGrid.Children)
							if (headerChild is TextBox headerTextBox)
								switch (headerTextBox.Name)
								{
									case string name when name == $"{location.Name}HeaderAmountTextBox":
										headerTextBox.Text = billOverviewModel.Sum(x => x.FinalAmount).FormatIndianCurrency();
										break;
									case string name when name == $"{location.Name}HeaderBillsTextBox":
										headerTextBox.Text = billOverviewModel.Count.ToString();
										break;
								}

					if (expander.Content is Grid contentGrid)
						foreach (var contentChild in contentGrid.Children)
							if (contentChild is Border innerBorder)
								if (innerBorder.Child is Grid innerGrid)
									foreach (var innerChild in innerGrid.Children)
									{
										if (innerChild is TextBox contentTextBox)
											switch (contentTextBox.Name)
											{
												case string name when name == $"{location.Name}AmountCollectedTextBox":
													contentTextBox.Text = billOverviewModel.Sum(x => (x.Cash ?? 0) + (x.Card ?? 0) + (x.UPI ?? 0)).FormatIndianCurrency();
													break;
												case string name when name == $"{location.Name}CashTextBox":
													contentTextBox.Text = billOverviewModel.Sum(x => x.Cash).FormatIndianCurrency();
													break;
												case string name when name == $"{location.Name}CardTextBox":
													contentTextBox.Text = billOverviewModel.Sum(x => x.Card).FormatIndianCurrency();
													break;
												case string name when name == $"{location.Name}UpiTextBox":
													contentTextBox.Text = billOverviewModel.Sum(x => x.UPI).FormatIndianCurrency();
													break;

												case string name when name == $"{location.Name}ProductsTextBox":
													contentTextBox.Text = billOverviewModel.Sum(x => x.TotalProducts).ToString();
													break;
												case string name when name == $"{location.Name}QuantityTextBox":
													contentTextBox.Text = billOverviewModel.Sum(x => x.TotalQuantity).ToString();
													break;
												case string name when name == $"{location.Name}SGSTTextBox":
													contentTextBox.Text = billOverviewModel.Sum(x => x.SGSTAmount).FormatIndianCurrency();
													break;
												case string name when name == $"{location.Name}CGSTTextBox":
													contentTextBox.Text = billOverviewModel.Sum(x => x.CGSTAmount).FormatIndianCurrency();
													break;
												case string name when name == $"{location.Name}IGSTTextBox":
													contentTextBox.Text = billOverviewModel.Sum(x => x.IGSTAmount).FormatIndianCurrency();
													break;

												case string name when name == $"{location.Name}NoBillsTextBox":
													contentTextBox.Text = billOverviewModel.Count.ToString();
													break;
												case string name when name == $"{location.Name}PeopleTextBox":
													contentTextBox.Text = billOverviewModel.Sum(x => x.TotalPeople).ToString();
													break;
												case string name when name == $"{location.Name}LoyaltyTextBox":
													contentTextBox.Text = billOverviewModel.Sum(x => x.PersonLoyalty.HasValue && x.PersonLoyalty.Value ? 1 : 0).ToString();
													break;
											}

										if (innerChild is Border contentBorder)
											if (contentBorder.Child is Grid innerContentGrid)
												foreach (var innerContentChild in innerContentGrid.Children)
													if (innerContentChild is TextBox innerContentTextBox)
														switch (innerContentTextBox.Name)
														{
															case string name when name == $"{location.Name}SalesTextBox":
																innerContentTextBox.Text = billOverviewModel.Sum(x => x.FinalAmount).FormatIndianCurrency();
																break;
															case string name when name == $"{location.Name}AfterServiceTextBox":
																innerContentTextBox.Text = billOverviewModel.Sum(x => x.AfterService).FormatIndianCurrency();
																break;
															case string name when name == $"{location.Name}AfterTaxTextBox":
																innerContentTextBox.Text = billOverviewModel.Sum(x => x.AfterTax).FormatIndianCurrency();
																break;
															case string name when name == $"{location.Name}BaseTotalTextBox":
																innerContentTextBox.Text = billOverviewModel.Sum(x => x.BaseTotal).FormatIndianCurrency();
																break;
															case string name when name == $"{location.Name}AfterDiscountTextBox":
																innerContentTextBox.Text = billOverviewModel.Sum(x => x.AfterDiscount).FormatIndianCurrency();
																break;

															case string name when name == $"{location.Name}EntryRedeemedTextBox":
																innerContentTextBox.Text = billOverviewModel.Sum(x => x.EntryPaid).FormatIndianCurrency();
																break;
															case string name when name == $"{location.Name}ServiceChargeTextBox":
																innerContentTextBox.Text = billOverviewModel.Sum(x => x.ServiceAmount).FormatIndianCurrency();
																break;
															case string name when name == $"{location.Name}TaxTextBox":
																innerContentTextBox.Text = billOverviewModel.Sum(x => x.TotalTaxAmount).FormatIndianCurrency();
																break;
															case string name when name == $"{location.Name}DiscountTextBox":
																innerContentTextBox.Text = billOverviewModel.Sum(x => x.DiscountAmount).FormatIndianCurrency();
																break;
														}
									}
				}
		}

		var totalBillOverviewModel = await BillReportData.LoadBillDetailsByDateLocationId(fromDateTime, toDateTime, 0);
		string totalText = "Total";

		foreach (var child in expanderGrid.Children)
			if (child is Expander expander)
			{
				if (expander.Header is Grid headerGrid)
					foreach (var headerChild in headerGrid.Children)
						if (headerChild is TextBox headerTextBox)
							switch (headerTextBox.Name)
							{
								case string name when name == $"{totalText}HeaderAmountTextBox":
									headerTextBox.Text = totalBillOverviewModel.Sum(x => x.FinalAmount).FormatIndianCurrency();
									break;
								case string name when name == $"{totalText}HeaderPersonTextBox":
									headerTextBox.Text = totalBillOverviewModel.Count.ToString();
									break;
							}

				if (expander.Content is Grid contentGrid)
					foreach (var contentChild in contentGrid.Children)
						if (contentChild is Border innerBorder)
							if (innerBorder.Child is Grid innerGrid)
								foreach (var innerChild in innerGrid.Children)
								{
									if (innerChild is TextBox contentTextBox)
										switch (contentTextBox.Name)
										{
											case string name when name == $"{totalText}AmountCollectedTextBox":
												contentTextBox.Text = totalBillOverviewModel.Sum(x => (x.Cash ?? 0) + (x.Card ?? 0) + (x.UPI ?? 0)).FormatIndianCurrency();
												break;
											case string name when name == $"{totalText}CashTextBox":
												contentTextBox.Text = totalBillOverviewModel.Sum(x => x.Cash).FormatIndianCurrency();
												break;
											case string name when name == $"{totalText}CardTextBox":
												contentTextBox.Text = totalBillOverviewModel.Sum(x => x.Card).FormatIndianCurrency();
												break;
											case string name when name == $"{totalText}UpiTextBox":
												contentTextBox.Text = totalBillOverviewModel.Sum(x => x.UPI).FormatIndianCurrency();
												break;

											case string name when name == $"{totalText}ProductsTextBox":
												contentTextBox.Text = totalBillOverviewModel.Sum(x => x.TotalProducts).ToString();
												break;
											case string name when name == $"{totalText}QuantityTextBox":
												contentTextBox.Text = totalBillOverviewModel.Sum(x => x.TotalQuantity).ToString();
												break;
											case string name when name == $"{totalText}SGSTTextBox":
												contentTextBox.Text = totalBillOverviewModel.Sum(x => x.SGSTAmount).FormatIndianCurrency();
												break;
											case string name when name == $"{totalText}CGSTTextBox":
												contentTextBox.Text = totalBillOverviewModel.Sum(x => x.CGSTAmount).FormatIndianCurrency();
												break;
											case string name when name == $"{totalText}IGSTTextBox":
												contentTextBox.Text = totalBillOverviewModel.Sum(x => x.IGSTAmount).FormatIndianCurrency();
												break;

											case string name when name == $"{totalText}NoBillsTextBox":
												contentTextBox.Text = totalBillOverviewModel.Count.ToString();
												break;
											case string name when name == $"{totalText}PeopleTextBox":
												contentTextBox.Text = totalBillOverviewModel.Sum(x => x.TotalPeople).ToString();
												break;
											case string name when name == $"{totalText}LoyaltyTextBox":
												contentTextBox.Text = totalBillOverviewModel.Sum(x => x.PersonLoyalty.HasValue && x.PersonLoyalty.Value ? 1 : 0).ToString();
												break;
										}

									if (innerChild is Border contentBorder)
										if (contentBorder.Child is Grid innerContentGrid)
											foreach (var innerContentChild in innerContentGrid.Children)
												if (innerContentChild is TextBox innerContentTextBox)
													switch (innerContentTextBox.Name)
													{
														case string name when name == $"{totalText}SalesTextBox":
															innerContentTextBox.Text = totalBillOverviewModel.Sum(x => x.FinalAmount).FormatIndianCurrency();
															break;
														case string name when name == $"{totalText}AfterServiceTextBox":
															innerContentTextBox.Text = totalBillOverviewModel.Sum(x => x.AfterService).FormatIndianCurrency();
															break;
														case string name when name == $"{totalText}AfterTaxTextBox":
															innerContentTextBox.Text = totalBillOverviewModel.Sum(x => x.AfterTax).FormatIndianCurrency();
															break;
														case string name when name == $"{totalText}BaseTotalTextBox":
															innerContentTextBox.Text = totalBillOverviewModel.Sum(x => x.BaseTotal).FormatIndianCurrency();
															break;
														case string name when name == $"{totalText}AfterDiscountTextBox":
															innerContentTextBox.Text = totalBillOverviewModel.Sum(x => x.AfterDiscount).FormatIndianCurrency();
															break;

														case string name when name == $"{totalText}EntryRedeemedTextBox":
															innerContentTextBox.Text = totalBillOverviewModel.Sum(x => x.EntryPaid).FormatIndianCurrency();
															break;
														case string name when name == $"{totalText}ServiceChargeTextBox":
															innerContentTextBox.Text = totalBillOverviewModel.Sum(x => x.ServiceAmount).FormatIndianCurrency();
															break;
														case string name when name == $"{totalText}TaxTextBox":
															innerContentTextBox.Text = totalBillOverviewModel.Sum(x => x.TotalTaxAmount).FormatIndianCurrency();
															break;
														case string name when name == $"{totalText}DiscountTextBox":
															innerContentTextBox.Text = totalBillOverviewModel.Sum(x => x.DiscountAmount).FormatIndianCurrency();
															break;
													}
								}
			}
	}

	#region CreateExpanders

	private static async Task InitalializeExpanders(Grid expanderGrid)
	{
		expanderGrid.Children.Clear();
		expanderGrid.RowDefinitions.Clear();

		foreach (var location in await CommonData.LoadTableDataByStatus<LocationModel>(TableNames.Location))
		{
			expanderGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			var expander = CreateLocationExpander(location.Name, location.Id);
			Grid.SetRow(expander, expanderGrid.RowDefinitions.Count - 1);
			expanderGrid.Children.Add(expander);
		}

		expanderGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
		expanderGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		var totalExpander = CreateLocationExpander("Total");
		Grid.SetRow(totalExpander, expanderGrid.RowDefinitions.Count - 1);
		expanderGrid.Children.Add(totalExpander);
	}

	private static Expander CreateLocationExpander(string locationName, int locationId = 0)
	{
		// Expander
		var expander = new Expander
		{
			Margin = new Thickness(10),
		};

		#region Header

		// Expander Header
		var headerGrid = new Grid();
		expander.Header = headerGrid;

		// Header Grid Column Definitions
		headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto, MinWidth = 100 });
		headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Header controls
		var locationText = new TextBlock
		{
			Text = locationName == "Total" ? "Grand Total" : $"{locationName}",
			Margin = new Thickness(10),
			Padding = new Thickness(5)
		};
		Grid.SetColumn(locationText, 0);
		headerGrid.Children.Add(locationText);

		var amountText = new TextBlock { Text = "Amount", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetColumn(amountText, 2);
		headerGrid.Children.Add(amountText);

		var headerAmountTextBox = new TextBox
		{
			Name = $"{locationName}HeaderAmountTextBox",
			Margin = new Thickness(0, 10, 10, 10),
			Padding = new Thickness(5),
			Text = "Amount",
			MinWidth = 100,
			TextAlignment = TextAlignment.Right,
			IsReadOnly = true
		};
		Grid.SetColumn(headerAmountTextBox, 3);
		headerGrid.Children.Add(headerAmountTextBox);

		var billsText = new TextBlock { Text = "Bills", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetColumn(billsText, 5);
		headerGrid.Children.Add(billsText);

		var headerBillsTextBox = new TextBox
		{
			Name = $"{locationName}HeaderBillsTextBox",
			Margin = new Thickness(0, 10, 10, 10),
			Padding = new Thickness(5),
			Text = "0",
			MinWidth = 100,
			TextAlignment = TextAlignment.Right,
			IsReadOnly = true
		};
		Grid.SetColumn(headerBillsTextBox, 6);
		headerGrid.Children.Add(headerBillsTextBox);

		var detailedButton = new Button
		{
			Name = locationName == "Total" ? "summaryPrintButton" : "detailedButton",
			Content = locationName == "Total" ? "Print Summary" : "Detailed",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			MinWidth = 100,
		};

		detailedButton.Click += async (s, e) =>
		{
			if (s is not Button button) return;

			if (button.Name == "summaryPrintButton")
			{
				//MemoryStream ms = await PDF.Summary(_fromDateTime, _toDateTime);
				//using FileStream stream = new(Path.Combine(Path.GetTempPath(), "SummaryReport.pdf"), FileMode.Create, FileAccess.Write);
				//await ms.CopyToAsync(stream);
				//Process.Start(new ProcessStartInfo($"{Path.GetTempPath()}\\SummaryReport.pdf") { UseShellExecute = true });
			}
			else
			{
				//DetailedReportWindow detailedReportWindow = new(_fromDateTime, _toDateTime, locationId);
				//detailedReportWindow.Show();
			}
		};

		Grid.SetColumn(detailedButton, 8);
		headerGrid.Children.Add(detailedButton);

		#endregion

		#region Body

		// Expander Content
		var contentGrid = new Grid();
		expander.Content = contentGrid;

		// Content Grid Column Definitions
		contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Amount Section
		var amountSection = CreateAmountSection(locationName);
		Grid.SetColumn(amountSection, 0);
		contentGrid.Children.Add(amountSection);

		// Amount Details Section
		var amountDetailsSection = CreateAmountDetailsSection(locationName);
		Grid.SetColumn(amountDetailsSection, 2);
		contentGrid.Children.Add(amountDetailsSection);

		// Products Section
		var productsSection = CreateProductsSection(locationName);
		Grid.SetColumn(productsSection, 4);
		contentGrid.Children.Add(productsSection);

		// Bill Detail Section
		var billDetailSection = CreateBillDetailSection(locationName);
		Grid.SetColumn(billDetailSection, 6);
		contentGrid.Children.Add(billDetailSection);

		#endregion

		return expander;
	}

	private static Border CreateAmountSection(string locationName)
	{
		var border = new Border
		{
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			BorderBrush = Brushes.Gold,
			BorderThickness = new Thickness(2),
			CornerRadius = new CornerRadius(5)
		};

		var grid = new Grid();
		// Column definitions
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Row definitions
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		// Controls
		var headerText = new TextBlock
		{
			Text = "Amount Collected",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		Grid.SetRow(headerText, 0);
		Grid.SetColumnSpan(headerText, 2);
		Grid.SetColumn(headerText, 0);
		grid.Children.Add(headerText);

		var amountText = new TextBlock { Text = "Total Amount", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(amountText, 1);
		Grid.SetColumn(amountText, 0);
		grid.Children.Add(amountText);

		var amountTextBox = new TextBox
		{
			Name = $"{locationName}AmountCollectedTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(amountTextBox, 1);
		Grid.SetColumn(amountTextBox, 1);
		grid.Children.Add(amountTextBox);


		var cashText = new TextBlock { Text = "Cash", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(cashText, 3);
		Grid.SetColumn(cashText, 0);
		grid.Children.Add(cashText);

		var cashTextBox = new TextBox
		{
			Name = $"{locationName}CashTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(cashTextBox, 3);
		Grid.SetColumn(cashTextBox, 1);
		grid.Children.Add(cashTextBox);


		var cardText = new TextBlock { Text = "Card", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(cardText, 4);
		Grid.SetColumn(cardText, 0);
		grid.Children.Add(cardText);

		var cardTextBox = new TextBox
		{
			Name = $"{locationName}CardTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(cardTextBox, 4);
		Grid.SetColumn(cardTextBox, 1);
		grid.Children.Add(cardTextBox);


		var upiText = new TextBlock { Text = "UPI", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(upiText, 5);
		Grid.SetColumn(upiText, 0);
		grid.Children.Add(upiText);

		var upiTextBox = new TextBox
		{
			Name = $"{locationName}UpiTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(upiTextBox, 5);
		Grid.SetColumn(upiTextBox, 1);
		grid.Children.Add(upiTextBox);

		border.Child = grid;

		return border;
	}

	private static Border CreateAmountDetailsSection(string locationName)
	{
		var border = new Border
		{
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			BorderBrush = Brushes.Gold,
			BorderThickness = new Thickness(2),
			CornerRadius = new CornerRadius(5)
		};

		var grid = new Grid();
		// Column definitions
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Row definitions
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		var headerText = new TextBlock
		{
			Text = "Amount Details",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		Grid.SetRow(headerText, 0);
		Grid.SetColumnSpan(headerText, 2);
		Grid.SetColumn(headerText, 0);
		grid.Children.Add(headerText);

		var salesSection = CreateSalesSection(locationName);
		Grid.SetRow(salesSection, 1);
		Grid.SetColumn(salesSection, 0);
		grid.Children.Add(salesSection);

		var deductionsSection = CreateDeductionsSection(locationName);
		Grid.SetRow(deductionsSection, 1);
		Grid.SetColumn(deductionsSection, 1);
		grid.Children.Add(deductionsSection);

		border.Child = grid;

		return border;
	}

	private static Border CreateSalesSection(string locationName)
	{
		var border = new Border
		{
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			BorderBrush = Brushes.MediumVioletRed,
			BorderThickness = new Thickness(2),
			CornerRadius = new CornerRadius(5)
		};

		var grid = new Grid();
		// Column definitions
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Row definitions
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		// Controls
		var headerText = new TextBlock
		{
			Text = "Sales",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		Grid.SetRow(headerText, 0);
		Grid.SetColumnSpan(headerText, 2);
		Grid.SetColumn(headerText, 0);
		grid.Children.Add(headerText);

		var salesText = new TextBlock { Text = "Sales", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(salesText, 1);
		Grid.SetColumn(salesText, 0);
		grid.Children.Add(salesText);

		var salesTextBox = new TextBox
		{
			Name = $"{locationName}SalesTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(salesTextBox, 1);
		Grid.SetColumn(salesTextBox, 1);
		grid.Children.Add(salesTextBox);


		var afterServiceText = new TextBlock { Text = "After Service", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(afterServiceText, 3);
		Grid.SetColumn(afterServiceText, 0);
		grid.Children.Add(afterServiceText);

		var afterServiceTextBox = new TextBox
		{
			Name = $"{locationName}AfterServiceTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(afterServiceTextBox, 3);
		Grid.SetColumn(afterServiceTextBox, 1);
		grid.Children.Add(afterServiceTextBox);


		var afterTextText = new TextBlock { Text = "After Tax", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(afterTextText, 4);
		Grid.SetColumn(afterTextText, 0);
		grid.Children.Add(afterTextText);

		var afterTaxTextBox = new TextBox
		{
			Name = $"{locationName}AfterTaxTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(afterTaxTextBox, 4);
		Grid.SetColumn(afterTaxTextBox, 1);
		grid.Children.Add(afterTaxTextBox);


		var baseTotalText = new TextBlock { Text = "Base Total", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(baseTotalText, 5);
		Grid.SetColumn(baseTotalText, 0);
		grid.Children.Add(baseTotalText);

		var baseTotalTextBox = new TextBox
		{
			Name = $"{locationName}BaseTotalTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(baseTotalTextBox, 5);
		Grid.SetColumn(baseTotalTextBox, 1);
		grid.Children.Add(baseTotalTextBox);


		var afterDiscountText = new TextBlock { Text = "After Discount", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(afterDiscountText, 6);
		Grid.SetColumn(afterDiscountText, 0);
		grid.Children.Add(afterDiscountText);

		var afterDiscountTextBox = new TextBox
		{
			Name = $"{locationName}AfterDiscountTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(afterDiscountTextBox, 6);
		Grid.SetColumn(afterDiscountTextBox, 1);
		grid.Children.Add(afterDiscountTextBox);

		border.Child = grid;

		return border;
	}

	private static Border CreateDeductionsSection(string locationName)
	{
		var border = new Border
		{
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			BorderBrush = Brushes.MediumVioletRed,
			BorderThickness = new Thickness(2),
			CornerRadius = new CornerRadius(5)
		};

		var grid = new Grid();
		// Column definitions
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Row definitions
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		// Controls
		var headerText = new TextBlock
		{
			Text = "Tax / Deduction",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		Grid.SetRow(headerText, 0);
		Grid.SetColumnSpan(headerText, 2);
		Grid.SetColumn(headerText, 0);
		grid.Children.Add(headerText);

		var entryRedeemedText = new TextBlock { Text = "Entry Redeemed", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(entryRedeemedText, 1);
		Grid.SetColumn(entryRedeemedText, 0);
		grid.Children.Add(entryRedeemedText);

		var entryRedeemedTextBox = new TextBox
		{
			Name = $"{locationName}EntryRedeemedTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(entryRedeemedTextBox, 1);
		Grid.SetColumn(entryRedeemedTextBox, 1);
		grid.Children.Add(entryRedeemedTextBox);


		var serviceChargeText = new TextBlock { Text = "Service Charge", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(serviceChargeText, 3);
		Grid.SetColumn(serviceChargeText, 0);
		grid.Children.Add(serviceChargeText);

		var serviceChargeTextBox = new TextBox
		{
			Name = $"{locationName}ServiceChargeTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(serviceChargeTextBox, 3);
		Grid.SetColumn(serviceChargeTextBox, 1);
		grid.Children.Add(serviceChargeTextBox);


		var taxText = new TextBlock { Text = "Tax", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(taxText, 4);
		Grid.SetColumn(taxText, 0);
		grid.Children.Add(taxText);

		var taxTextBox = new TextBox
		{
			Name = $"{locationName}TaxTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(taxTextBox, 4);
		Grid.SetColumn(taxTextBox, 1);
		grid.Children.Add(taxTextBox);


		var discountText = new TextBlock { Text = "Discount", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(discountText, 5);
		Grid.SetColumn(discountText, 0);
		grid.Children.Add(discountText);

		var discountTextBox = new TextBox
		{
			Name = $"{locationName}DiscountTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(discountTextBox, 5);
		Grid.SetColumn(discountTextBox, 1);
		grid.Children.Add(discountTextBox);

		border.Child = grid;

		return border;
	}

	private static Border CreateProductsSection(string locationName)
	{
		var border = new Border
		{
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			BorderBrush = Brushes.Gold,
			BorderThickness = new Thickness(2),
			CornerRadius = new CornerRadius(5)
		};

		var grid = new Grid();
		// Column definitions
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Row definitions
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		// Controls
		var headerText = new TextBlock
		{
			Text = "Products Details",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		Grid.SetRow(headerText, 0);
		Grid.SetColumnSpan(headerText, 2);
		Grid.SetColumn(headerText, 0);
		grid.Children.Add(headerText);

		var productsText = new TextBlock { Text = "Products", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(productsText, 1);
		Grid.SetColumn(productsText, 0);
		grid.Children.Add(productsText);

		var productsTextBox = new TextBox
		{
			Name = $"{locationName}ProductsTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(productsTextBox, 1);
		Grid.SetColumn(productsTextBox, 1);
		grid.Children.Add(productsTextBox);


		var quantityText = new TextBlock { Text = "Quantity", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(quantityText, 2);
		Grid.SetColumn(quantityText, 0);
		grid.Children.Add(quantityText);

		var quantityTextBox = new TextBox
		{
			Name = $"{locationName}QuantityTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(quantityTextBox, 2);
		Grid.SetColumn(quantityTextBox, 1);
		grid.Children.Add(quantityTextBox);


		var sgstText = new TextBlock { Text = "SGST", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(sgstText, 4);
		Grid.SetColumn(sgstText, 0);
		grid.Children.Add(sgstText);

		var sgstTextBox = new TextBox
		{
			Name = $"{locationName}SGSTTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(sgstTextBox, 4);
		Grid.SetColumn(sgstTextBox, 1);
		grid.Children.Add(sgstTextBox);


		var cgstText = new TextBlock { Text = "CGST", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(cgstText, 5);
		Grid.SetColumn(cgstText, 0);
		grid.Children.Add(cgstText);

		var cgstTextBox = new TextBox
		{
			Name = $"{locationName}CGSTTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(cgstTextBox, 5);
		Grid.SetColumn(cgstTextBox, 1);
		grid.Children.Add(cgstTextBox);

		var igstText = new TextBlock { Text = "IGST", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(igstText, 6);
		Grid.SetColumn(igstText, 0);
		grid.Children.Add(igstText);

		var igstTextBox = new TextBox
		{
			Name = $"{locationName}IGSTTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(igstTextBox, 6);
		Grid.SetColumn(igstTextBox, 1);
		grid.Children.Add(igstTextBox);

		border.Child = grid;

		return border;
	}

	private static Border CreateBillDetailSection(string locationName)
	{
		var border = new Border
		{
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			BorderBrush = Brushes.Gold,
			BorderThickness = new Thickness(2),
			CornerRadius = new CornerRadius(5)
		};

		var grid = new Grid();
		// Column definitions
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Row definitions
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		// Controls
		var headerText = new TextBlock
		{
			Text = "Bill Details",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		Grid.SetRow(headerText, 0);
		Grid.SetColumnSpan(headerText, 2);
		Grid.SetColumn(headerText, 0);
		grid.Children.Add(headerText);

		var noBillsText = new TextBlock { Text = "No. Of Bills", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(noBillsText, 1);
		Grid.SetColumn(noBillsText, 0);
		grid.Children.Add(noBillsText);

		var noBillsTextBox = new TextBox
		{
			Name = $"{locationName}NoBillsTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(noBillsTextBox, 1);
		Grid.SetColumn(noBillsTextBox, 1);
		grid.Children.Add(noBillsTextBox);


		var peopleText = new TextBlock { Text = "People", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(peopleText, 3);
		Grid.SetColumn(peopleText, 0);
		grid.Children.Add(peopleText);

		var peopleTextBox = new TextBox
		{
			Name = $"{locationName}PeopleTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(peopleTextBox, 3);
		Grid.SetColumn(peopleTextBox, 1);
		grid.Children.Add(peopleTextBox);


		var loyaltyText = new TextBlock { Text = "Loyalty Members", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(loyaltyText, 4);
		Grid.SetColumn(loyaltyText, 0);
		grid.Children.Add(loyaltyText);

		var loyaltyTextBox = new TextBox
		{
			Name = $"{locationName}LoyaltyTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(loyaltyTextBox, 4);
		Grid.SetColumn(loyaltyTextBox, 1);
		grid.Children.Add(loyaltyTextBox);

		border.Child = grid;

		return border;
	}

	#endregion
}

