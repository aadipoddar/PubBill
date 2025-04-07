using System.Windows;

using PubBill.Common;

namespace PubBill.Billing;

/// <summary>
/// Interaction logic for TableDashboard.xaml
/// </summary>
public partial class TableDashboard : Window
{
	#region Timers

	private SmartRefreshManager _refreshManager;

	private void InitializeTimers()
	{
		_refreshManager = new SmartRefreshManager(
			refreshAction: RefreshScreen,
			interval: TimeSpan.FromSeconds(20)
		);
		_refreshManager.Start();
	}

	#endregion

	private readonly UserModel _user;
	private readonly LoginWindow _loginWindow;

	public TableDashboard(UserModel user, LoginWindow loginWindow)
	{
		InitializeComponent();
		_user = user;
		_loginWindow = loginWindow;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) => await LoadData();

	private async Task LoadData()
	{
		InitializeTimers();

		if (_user is null)
		{
			MessageBox.Show("User not Valid. Please contact the administrator.", "Invalid User", MessageBoxButton.OK, MessageBoxImage.Error);
			Close();
		}

		userTextBlock.Text = _user.Name;

		var location = await CommonData.LoadTableDataById<LocationModel>(TableNames.Location, _user.LocationId);
		locationTextBlock.Text = location.Name;

		_loginWindow.Hide();

		await RefreshScreen();
	}

	public async Task RefreshScreen()
	{
		try
		{
			// Tell the inactivity monitor that we're starting a programmatic refresh
			InactivityMonitor.Instance.BeginRefreshOperation();

			dateTimeTextBlock.Text = DateTime.Now.ToString("HH:mm tt");
			await CreateComponents.CreateDiningAreaExpanders(areasStackPanel, _user, this);
		}
		finally
		{
			// Always make sure to end the refresh operation, even if an error occurs
			InactivityMonitor.Instance.EndRefreshOperation();
		}
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		_refreshManager.Stop();
		_refreshManager.Dispose();
		_loginWindow.Show();
	}
}
