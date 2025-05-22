using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PubBill.Reports;

/// <summary>
/// Interaction logic for ItemReportWindow.xaml
/// </summary>
public partial class ItemReportWindow : Window
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

	public ItemReportWindow(DateTime fromDateTime, DateTime toDateTime)
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

			List<int> hours = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
			List<string> slots = ["AM", "PM"];

			fromTimePicker.ItemsSource = hours;
			toTimePicker.ItemsSource = hours;
			fromSlotPicker.ItemsSource = slots;
			toSlotPicker.ItemsSource = slots;

			if (PubOpenTime >= 12)
			{
				fromSlotPicker.SelectedItem = "PM";
				if (PubOpenTime > 12) fromTimePicker.SelectedItem = PubOpenTime - 12;
				else fromTimePicker.SelectedItem = PubOpenTime;
			}
			else
			{
				fromSlotPicker.SelectedItem = "AM";
				fromTimePicker.SelectedItem = PubOpenTime;
			}

			if (PubCloseTime >= 12)
			{
				toSlotPicker.SelectedItem = "PM";
				if (PubCloseTime > 12) toTimePicker.SelectedItem = PubCloseTime - 12;
				else toTimePicker.SelectedItem = PubCloseTime;
			}
			else
			{
				toSlotPicker.SelectedItem = "AM";
				toTimePicker.SelectedItem = PubCloseTime;
			}
		}
		else
		{
			fromDatePicker.SelectedDate = _fromDateTime.Date;
			toDatePicker.SelectedDate = _toDateTime.Date;

			List<int> hours = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
			List<string> slots = ["AM", "PM"];

			fromTimePicker.ItemsSource = hours;
			toTimePicker.ItemsSource = hours;
			fromSlotPicker.ItemsSource = slots;
			toSlotPicker.ItemsSource = slots;

			var fromHour = _fromDateTime.Hour;
			var toHour = _toDateTime.Hour;

			if (fromHour >= 12)
			{
				fromSlotPicker.SelectedItem = "PM";
				fromTimePicker.SelectedItem = fromHour > 12 ? fromHour - 12 : 12;
			}
			else
			{
				fromSlotPicker.SelectedItem = "AM";
				fromTimePicker.SelectedItem = fromHour == 0 ? 12 : fromHour;
			}

			if (toHour >= 12)
			{
				toSlotPicker.SelectedItem = "PM";
				toTimePicker.SelectedItem = toHour > 12 ? toHour - 12 : 12;
			}
			else
			{
				toSlotPicker.SelectedItem = "AM";
				toTimePicker.SelectedItem = toHour == 0 ? 12 : toHour;
			}
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
		if (toDatePicker.SelectedDate is null ||
			fromDatePicker.SelectedDate is null ||
			fromTimePicker.SelectedItem is null ||
			toTimePicker.SelectedItem is null ||
			toTimePicker is null ||
			fromTimePicker is null ||
			toSlotPicker is null ||
			fromSlotPicker is null) return;

		fromDatePicker.DisplayDateEnd = toDatePicker.SelectedDate;
		toDatePicker.DisplayDateStart = fromDatePicker.SelectedDate;

		var fromTime = fromSlotPicker.SelectedItem.ToString() == "AM" ? (int)fromTimePicker.SelectedItem : (int)fromTimePicker.SelectedItem + 12;
		var toTime = toSlotPicker.SelectedItem.ToString() == "AM" ? (int)toTimePicker.SelectedItem : (int)toTimePicker.SelectedItem + 12;

		_fromDateTime = fromDatePicker.SelectedDate.Value.AddHours(fromTime);
		_toDateTime = toDatePicker.SelectedDate.Value.AddHours(toTime);

		itemsDataGrid.ItemsSource = await ProductData.LoadItemDetailsByDate(_fromDateTime, _toDateTime);
	}
	#endregion

	private void ExportExcel(object sender, ExecutedRoutedEventArgs e)
	{

	}

	private void StockReport(object sender, ExecutedRoutedEventArgs e)
	{
		StockReportWindow stockReportWindow = new(_fromDateTime, _toDateTime);
		stockReportWindow.Show();
	}
}
