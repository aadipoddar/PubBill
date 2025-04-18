using System.Windows;
using System.Windows.Controls;

namespace PubBill.Reports;

internal static class CreateReportExpanders
{
	private static DateTime _fromDateTime;
	private static DateTime _toDateTime;

	internal static async Task LoadExpandersData(DateTime fromDateTime, DateTime toDateTime, Grid expanderGrid, bool initialLoad = false)
	{
		_fromDateTime = fromDateTime;
		_toDateTime = toDateTime;

		if (initialLoad) await InitalializeExpanders(expanderGrid);

		foreach (var location in await CommonData.LoadTableDataByStatus<LocationModel>(TableNames.Location))
		{
			var billOverviewModels = await BillReportData.LoadBillDetailsByDateLocationId(fromDateTime, toDateTime, location.Id);

			foreach (var child in expanderGrid.Children)
				if (child is Expander expander)
				{
					if (expander.Header is Grid headerGrid)
						foreach (var headerChild in headerGrid.Children)
							if (headerChild is TextBox headerTextBox)
								switch (headerTextBox.Name)
								{
									case string name when name == $"{location.Name}headerAmountTextBox":
										headerTextBox.Text = billOverviewModels.Sum(x => x.FinalAmount).FormatIndianCurrency();
										break;
									case string name when name == $"{location.Name}headerPersonTextBox":
										headerTextBox.Text = billOverviewModels.Count.ToString();
										break;
								}

					//if (expander.Content is Grid contentGrid)
					//	foreach (var contentChild in contentGrid.Children)
					//		if (contentChild is Grid innerGrid)
					//			foreach (var innerChild in innerGrid.Children)
					//				if (innerChild is TextBox contentTextBox)
					//					switch (contentTextBox.Name)
					//					{
					//						case string name when name == $"{location.Name}TotalPeopleTextBox":
					//							contentTextBox.Text = (transactionTotal.Male + transactionTotal.Female).ToString();
					//							break;
					//						case string name when name == $"{location.Name}maleTextBox":
					//							contentTextBox.Text = transactionTotal.Male.ToString();
					//							break;
					//						case string name when name == $"{location.Name}femaleTextBox":
					//							contentTextBox.Text = transactionTotal.Female.ToString();
					//							break;
					//						case string name when name == $"{location.Name}loyaltyTextBox":
					//							contentTextBox.Text = transactionTotal.Loyalty.ToString();
					//							break;

					//						case string name when name == $"{location.Name}amountTextBox":
					//							contentTextBox.Text = (transactionTotal.Cash + transactionTotal.Card + transactionTotal.UPI + transactionTotal.Amex).ToString();
					//							break;
					//						case string name when name == $"{location.Name}cashTextBox":
					//							contentTextBox.Text = transactionTotal.Cash.ToString();
					//							break;
					//						case string name when name == $"{location.Name}cardTextBox":
					//							contentTextBox.Text = transactionTotal.Card.ToString();
					//							break;
					//						case string name when name == $"{location.Name}upiTextBox":
					//							contentTextBox.Text = transactionTotal.UPI.ToString();
					//							break;
					//						case string name when name == $"{location.Name}amexTextBox":
					//							contentTextBox.Text = transactionTotal.Amex.ToString();
					//							break;

					//						case string name when name == $"{location.Name}advanceTextBox":
					//							contentTextBox.Text = advanceTotal.TotalAdvance.ToString();
					//							break;
					//						case string name when name == $"{location.Name}redeemedAdvanceTextBox":
					//							contentTextBox.Text = advanceTotal.RedeemedAdvance.ToString();
					//							break;
					//						case string name when name == $"{location.Name}notRedeemedAdvanceTextBox":
					//							contentTextBox.Text = advanceTotal.NotRedeemedAdvance.ToString();
					//							break;

					//						case string name when name == $"{location.Name}bookingTextBox":
					//							contentTextBox.Text = advanceTotal.TotalBooking.ToString();
					//							break;
					//						case string name when name == $"{location.Name}redeemedBookingTextBox":
					//							contentTextBox.Text = advanceTotal.RedeemedBooking.ToString();
					//							break;
					//						case string name when name == $"{location.Name}notRedeemedBookingTextBox":
					//							contentTextBox.Text = advanceTotal.NotRedeemedBooking.ToString();
					//							break;
					//					}
				}
		}

		string totalText = "Total";

		//foreach (var child in expanderGrid.Children)
		//	if (child is Expander expander)
		//	{
		//		if (expander.Header is Grid headerGrid)
		//			foreach (var headerChild in headerGrid.Children)
		//				if (headerChild is TextBox headerTextBox)
		//					switch (headerTextBox.Name)
		//					{
		//						case string name when name == $"{totalText}headerAmountTextBox":
		//							headerTextBox.Text = transactionTotalsModel.Sum(x => x.Cash + x.Card + x.UPI + x.Amex).ToString();
		//							break;
		//						case string name when name == $"{totalText}headerPersonTextBox":
		//							headerTextBox.Text = transactionTotalsModel.Sum(x => x.Male + x.Female).ToString();
		//							break;
		//					}

		//		if (expander.Content is Grid contentGrid)
		//			foreach (var contentChild in contentGrid.Children)
		//				if (contentChild is Grid innerGrid)
		//					foreach (var innerChild in innerGrid.Children)
		//						if (innerChild is TextBox contentTextBox)
		//							switch (contentTextBox.Name)
		//							{
		//								case string name when name == $"{totalText}TotalPeopleTextBox":
		//									contentTextBox.Text = transactionTotalsModel.Sum(x => x.Male + x.Female).ToString();
		//									break;
		//								case string name when name == $"{totalText}maleTextBox":
		//									contentTextBox.Text = transactionTotalsModel.Sum(x => x.Male).ToString();
		//									break;
		//								case string name when name == $"{totalText}femaleTextBox":
		//									contentTextBox.Text = transactionTotalsModel.Sum(x => x.Female).ToString();
		//									break;
		//								case string name when name == $"{totalText}loyaltyTextBox":
		//									contentTextBox.Text = transactionTotalsModel.Sum(x => x.Loyalty).ToString();
		//									break;

		//								case string name when name == $"{totalText}amountTextBox":
		//									contentTextBox.Text = transactionTotalsModel.Sum(x => x.Cash + x.Card + x.UPI + x.Amex).ToString();
		//									break;
		//								case string name when name == $"{totalText}cashTextBox":
		//									contentTextBox.Text = transactionTotalsModel.Sum(x => x.Cash).ToString();
		//									break;
		//								case string name when name == $"{totalText}cardTextBox":
		//									contentTextBox.Text = transactionTotalsModel.Sum(x => x.Card).ToString();
		//									break;
		//								case string name when name == $"{totalText}upiTextBox":
		//									contentTextBox.Text = transactionTotalsModel.Sum(x => x.UPI).ToString();
		//									break;
		//								case string name when name == $"{totalText}amexTextBox":
		//									contentTextBox.Text = transactionTotalsModel.Sum(x => x.Amex).ToString();
		//									break;

		//								case string name when name == $"{totalText}advanceTextBox":
		//									contentTextBox.Text = advanceTotalsModel.Sum(x => x.TotalAdvance).ToString();
		//									break;
		//								case string name when name == $"{totalText}redeemedAdvanceTextBox":
		//									contentTextBox.Text = advanceTotalsModel.Sum(x => x.RedeemedAdvance).ToString();
		//									break;
		//								case string name when name == $"{totalText}notRedeemedAdvanceTextBox":
		//									contentTextBox.Text = advanceTotalsModel.Sum(x => x.NotRedeemedAdvance).ToString();
		//									break;

		//								case string name when name == $"{totalText}bookingTextBox":
		//									contentTextBox.Text = advanceTotalsModel.Sum(x => x.TotalBooking).ToString();
		//									break;
		//								case string name when name == $"{totalText}redeemedBookingTextBox":
		//									contentTextBox.Text = advanceTotalsModel.Sum(x => x.RedeemedBooking).ToString();
		//									break;
		//								case string name when name == $"{totalText}notRedeemedBookingTextBox":
		//									contentTextBox.Text = advanceTotalsModel.Sum(x => x.NotRedeemedBooking).ToString();
		//									break;
		//							}
		//	}
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
			Name = $"{locationName}headerAmountTextBox",
			Margin = new Thickness(0, 10, 10, 10),
			Padding = new Thickness(5),
			Text = "Amount",
			MinWidth = 100,
			TextAlignment = TextAlignment.Right,
			IsReadOnly = true
		};
		Grid.SetColumn(headerAmountTextBox, 3);
		headerGrid.Children.Add(headerAmountTextBox);

		var personText = new TextBlock { Text = "Person", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetColumn(personText, 5);
		headerGrid.Children.Add(personText);

		var headerPersonTextBox = new TextBox
		{
			Name = $"{locationName}headerPersonTextBox",
			Margin = new Thickness(0, 10, 10, 10),
			Padding = new Thickness(5),
			Text = "0",
			MinWidth = 100,
			TextAlignment = TextAlignment.Right,
			IsReadOnly = true
		};
		Grid.SetColumn(headerPersonTextBox, 6);
		headerGrid.Children.Add(headerPersonTextBox);

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

		// People Grid
		var peopleGrid = CreatePeopleGrid(locationName);
		Grid.SetColumn(peopleGrid, 0);
		contentGrid.Children.Add(peopleGrid);

		// Amount Grid
		var amountGrid = CreateAmountGrid(locationName);
		Grid.SetColumn(amountGrid, 2);
		contentGrid.Children.Add(amountGrid);

		// Advance Grid
		var advanceGrid = CreateAdvanceGrid(locationName);
		Grid.SetColumn(advanceGrid, 4);
		contentGrid.Children.Add(advanceGrid);

		// Booking Grid
		var bookingGrid = CreateBookingGrid(locationName);
		Grid.SetColumn(bookingGrid, 6);
		contentGrid.Children.Add(bookingGrid);

		#endregion

		return expander;
	}

	private static Grid CreatePeopleGrid(string locationName)
	{
		var grid = new Grid();
		// Column definitions
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Row definitions
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		// Controls
		var totalPeopleText = new TextBlock { Text = "Total People", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(totalPeopleText, 0);
		Grid.SetColumn(totalPeopleText, 0);
		grid.Children.Add(totalPeopleText);

		var peopleTextBox = new TextBox
		{
			Name = $"{locationName}TotalPeopleTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(peopleTextBox, 0);
		Grid.SetColumn(peopleTextBox, 1);
		grid.Children.Add(peopleTextBox);


		var maleText = new TextBlock { Text = "Male", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(maleText, 2);
		Grid.SetColumn(maleText, 0);
		grid.Children.Add(maleText);

		var maleTextBox = new TextBox
		{
			Name = $"{locationName}maleTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(maleTextBox, 2);
		Grid.SetColumn(maleTextBox, 1);
		grid.Children.Add(maleTextBox);


		var femaleText = new TextBlock { Text = "Female", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(femaleText, 3);
		Grid.SetColumn(femaleText, 0);
		grid.Children.Add(femaleText);

		var femaleTextBox = new TextBox
		{
			Name = $"{locationName}femaleTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(femaleTextBox, 3);
		Grid.SetColumn(femaleTextBox, 1);
		grid.Children.Add(femaleTextBox);


		var loyaltyText = new TextBlock { Text = "Loyalty", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(loyaltyText, 4);
		Grid.SetColumn(loyaltyText, 0);
		grid.Children.Add(loyaltyText);

		var loyaltyTextBox = new TextBox
		{
			Name = $"{locationName}loyaltyTextBox",
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

		return grid;
	}

	private static Grid CreateAmountGrid(string locationName)
	{
		var grid = new Grid();
		// Column definitions
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Row definitions
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		// Controls
		var amountText = new TextBlock { Text = "Total Amount", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(amountText, 0);
		Grid.SetColumn(amountText, 0);
		grid.Children.Add(amountText);

		var amountTextBox = new TextBox
		{
			Name = $"{locationName}amountTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(amountTextBox, 0);
		Grid.SetColumn(amountTextBox, 1);
		grid.Children.Add(amountTextBox);


		var cashText = new TextBlock { Text = "Cash", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(cashText, 2);
		Grid.SetColumn(cashText, 0);
		grid.Children.Add(cashText);

		var cashTextBox = new TextBox
		{
			Name = $"{locationName}cashTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(cashTextBox, 2);
		Grid.SetColumn(cashTextBox, 1);
		grid.Children.Add(cashTextBox);


		var cardText = new TextBlock { Text = "Card", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(cardText, 3);
		Grid.SetColumn(cardText, 0);
		grid.Children.Add(cardText);

		var cardTextBox = new TextBox
		{
			Name = $"{locationName}cardTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(cardTextBox, 3);
		Grid.SetColumn(cardTextBox, 1);
		grid.Children.Add(cardTextBox);


		var upiText = new TextBlock { Text = "UPI", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(upiText, 4);
		Grid.SetColumn(upiText, 0);
		grid.Children.Add(upiText);

		var upiTextBox = new TextBox
		{
			Name = $"{locationName}upiTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(upiTextBox, 4);
		Grid.SetColumn(upiTextBox, 1);
		grid.Children.Add(upiTextBox);


		var amexText = new TextBlock { Text = "Amex", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(amexText, 5);
		Grid.SetColumn(amexText, 0);
		grid.Children.Add(amexText);

		var amexTextBox = new TextBox
		{
			Name = $"{locationName}amexTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(amexTextBox, 5);
		Grid.SetColumn(amexTextBox, 1);
		grid.Children.Add(amexTextBox);

		return grid;
	}

	private static Grid CreateAdvanceGrid(string locationName)
	{
		var grid = new Grid();
		// Column definitions
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Row definitions
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		// Controls
		var advanceText = new TextBlock { Text = "Total Advance", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(advanceText, 0);
		Grid.SetColumn(advanceText, 0);
		grid.Children.Add(advanceText);

		var advanceTextBox = new TextBox
		{
			Name = $"{locationName}advanceTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(advanceTextBox, 0);
		Grid.SetColumn(advanceTextBox, 1);
		grid.Children.Add(advanceTextBox);


		var redeemedText = new TextBlock { Text = "Redeemed", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(redeemedText, 2);
		Grid.SetColumn(redeemedText, 0);
		grid.Children.Add(redeemedText);

		var redeemedTextBox = new TextBox
		{
			Name = $"{locationName}redeemedAdvanceTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(redeemedTextBox, 2);
		Grid.SetColumn(redeemedTextBox, 1);
		grid.Children.Add(redeemedTextBox);


		var notRedeemedText = new TextBlock { Text = "Not Redeemed", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(notRedeemedText, 3);
		Grid.SetColumn(notRedeemedText, 0);
		grid.Children.Add(notRedeemedText);

		var notRedeemedTextBox = new TextBox
		{
			Name = $"{locationName}notRedeemedAdvanceTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(notRedeemedTextBox, 3);
		Grid.SetColumn(notRedeemedTextBox, 1);
		grid.Children.Add(notRedeemedTextBox);

		return grid;
	}

	private static Grid CreateBookingGrid(string locationName)
	{
		var grid = new Grid();
		// Column definitions
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Row definitions
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		// Controls
		var bookingText = new TextBlock { Text = "Total Booking", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(bookingText, 0);
		Grid.SetColumn(bookingText, 0);
		grid.Children.Add(bookingText);

		var bookingTextBox = new TextBox
		{
			Name = $"{locationName}bookingTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(bookingTextBox, 0);
		Grid.SetColumn(bookingTextBox, 1);
		grid.Children.Add(bookingTextBox);


		var redeemedText = new TextBlock { Text = "Redeemed", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(redeemedText, 2);
		Grid.SetColumn(redeemedText, 0);
		grid.Children.Add(redeemedText);

		var redeemedTextBox = new TextBox
		{
			Name = $"{locationName}redeemedBookingTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(redeemedTextBox, 2);
		Grid.SetColumn(redeemedTextBox, 1);
		grid.Children.Add(redeemedTextBox);


		var notRedeemedText = new TextBlock { Text = "Not Redeemed", Margin = new Thickness(10), Padding = new Thickness(5) };
		Grid.SetRow(notRedeemedText, 3);
		Grid.SetColumn(notRedeemedText, 0);
		grid.Children.Add(notRedeemedText);

		var notRedeemedTextBox = new TextBox
		{
			Name = $"{locationName}notRedeemedBookingTextBox",
			Margin = new Thickness(10),
			Padding = new Thickness(5),
			Text = "0",
			TextAlignment = TextAlignment.Right,
			MinWidth = 100,
			IsReadOnly = true
		};
		Grid.SetRow(notRedeemedTextBox, 3);
		Grid.SetColumn(notRedeemedTextBox, 1);
		grid.Children.Add(notRedeemedTextBox);

		return grid;
	}

	#endregion
}

