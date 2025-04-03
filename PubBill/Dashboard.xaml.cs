using System.Windows;
using System.Windows.Threading;

using PubBill.Admin;
using PubBill.Billing;

namespace PubBill;

/// <summary>
/// Interaction logic for Dashboard.xaml
/// </summary>
public partial class Dashboard : Window
{
	#region Timers

	private readonly DispatcherTimer _inactivityTimer = new() { Interval = TimeSpan.FromSeconds(10) };

	private void InitializeTimers()
	{
		_inactivityTimer.Tick += (sender, e) => Close();
		_inactivityTimer.Start();
	}

	private void Dashboard_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		if (IsVisible) _inactivityTimer.Start();
		else _inactivityTimer.Stop();
	}

	#endregion

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
		InitializeTimers();
		IsVisibleChanged += Dashboard_IsVisibleChanged;

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
				BillWindow billWindow = new(_user);
				billWindow.Show();
				Hide();
			}

			else if (isKOT && !isBill && !isInventory)
			{
				// TODO
			}

			else if (isInventory && !isBill && !isKOT)
			{
				// TODO
			}
		}
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		_loginWindow.Show();
		Close();
	}

	private void billButton_Click(object sender, RoutedEventArgs e)
	{
		BillWindow billWindow = new(_user);
		billWindow.Show();
		Hide();
	}

	private void kotButton_Click(object sender, RoutedEventArgs e)
	{

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
