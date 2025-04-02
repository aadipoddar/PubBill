using System.Text.RegularExpressions;
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
				userPasswordBox.Clear();

				if (user.Status is false)
				{
					MessageBox.Show("User is inactive. Please contact the administrator.", "Inactive User", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				if (user.Admin || (user.KOT && user.Bill))
				{
					Dashboard dashboard = new(user, this);
					dashboard.Show();
					Hide();
					return;
				}

				if (user.Bill)
				{
					BillWindow billWindow = new(user);
					billWindow.Show();
					Hide();
					return;
				}

				if (user.KOT)
				{
					// TODO
				}
			}
		}
	}

	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
	{
		Regex regex = new("[^0-9]+");
		e.Handled = regex.IsMatch(e.Text);
	}
}
