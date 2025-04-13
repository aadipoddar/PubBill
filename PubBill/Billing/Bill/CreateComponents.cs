using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PubBill.Billing.Bill;

static class CreateComponents
{
	internal static async Task CreateDiningAreaExpanders(StackPanel areaStackPanel, UserModel userModel, TableDashboard tableDashboard)
	{
		var location = await CommonData.LoadTableDataById<LocationModel>(TableNames.Location, userModel.LocationId);
		var diningAreas = await DiningAreaData.LoadDiningAreaByLocation(location.Id);
		var runningBills = await CommonData.LoadTableDataByStatus<RunningBillModel>(TableNames.RunningBill);
		runningBills = [.. runningBills.Where(b => b.LocationId == location.Id)];

		areaStackPanel.Children.Clear();

		foreach (var diningArea in diningAreas)
		{
			var diningTables = await DiningTableData.LoadDiningTableByDiningArea(diningArea.Id);
			var runningTables = runningBills.Where(b => b.DiningAreaId == diningArea.Id).ToList();

			var expander = new Expander
			{
				Name = $"{diningArea.Name.RemoveSpace()}{diningArea.Id}Expander",
				Header = diningArea.Name,
				IsExpanded = diningTables.Count > 0,
				Margin = new Thickness(10),
			};

			var itemsControl = new ItemsControl
			{
				Margin = new Thickness(10),
			};

			var itemsPanelTemplate = new ItemsPanelTemplate();
			var wrapPanelFactory = new FrameworkElementFactory(typeof(WrapPanel));
			wrapPanelFactory.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
			itemsPanelTemplate.VisualTree = wrapPanelFactory;
			itemsControl.ItemsPanel = itemsPanelTemplate;

			foreach (var table in diningTables)
			{
				var runningTable = runningTables.FirstOrDefault(b => b.DiningTableId == table.Id);

				Button button;
				if (runningTable is null) button = MakeTableButton(userModel, tableDashboard, table);
				else button = await MakeRunningTableButton(userModel, tableDashboard, table, runningTable);

				itemsControl.Items.Add(button);
			}

			expander.Content = itemsControl;

			areaStackPanel.Children.Add(expander);
		}
	}

	private static Button MakeTableButton(UserModel userModel, TableDashboard tableDashboard, DiningTableModel table)
	{
		Button button = new()
		{
			Name = $"{table.Name.RemoveSpace()}{table.Id}Button",
			Content = table.Name,
			FontWeight = FontWeights.SemiBold,
			FontSize = 20,
			Foreground = Brushes.Black,
			MinWidth = 120,
			MinHeight = 100,
			Background = Brushes.LightGreen,
			Margin = new Thickness(10),
			Padding = new Thickness(5),
		};

		button.Click += (sender, e) =>
		{
			BillWindow billWindow = new(userModel, tableDashboard, table);
			billWindow.Show();
			tableDashboard.Hide();
		};

		return button;
	}

	private static async Task<Button> MakeRunningTableButton(UserModel userModel, TableDashboard tableDashboard, DiningTableModel table, RunningBillModel runningBill)
	{
		var user = await CommonData.LoadTableDataById<UserModel>(TableNames.User, runningBill.UserId);
		var runningBillDetails = await RunningBillData.LoadRunningBillDetailByRunningBillId(runningBill.Id);
		var total = runningBillDetails.Where(x => !x.Cancelled).Sum(b => b.Rate * b.Quantity);

		var grid = new Grid();
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto, MinWidth = 20 });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto, MinHeight = 7 });
		grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto, MinHeight = 7 });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		var userNameTextBlock = new TextBlock
		{
			Text = $"{user.Name}",
			FontSize = 12,
			Foreground = Brushes.Black,
			HorizontalAlignment = HorizontalAlignment.Left
		};
		Grid.SetRow(userNameTextBlock, 0);
		Grid.SetColumn(userNameTextBlock, 0);
		grid.Children.Add(userNameTextBlock);

		var totalTimeTextBlock = new TextBlock
		{
			Text = $"{DateTime.Now - runningBill.BillStartDateTime:hh\\:mm}",
			FontSize = 12,
			HorizontalAlignment = HorizontalAlignment.Right
		};
		Grid.SetRow(totalTimeTextBlock, 0);
		Grid.SetColumn(totalTimeTextBlock, 2);
		grid.Children.Add(totalTimeTextBlock);

		var tableNameTextBlock = new TextBlock
		{
			Text = $"{table.Name}",
			FontWeight = FontWeights.Bold,
			FontSize = 20,
			HorizontalAlignment = HorizontalAlignment.Center
		};
		Grid.SetRow(tableNameTextBlock, 2);
		Grid.SetColumn(tableNameTextBlock, 0);
		Grid.SetColumnSpan(tableNameTextBlock, 3);
		grid.Children.Add(tableNameTextBlock);

		var totalAmountTextBlock = new TextBlock
		{
			Text = total.FormatIndianCurrency(),
			FontSize = 12,
			HorizontalAlignment = HorizontalAlignment.Center
		};
		Grid.SetRow(totalAmountTextBlock, 4);
		Grid.SetColumn(totalAmountTextBlock, 0);
		Grid.SetColumnSpan(totalAmountTextBlock, 3);
		grid.Children.Add(totalAmountTextBlock);

		var totalPeopleTextBlock = new TextBlock
		{
			Text = $"People: {runningBill.TotalPeople}",
			FontSize = 12,
			HorizontalAlignment = HorizontalAlignment.Center
		};
		Grid.SetRow(totalPeopleTextBlock, 5);
		Grid.SetColumn(totalPeopleTextBlock, 0);
		Grid.SetColumnSpan(totalPeopleTextBlock, 3);
		grid.Children.Add(totalPeopleTextBlock);

		Button button = new()
		{
			Name = $"{table.Name.RemoveSpace()}{table.Id}Button",
			Content = grid,
			FontWeight = FontWeights.SemiBold,
			FontSize = 20,
			Foreground = Brushes.Black,
			MinWidth = 120,
			MinHeight = 100,
			Background = Brushes.IndianRed,
			Margin = new Thickness(10),
			Padding = new Thickness(5),
		};

		button.Click += (sender, e) =>
		{
			BillWindow billWindow = new(tableDashboard, runningBill);
			billWindow.Show();
			tableDashboard.Hide();
		};

		return button;
	}

	internal static Button BuildProductButton(ProductModel product)
	{
		var button = new Button
		{
			Name = $"{product.Name.RemoveSpace()}{product.Id}Button",
			MinWidth = 140,
			MinHeight = 80,
			Width = 160,
			Height = 100,
			Margin = new Thickness(10),
			Padding = new Thickness(1),
			Background = Brushes.Thistle,
			BorderBrush = Brushes.DarkGray,
			BorderThickness = new Thickness(5),
			Cursor = Cursors.Hand,
			Content = new StackPanel
			{
				Children ={
						new TextBlock
						{
							Text = product.Name,
							FontWeight = FontWeights.Bold,
							FontSize = 14,
							Foreground = Brushes.Black,
							TextAlignment = TextAlignment.Center,
							Margin = new Thickness(5)
						},
						new TextBlock
						{
							Text = $"{product.Code}",
							FontWeight = FontWeights.Bold,
							FontSize = 12,
							Foreground = Brushes.DarkBlue,
							TextAlignment = TextAlignment.Center,
							Margin = new Thickness(5)
						},
						new TextBlock
						{
							Text = $"{product.Rate.FormatIndianCurrency()}",
							FontWeight = FontWeights.Bold,
							FontSize = 12,
							Foreground = Brushes.DarkGreen,
							TextAlignment = TextAlignment.Center,
							Margin = new Thickness(5)
						}
					}
			}
		};

		return button;
	}
}