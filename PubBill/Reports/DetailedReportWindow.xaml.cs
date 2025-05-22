using System.Windows;
using System.Windows.Controls;

namespace PubBill.Reports;

/// <summary>
/// Interaction logic for DetailedReport.xaml
/// </summary>
public partial class DetailedReportWindow : Window
{
	#region Load Properties
	private static int PubOpenTime => (int)Application.Current.Resources[SettingsKeys.PubOpenTime];
	private static int PubCloseTime => (int)Application.Current.Resources[SettingsKeys.PubCloseTime];
	private static int RefreshReportTimer => (int)Application.Current.Resources[SettingsKeys.RefreshReportTimer];

	private static DateTime _fromDateTime, _toDateTime;
	private static int _locationId;
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

	public DetailedReportWindow(DateTime fromDateTime, DateTime toDateTime, int locationId)
	{
		InitializeComponent();
		_fromDateTime = fromDateTime;
		_toDateTime = toDateTime;
		_locationId = locationId;
	}

	public DetailedReportWindow(DateTime fromDateTime, DateTime toDateTime)
	{
		InitializeComponent();
		_fromDateTime = fromDateTime;
		_toDateTime = toDateTime;
		_locationId = 0;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		await LoadComboBox();
		await LoadData();
		InitializeTimers();
	}

	private async Task LoadComboBox()
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

		fromDatePicker.DisplayDateEnd = toDatePicker.SelectedDate;
		toDatePicker.DisplayDateStart = fromDatePicker.SelectedDate;

		locationComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<LocationModel>(TableNames.Location);
		locationComboBox.DisplayMemberPath = nameof(LocationModel.Name);
		locationComboBox.SelectedValuePath = nameof(LocationModel.Id);

		if (_locationId > 0) locationComboBox.SelectedValue = _locationId;
		else locationComboBox.SelectedIndex = 0;
	}

	private async void values_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
		await LoadData();

	private async void locationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
		await LoadData();

	private async void RefreshData(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) =>
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
			fromSlotPicker is null ||
			locationComboBox.SelectedValue is null) return;

		fromDatePicker.DisplayDateEnd = toDatePicker.SelectedDate;
		toDatePicker.DisplayDateStart = fromDatePicker.SelectedDate;

		var fromTime = fromSlotPicker.SelectedItem.ToString() == "AM" ? (int)fromTimePicker.SelectedItem : (int)fromTimePicker.SelectedItem + 12;
		var toTime = toSlotPicker.SelectedItem.ToString() == "AM" ? (int)toTimePicker.SelectedItem : (int)toTimePicker.SelectedItem + 12;

		_fromDateTime = fromDatePicker.SelectedDate.Value.AddHours(fromTime);
		_toDateTime = toDatePicker.SelectedDate.Value.AddHours(toTime);

		_locationId = (int)locationComboBox.SelectedValue;

		Title = $"{(locationComboBox.SelectedItem as LocationModel).Name} Detailed Report - {_fromDateTime:dd/MM/yy h tt} to {_toDateTime:dd/MM/yy h tt}";

		billsDataGrid.ItemsSource = await BillReportData.LoadBillDetailsByDateLocationId(_fromDateTime, _toDateTime, _locationId);
	}

	private void PrintPDF(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		//MemoryStream ms = await PDF.Summary(_fromDateTime, _toDateTime);
		//using FileStream stream = new(Path.Combine(Path.GetTempPath(), "SummaryReport.pdf"), FileMode.Create, FileAccess.Write);
		//await ms.CopyToAsync(stream);
		//Process.Start(new ProcessStartInfo($"{Path.GetTempPath()}\\SummaryReport.pdf") { UseShellExecute = true });
	}

	private void ExportExcel(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		//MemoryStream ms = await PDF.Summary(_fromDateTime, _toDateTime);
		//using FileStream stream = new(Path.Combine(Path.GetTempPath(), "SummaryReport.pdf"), FileMode.Create, FileAccess.Write);
		//await ms.CopyToAsync(stream);
		//Process.Start(new ProcessStartInfo($"{Path.GetTempPath()}\\SummaryReport.pdf") { UseShellExecute = true });
	}
}
