using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PubBill.Billing;

static class CreateComponents
{
	internal static async Task CreateDiningAreaExpanders(StackPanel areaStackPanel, List<DiningAreaModel> diningAreaModels, UserModel userModel, LoginWindow loginWindow, TableDashboard tableDashboard)
	{
		areaStackPanel.Children.Clear();

		foreach (var diningArea in diningAreaModels)
		{
			var diningTables = await DiningTableData.LoadDiningTableByDiningArea(diningArea.Id);

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
				var button = new Button
				{
					Name = $"{table.Name.RemoveSpace()}{table.Id}Button",
					Content = table.Name,
					FontWeight = FontWeights.SemiBold,
					FontSize = 20,
					Foreground = Brushes.Black,
					MinWidth = 120,
					MinHeight = 100,
					Background = table.Running ? Brushes.IndianRed : Brushes.LightGreen,
					Margin = new Thickness(10),
					Padding = new Thickness(5),
				};

				button.Click += (sender, e) =>
				{
					BillWindow billWindow = new(userModel, loginWindow, tableDashboard, table, diningArea);
					billWindow.Show();
					tableDashboard.Hide();
				};

				itemsControl.Items.Add(button);
			}

			expander.Content = itemsControl;

			areaStackPanel.Children.Add(expander);
		}
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
			Padding = new Thickness(5),
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