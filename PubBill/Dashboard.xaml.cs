using System.Windows;

using PubBill.Admin;
using PubBill.Billing;

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
			if (!_user.Admin) adminButton.Visibility = Visibility.Collapsed;
			if (!_user.Bill) billButton.Visibility = Visibility.Collapsed;
			if (!_user.KOT) kotButton.Visibility = Visibility.Collapsed;

			if (!_user.Admin && !_user.Bill && !_user.KOT) Close();
			if (_user.Admin)
			{
				billButton.Visibility = Visibility.Visible;
				kotButton.Visibility = Visibility.Visible;
				adminButton.Visibility = Visibility.Visible;
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

	private void adminButton_Click(object sender, RoutedEventArgs e)
	{
		AdminPanel adminPanel = new(this);
		adminPanel.Show();
		Hide();
	}
}
