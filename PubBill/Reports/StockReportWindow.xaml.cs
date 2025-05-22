using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PubBill.Reports;

/// <summary>
/// Interaction logic for StockReportWindow.xaml
/// </summary>
public partial class StockReportWindow : Window
{
	#region Load Properties
	private static int PubOpenTime => (int)Application.Current.Resources[SettingsKeys.PubOpenTime];
	private static int PubCloseTime => (int)Application.Current.Resources[SettingsKeys.PubCloseTime];
	private static int RefreshReportTimer => (int)Application.Current.Resources[SettingsKeys.RefreshReportTimer];

	private DateTime _fromDateTime, _toDateTime;
	#endregion

	#region Timers
	private SmartRefreshManager _refreshManager;

	private void InitializeTimers()
	{
		_refreshManager = new SmartRefreshManager(
			refreshAction: RefreshScreen,
			interval: TimeSpan.FromSeconds(RefreshReportTimer)
		);
		_refreshManager.Start();
	}

	public async Task RefreshScreen()
	{
		try
		{
			// Tell the inactivity monitor that we're starting a programmatic refresh
			InactivityMonitor.Instance.BeginRefreshOperation();
			await LoadData();
		}
		finally
		{
			// Always make sure to end the refresh operation, even if an error occurs
			InactivityMonitor.Instance.EndRefreshOperation();
		}
	}
	#endregion

	public StockReportWindow(DateTime fromDateTime, DateTime toDateTime)
	{
		InitializeComponent();
		_fromDateTime = fromDateTime;
		_toDateTime = toDateTime;
	}

	#region Load Data
	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		LoadComboBox();
		await LoadData();
		InitializeTimers();
	}

	private void LoadComboBox()
	{
		if (_fromDateTime == default && _toDateTime == default)
		{
			if (DateTime.Now.Hour >= PubOpenTime)
			{
				toDatePicker.SelectedDate = DateTime.Now.Date.AddDays(1);
				fromDatePicker.SelectedDate = DateTime.Now.Date;
			}
			else
			{
				toDatePicker.SelectedDate = DateTime.Now.Date;
				fromDatePicker.SelectedDate = DateTime.Now.Date.AddDays(-1);
			}
		}
		else
		{
			fromDatePicker.SelectedDate = _fromDateTime.Date;
			toDatePicker.SelectedDate = _toDateTime.Date;
		}

		fromDatePicker.DisplayDateEnd = toDatePicker.SelectedDate;
		toDatePicker.DisplayDateStart = fromDatePicker.SelectedDate;
	}

	private async void values_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
		await LoadData();

	private async void RefreshData(object sender, ExecutedRoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		if (toDatePicker.SelectedDate is null || fromDatePicker.SelectedDate is null) return;

		fromDatePicker.DisplayDateEnd = toDatePicker.SelectedDate;
		toDatePicker.DisplayDateStart = fromDatePicker.SelectedDate;

		_fromDateTime = fromDatePicker.SelectedDate.Value;
		_toDateTime = toDatePicker.SelectedDate.Value;

		stocksDataGrid.ItemsSource = await StockData.LoadStockDetailsByDate(DateOnly.FromDateTime(_fromDateTime), DateOnly.FromDateTime(_toDateTime));
	}
	#endregion

	private void ExportExcel(object sender, ExecutedRoutedEventArgs e)
	{

	}
}
