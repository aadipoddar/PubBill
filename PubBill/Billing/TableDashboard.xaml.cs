﻿using System.Windows;
using System.Windows.Threading;

namespace PubBill.Billing;

/// <summary>
/// Interaction logic for TableDashboard.xaml
/// </summary>
public partial class TableDashboard : Window
{
	#region Timers

	private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(30) };

	private void InitializeTimers()
	{
		_timer.Tick += async (sender, e) => await RefreshScreen();
		_timer.Start();
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
		dateTimeTextBlock.Text = DateTime.Now.ToString("HH:mm tt");
		await CreateComponents.CreateDiningAreaExpanders(areasStackPanel, _user, _loginWindow, this);
	}

	private void Window_Closed(object sender, EventArgs e) => _loginWindow.Show();
}
