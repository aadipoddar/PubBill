using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin;

/// <summary>
/// Interaction logic for DiningAreaPage.xaml
/// </summary>
public partial class DiningAreaPage : Page
{
	public DiningAreaPage() => InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) => await LoadData();

	private async Task LoadData()
	{
		var locations = await CommonData.LoadTableData<LocationModel>(TableNames.Location);

		searchLocationComboBox.ItemsSource = locations;
		searchLocationComboBox.DisplayMemberPath = nameof(LocationModel.Name);
		searchLocationComboBox.SelectedValuePath = nameof(LocationModel.Id);
		searchLocationComboBox.SelectedIndex = 0;

		locationComboBox.ItemsSource = locations;
		locationComboBox.DisplayMemberPath = nameof(LocationModel.Name);
		locationComboBox.SelectedValuePath = nameof(LocationModel.Id);
		locationComboBox.SelectedIndex = 0;

		await ApplySearchFilter();
	}

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) => await ApplySearchFilter();

	private async void searchLocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => await ApplySearchFilter();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) => await ApplySearchFilter();

	private void diningAreaDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) => UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateButtonField();

	private async Task ApplySearchFilter()
	{
		if (diningAreaDataGrid is null || searchLocationComboBox is null) return;

		var nameSearch = searchTextBox.Text.Trim();

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;

		diningAreaDataGrid.ItemsSource = (await DiningAreaData.LoadDiningAreaByLocation((int)searchLocationComboBox.SelectedValue))
			.Where(item => string.IsNullOrEmpty(nameSearch) || item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.OrderBy(item => !item.Status)
			.ToList();

		UpdateFields();
	}

	private void UpdateFields()
	{
		if (diningAreaDataGrid.SelectedItem is DiningAreaModel selectedDiningArea)
		{
			nameTextBox.Text = selectedDiningArea.Name;
			locationComboBox.SelectedValue = selectedDiningArea.LocationId;
			statusCheckBox.IsChecked = selectedDiningArea.Status;
			saveButton.Content = "Update";
			saveButton.IsEnabled = true;
		}

		else
		{
			nameTextBox.Clear();
			statusCheckBox.IsChecked = true;
			saveButton.Content = "Save";
			saveButton.IsEnabled = false;
		}

		UpdateButtonField();
	}

	private void UpdateButtonField()
	{
		if (diningAreaDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(nameTextBox.Text)) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(nameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter an Area Name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (locationComboBox.SelectedIndex == -1)
		{
			MessageBox.Show("Please select a Location", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		DiningAreaModel diningAreaModel = new()
		{
			Id = diningAreaDataGrid.SelectedItem is DiningAreaModel selectedDiningArea ? selectedDiningArea.Id : 0,
			Name = nameTextBox.Text,
			LocationId = (int)locationComboBox.SelectedValue,
			Status = (bool)statusCheckBox.IsChecked
		};

		await DiningAreaData.InsertDiningArea(diningAreaModel);

		await ApplySearchFilter();
	}
}
