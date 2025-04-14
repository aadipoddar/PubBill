using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin;

/// <summary>
/// Interaction logic for TaxPage.xaml
/// </summary>
public partial class TaxPage : Page
{
	public TaxPage() => InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) => await LoadData();

	private async Task LoadData()
	{
		if (taxDataGrid is null) return;

		var codeSearch = searchTextBox.Text.Trim();

		var taxes = await CommonData.LoadTableData<TaxModel>(TableNames.Tax);

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;

		taxDataGrid.ItemsSource = taxes
			.Where(item => string.IsNullOrEmpty(codeSearch) || item.Code.Contains(codeSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.OrderBy(item => !item.Status)
			.ToList();

		UpdateFields();
	}

	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) => await LoadData();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) => await LoadData();

	private void taxDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) => UpdateFields();

	private void codeTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateButtonField();

	private void UpdateFields()
	{
		if (taxDataGrid.SelectedItem is TaxModel selectedTax)
		{
			codeTextBox.Text = selectedTax.Code;
			cgstTextBox.Text = selectedTax.CGST.ToString();
			sgstTextBox.Text = selectedTax.SGST.ToString();
			igstTextBox.Text = selectedTax.IGST.ToString();
			statusCheckBox.IsChecked = selectedTax.Status;
			saveButton.Content = "Update";
			saveButton.IsEnabled = true;
		}

		else
		{
			codeTextBox.Clear();
			cgstTextBox.Text = "0";
			sgstTextBox.Text = "0";
			igstTextBox.Text = "0";
			statusCheckBox.IsChecked = true;
			saveButton.Content = "Save";
			saveButton.IsEnabled = false;
		}

		UpdateButtonField();
	}

	private void UpdateButtonField()
	{
		if (taxDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(codeTextBox.Text)) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(codeTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Tax Code", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(cgstTextBox.Text.Trim())) cgstTextBox.Text = "0";
		if (string.IsNullOrEmpty(sgstTextBox.Text.Trim())) sgstTextBox.Text = "0";
		if (string.IsNullOrEmpty(igstTextBox.Text.Trim())) igstTextBox.Text = "0";

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		TaxModel taxModel = new()
		{
			Id = taxDataGrid.SelectedItem is TaxModel selectedTax ? selectedTax.Id : 0,
			Code = codeTextBox.Text.ToUpper(),
			CGST = decimal.Parse(cgstTextBox.Text),
			SGST = decimal.Parse(sgstTextBox.Text),
			IGST = decimal.Parse(igstTextBox.Text),
			Status = (bool)statusCheckBox.IsChecked
		};

		await TaxData.InsertTax(taxModel);

		await LoadData();
	}
}
