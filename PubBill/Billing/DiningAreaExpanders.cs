using System.Windows;
using System.Windows.Controls;

namespace PubBill.Billing;

static class DiningAreaExpanders
{
	internal static async Task CreateExpanders(StackPanel areaStackPanel, List<DiningAreaModel> diningAreaModels, UserModel userModel, LoginWindow loginWindow, TableDashboard tableDashboard)
	{
		areaStackPanel.Children.Clear();

		foreach (var diningArea in diningAreaModels)
		{
			var diningTables = await DiningTableData.LoadDiningTableByDiningArea(diningArea.Id);

			var expander = new Expander
			{
				Name = $"{diningArea.Name.RemoveSpace()}Expander",
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

			foreach (var tables in diningTables)
			{
				var button = new Button
				{
					Name = $"{tables.Name.RemoveSpace()}Button",
					Content = tables.Name,
					MinWidth = 120,
					MinHeight = 60,
					Margin = new Thickness(10),
					Padding = new Thickness(5),
				};

				button.Click += (sender, e) =>
				{
					BillWindow billWindow = new(userModel, loginWindow, tableDashboard);
					billWindow.Show();
					tableDashboard.Hide();
				};

				itemsControl.Items.Add(button);
			}

			expander.Content = itemsControl;

			areaStackPanel.Children.Add(expander);
		}
	}
}