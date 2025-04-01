using System.Windows;

using PubBill.Billing;

namespace PubBill;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : Window
{
	public LoginWindow() => InitializeComponent();

	private void Window_Loaded(object sender, RoutedEventArgs e) => userPasswordBox.Focus();

	private async void userPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
	{
		if (userPasswordBox.Password.Length == 4)
		{
			var user = await UserData.LoadUserByPassword(userPasswordBox.Password);
			if (user is not null)
			{
				if (user.Status is false)
				{
					MessageBox.Show("User is inactive. Please contact the administrator.", "Inactive User", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				if (user.Admin || (user.KOT && user.Bill))
				{
					Dashboard dashboard = new(user);
					dashboard.Show();
					Close();
					return;
				}

				if (user.Bill)
				{
					BillWindow billWindow = new(user);
					billWindow.Show();
					Close();
					return;
				}

				if (user.KOT)
				{
					// TODO
				}
			}
		}
	}
}
