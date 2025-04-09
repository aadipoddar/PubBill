using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PubBill.Billing.KOT;

static class CreateKOTComponents
{
	internal static async Task CreateLocationExpanders(StackPanel locationStackPanel, KOTDashboard kOTDashboard)
	{
		var locations = await CommonData.LoadTableDataByStatus<LocationModel>(TableNames.Location);
		var runningBills = await CommonData.LoadTableDataByStatus<RunningBillModel>(TableNames.RunningBill);

		locationStackPanel.Children.Clear();

		foreach (var location in locations)
		{
			var diningAreas = await DiningAreaData.LoadDiningAreaByLocation(location.Id);
			var runningTables = runningBills.Where(b => b.LocationId == location.Id).ToList();

			var expander = new Expander
			{
				Name = $"{location.Name.RemoveSpace()}{location.Id}Expander",
				Header = location.Name,
				IsExpanded = runningTables.Count > 0,
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

			foreach (var runningTable in runningTables)
			{
				Button button = await MakeRunningTableButton(kOTDashboard, runningTable);
				itemsControl.Items.Add(button);
			}
			expander.Content = itemsControl;

			locationStackPanel.Children.Add(expander);
		}
	}

	private static async Task<Button> MakeRunningTableButton(KOTDashboard kOTDashboard, RunningBillModel runningBill)
	{
		var user = await CommonData.LoadTableDataById<UserModel>(TableNames.User, runningBill.UserId);
		var table = await CommonData.LoadTableDataById<DiningTableModel>(TableNames.DiningTable, runningBill.DiningTableId);

		var productsCount = (await KOTData.LoadKOTBillDetailByRunningBillId(runningBill.Id)).Where(x => x.Status).Count();

		var runningBillDetails = await RunningBillData.LoadRunningBillDetailByRunningBillId(runningBill.Id);
		var total = runningBillDetails.Sum(b => b.Rate * b.Quantity);
		total -= runningBill.AdjAmount;

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
			Text = $"Products: {productsCount}",
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
			RunningKOTWindow runningKOTWindow = new(kOTDashboard, table, runningBill);
			runningKOTWindow.Show();
			kOTDashboard.Hide();
		};

		return button;
	}
}
