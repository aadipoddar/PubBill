using System.Windows;

using PubBill.Billing;

namespace PubBill
{
	/// <summary>
	/// Interaction logic for Dashboard.xaml
	/// </summary>
	public partial class Dashboard : Window
	{
		private readonly UserModel _user;

		public Dashboard(UserModel user)
		{
			InitializeComponent();
			_user = user;
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

		private void billButton_Click(object sender, RoutedEventArgs e)
		{
			BillWindow billWindow = new(_user);
			billWindow.Show();
			Close();
		}

		private void kotButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void adminButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
