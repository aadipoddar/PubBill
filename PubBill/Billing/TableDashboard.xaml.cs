using System.Windows;
using System.Windows.Threading;

namespace PubBill.Billing;

/// <summary>
/// Interaction logic for TableDashboard.xaml
/// </summary>
public partial class TableDashboard : Window
{
	#region Timers

	private readonly DispatcherTimer _inactivityTimer = new() { Interval = TimeSpan.FromSeconds(5) };
	private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(10) };

	private void InitializeTimers()
	{
		_timer.Tick += (sender, e) => dateTimeTextBlock.Text = DateTime.Now.ToString("HH:mm tt");
		_timer.Start();

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
	private readonly Dashboard _dashboard;
	private readonly LoginWindow _loginWindow;

	public TableDashboard(UserModel user, Dashboard dashboard, LoginWindow loginWindow)
	{
		InitializeComponent();
		_user = user;
		_dashboard = dashboard;
		_loginWindow = loginWindow;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) => await LoadData();

	private async Task LoadData()
	{
		InitializeTimers();
		IsVisibleChanged += Dashboard_IsVisibleChanged;

		if (_user is null)
		{
			MessageBox.Show("User not Valid. Please contact the administrator.", "Invalid User", MessageBoxButton.OK, MessageBoxImage.Error);
			Close();
		}

		userTextBlock.Text = _user.Name;

		var location = await CommonData.LoadTableDataById<LocationModel>(TableNames.Location, _user.LocationId);
		locationTextBlock.Text = location.Name;

		dateTimeTextBlock.Text = DateTime.Now.ToString("HH:mm tt");
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		if (_user.Bill && !_user.KOT && !_user.Inventory)
		{
			_loginWindow.Show();
			Close();
		}

		else
		{
			_dashboard.Show();
			Close();
		}
	}
}
