using System.Windows;

using PubBill.Admin;
using PubBill.Billing;
using PubBill.Billing.KOT;

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

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		if (_user is null) Close();
		else
		{
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
			}

			if (!isBill) billButton.Visibility = Visibility.Collapsed;
			if (!isKOT) kotButton.Visibility = Visibility.Collapsed;
			if (!isInventory) inventoryButton.Visibility = Visibility.Collapsed;
			if (!isAdmin) adminButton.Visibility = Visibility.Collapsed;

			if (isBill && !isKOT && !isInventory)
			{
				TableDashboard tableDashboard = new(_user, _loginWindow);
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
				// TODO
			}
		}
	}

	private void Window_Closed(object sender, EventArgs e) => _loginWindow.Show();

	private void billButton_Click(object sender, RoutedEventArgs e)
	{
		TableDashboard tableDashboard = new(_user, _loginWindow);
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

	}

	private void adminButton_Click(object sender, RoutedEventArgs e)
	{
		AdminPanel adminPanel = new(this);
		adminPanel.Show();
		Hide();
	}
}
