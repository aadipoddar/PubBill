using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin;

/// <summary>
/// Interaction logic for UserPage.xaml
/// </summary>
public partial class UserPage : Page
{
	public UserPage() => InitializeComponent();

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

	private void adminCheckBox_Checked(object sender, RoutedEventArgs e)
	{
		billCheckBox.IsChecked = true;
		kotCheckBox.IsChecked = true;
		inventoryCheckBox.IsChecked = true;
	}

	private void adminCheckBox_Unchecked(object sender, RoutedEventArgs e)
	{
		billCheckBox.IsChecked = true;
		kotCheckBox.IsChecked = false;
		inventoryCheckBox.IsChecked = false;
	}

	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
	{
		Regex regex = new("[^0-9]+");
		e.Handled = regex.IsMatch(e.Text);
	}

	private async Task ApplySearchFilter()
	{
		if (userDataGrid is null || searchLocationComboBox is null) return;

		var nameSearch = searchTextBox.Text.Trim();

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;
		bool showAdmin = showAdminCheckBox?.IsChecked ?? false;
		bool showNonAdmin = showNonAdminCheckBox?.IsChecked ?? false;
		bool showBill = showBillCheckBox?.IsChecked ?? false;
		bool showNonBill = true;
		bool showKOT = showKOTCheckBox?.IsChecked ?? false;
		bool showNonKOT = true;
		bool showInventory = showInventoryCheckBox?.IsChecked ?? false;
		bool showNonInventory = true;

		userDataGrid.ItemsSource = (await CommonData.LoadTableData<UserModel>(TableNames.User))
			.Where(item => string.IsNullOrEmpty(nameSearch) || item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => item.LocationId == (int)searchLocationComboBox.SelectedValue)
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.Where(item => (showAdmin && item.Admin) || (showNonAdmin && !item.Admin))
			.Where(item => (showBill && item.Bill) || (showNonBill && !item.Bill))
			.Where(item => (showKOT && item.KOT) || (showNonKOT && !item.KOT))
			.Where(item => (showInventory && item.Inventory) || (showNonInventory && !item.Inventory))
			.OrderBy(item => !item.Status)
			.ToList();

		UpdateFields();
	}

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) => await ApplySearchFilter();

	private async void searchLocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => await ApplySearchFilter();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) => await ApplySearchFilter();

	private void userDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) => UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateButtonField();

	private void passwordTextBox_TextChanged(object sender, RoutedEventArgs e) => UpdateButtonField();

	private void UpdateFields()
	{
		if (userDataGrid.SelectedItem is UserModel selectedUser)
		{
			nameTextBox.Text = selectedUser.Name;
			passwordBox.Password = selectedUser.Password.ToString();
			statusCheckBox.IsChecked = selectedUser.Status;
			billCheckBox.IsChecked = selectedUser.Bill;
			kotCheckBox.IsChecked = selectedUser.KOT;
			inventoryCheckBox.IsChecked = selectedUser.Inventory;
			adminCheckBox.IsChecked = selectedUser.Admin;
			locationComboBox.SelectedValue = selectedUser.LocationId;
			saveButton.Content = "Update";
			saveButton.IsEnabled = true;
		}

		else
		{
			nameTextBox.Clear();
			passwordBox.Clear();
			statusCheckBox.IsChecked = true;
			billCheckBox.IsChecked = true;
			kotCheckBox.IsChecked = false;
			inventoryCheckBox.IsChecked = false;
			adminCheckBox.IsChecked = false;
			saveButton.Content = "Save";
			saveButton.IsEnabled = false;
		}

		UpdateButtonField();
	}

	private void UpdateButtonField()
	{
		if (userDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(nameTextBox.Text) && !string.IsNullOrEmpty(passwordBox.Password)) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(nameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a User Name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(passwordBox.Password.Trim()))
		{
			MessageBox.Show("Please enter a Password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (locationComboBox.SelectedIndex == -1)
		{
			MessageBox.Show("Please select a Location", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (billCheckBox.IsChecked == false && kotCheckBox.IsChecked == false && inventoryCheckBox.IsChecked == false)
		{
			MessageBox.Show("Please select at least one Permission", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (adminCheckBox.IsChecked == true && (billCheckBox.IsChecked == false || kotCheckBox.IsChecked == false || inventoryCheckBox.IsChecked == false))
		{
			MessageBox.Show("Admin must have all permissions", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		UserModel userModel = new()
		{
			Id = userDataGrid.SelectedItem is UserModel selectedUser ? selectedUser.Id : 0,
			Name = nameTextBox.Text,
			Password = short.Parse(passwordBox.Password),
			Bill = (bool)billCheckBox.IsChecked,
			KOT = (bool)kotCheckBox.IsChecked,
			Inventory = (bool)inventoryCheckBox.IsChecked,
			Admin = (bool)adminCheckBox.IsChecked,
			LocationId = (int)locationComboBox.SelectedValue,
			Status = (bool)statusCheckBox.IsChecked
		};

		await UserData.InsertUser(userModel);

		await ApplySearchFilter();
	}
}
