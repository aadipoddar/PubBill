using System.Windows;
using System.Windows.Controls;

namespace PubBill.Inventory.Items;

/// <summary>
/// Interaction logic for SupplierWindow.xaml
/// </summary>
public partial class SupplierWindow : Window
{
	public SupplierWindow() =>
		InitializeComponent();

	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await ApplySearchFilter();

	#region Search and Filter Methods
	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		await ApplySearchFilter();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) =>
		await ApplySearchFilter();

	private async Task ApplySearchFilter()
	{
		if (supplierDataGrid is null) return;

		var nameSearch = searchTextBox.Text.Trim();

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;

		supplierDataGrid.ItemsSource = (await CommonData.LoadTableData<SupplierModel>(TableNames.Supplier))
			.Where(item => string.IsNullOrEmpty(nameSearch) ||
				item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase) ||
				item.Code.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase) ||
				(item.Phone?.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase) ?? false))
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.OrderBy(item => !item.Status)
			.ThenBy(item => item.Name)
			.ToList();

		UpdateFields();
	}
	#endregion

	#region UI Update Methods
	private void supplierDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) =>
		UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		UpdateButtonField();

	private void UpdateFields()
	{
		if (supplierDataGrid.SelectedItem is SupplierModel selectedSupplier)
		{
			nameTextBox.Text = selectedSupplier.Name;
			codeTextBox.Text = selectedSupplier.Code;
			gstNoTextBox.Text = selectedSupplier.GSTNo;
			phoneTextBox.Text = selectedSupplier.Phone;
			emailTextBox.Text = selectedSupplier.Email;
			addressTextBox.Text = selectedSupplier.Address;
			statusCheckBox.IsChecked = selectedSupplier.Status;
			saveButton.Content = "Update";
			saveButton.IsEnabled = true;
		}
		else
		{
			ClearFields();
		}
	}

	private void ClearFields()
	{
		nameTextBox.Clear();
		codeTextBox.Clear();
		gstNoTextBox.Clear();
		phoneTextBox.Clear();
		emailTextBox.Clear();
		addressTextBox.Clear();
		statusCheckBox.IsChecked = true;
		saveButton.Content = "Save";
		saveButton.IsEnabled = false;
		supplierDataGrid.SelectedItem = null;
	}

	private void UpdateButtonField()
	{
		if (supplierDataGrid.SelectedItem is null)
			saveButton.Content = "Save";
		else
			saveButton.Content = "Update";

		saveButton.IsEnabled = !string.IsNullOrEmpty(nameTextBox.Text);
	}
	#endregion

	#region Actions
	private void clearButton_Click(object sender, RoutedEventArgs e) =>
		ClearFields();

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		SupplierModel supplierModel = new()
		{
			Id = supplierDataGrid.SelectedItem is SupplierModel selectedSupplier ? selectedSupplier.Id : 0,
			Name = nameTextBox.Text.Trim(),
			Code = codeTextBox.Text.Trim(),
			GSTNo = gstNoTextBox.Text.Trim(),
			Phone = phoneTextBox.Text.Trim(),
			Email = emailTextBox.Text.Trim(),
			Address = addressTextBox.Text.Trim(),
			Status = statusCheckBox.IsChecked ?? true
		};

		await SupplierData.InsertSupplier(supplierModel);

		await ApplySearchFilter();
		ClearFields();
		MessageBox.Show(
			$"Supplier '{supplierModel.Name}' {(supplierModel.Id == 0 ? "added" : "updated")} successfully.",
			"Success",
			MessageBoxButton.OK,
			MessageBoxImage.Information);
	}

	private void phoneTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);
	#endregion

	#region Validation
	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(nameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Supplier Name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			nameTextBox.Focus();
			return false;
		}

		if (string.IsNullOrEmpty(codeTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Supplier Code", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			codeTextBox.Focus();
			return false;
		}

		return true;
	}
	#endregion
}
