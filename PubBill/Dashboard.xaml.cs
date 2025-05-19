using System.Windows;

using PubBill.Admin;
using PubBill.Billing.Bill;
using PubBill.Billing.KOT;
using PubBill.Inventory;
using PubBill.Reports;

namespace PubBill;

/// <summary>
/// Interaction logic for Dashboard.xaml
/// </summary>
public partial class Dashboard : Window
{
	private readonly UserModel _user;
	private readonly LoginWindow _loginWindow;

	public Dashboard(UserModel user, LoginWindow loginWindow)
	{
		InitializeComponent();
		_user = user;
		_loginWindow = loginWindow;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		if (_user is null) Close();
		else
		{
			locationComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<LocationModel>(TableNames.Location);
			locationComboBox.DisplayMemberPath = nameof(LocationModel.Name);
			locationComboBox.SelectedValuePath = nameof(LocationModel.Id);
			locationComboBox.SelectedValue = _user.LocationId;

			bool isAdmin = _user.Admin;
			bool isBill = _user.Bill;
			bool isKOT = _user.KOT;
			bool isInventory = _user.Inventory;

			if (!isAdmin && !isBill && !isKOT && !isInventory)
			{
				MessageBox.Show("User does not have any permissions. Please contact the administrator.", "No Permissions", MessageBoxButton.OK, MessageBoxImage.Error);
				Close();
			}

			if (isAdmin)
			{
				billButton.Visibility = Visibility.Visible;
				kotButton.Visibility = Visibility.Visible;
				inventoryButton.Visibility = Visibility.Visible;
				adminButton.Visibility = Visibility.Visible;
				reportButton.Visibility = Visibility.Visible;
			}

			if (!isBill) billButton.Visibility = Visibility.Collapsed;
			if (!isKOT) kotButton.Visibility = Visibility.Collapsed;
			if (!isInventory) inventoryButton.Visibility = Visibility.Collapsed;
			if (!isAdmin)
			{
				adminButton.Visibility = Visibility.Collapsed;
				reportButton.Visibility = Visibility.Collapsed;
			}

			if (isBill && !isKOT && !isInventory)
			{
				TableDashboard tableDashboard = new(_user, locationComboBox.SelectedItem as LocationModel, _loginWindow);
				tableDashboard.Show();
				Close();
			}

			else if (isKOT && !isBill && !isInventory)
			{
				KOTDashboard kOTDashboard = new(_loginWindow);
				kOTDashboard.Show();
				Close();
			}

			else if (isInventory && !isBill && !isKOT)
			{
				InventoryDashboard inventoryDashboard = new(_loginWindow);
				inventoryDashboard.Show();
				Close();
			}
		}
	}

	private void billButton_Click(object sender, RoutedEventArgs e)
	{
		TableDashboard tableDashboard = new(_user, locationComboBox.SelectedItem as LocationModel, _loginWindow);
		tableDashboard.Show();
		Close();
	}

	private void kotButton_Click(object sender, RoutedEventArgs e)
	{
		KOTDashboard kOTDashboard = new(_loginWindow);
		kOTDashboard.Show();
		Close();
	}

	private void inventoryButton_Click(object sender, RoutedEventArgs e)
	{
		InventoryDashboard inventoryDashboard = new(_loginWindow);
		inventoryDashboard.Show();
		Close();
		_loginWindow.Hide();
	}

	private void reportButton_Click(object sender, RoutedEventArgs e)
	{
		SummaryReport summaryReport = new(_loginWindow);
		summaryReport.Show();
		Close();
		_loginWindow.Hide();
	}

	private void adminButton_Click(object sender, RoutedEventArgs e)
	{
		AdminPanel adminPanel = new(_loginWindow);
		adminPanel.Show();
		Close();
		_loginWindow.Hide();
	}

	private void Window_Closed(object sender, EventArgs e) =>
		_loginWindow.Show();
}
