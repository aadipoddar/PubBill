using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace PubBill.Billing;

internal static class ThermalParagraphs
{
	#region LoadProperties

	private static FontFamily HeaderFontFamilyThermal => (FontFamily)Application.Current.Resources[SettingsKeys.HeaderFontFamilyThermal];
	private static int HeaderFontSizeThermal => (int)Application.Current.Resources[SettingsKeys.HeaderFontSizeThermal];
	private static FontWeight HeaderFontWeightThermal => (FontWeight)Application.Current.Resources[SettingsKeys.HeaderFontWeightThermal];
	private static int HeaderFontPaddingTopThermal => (int)Application.Current.Resources[SettingsKeys.HeaderFontPaddingTopThermal];
	private static int HeaderFontPaddingBottomThermal => (int)Application.Current.Resources[SettingsKeys.HeaderFontPaddingBottomThermal];
	private static int HeaderFontPaddingLeftThermal => (int)Application.Current.Resources[SettingsKeys.HeaderFontPaddingLeftThermal];
	private static int HeaderFontPaddingRightThermal => (int)Application.Current.Resources[SettingsKeys.HeaderFontPaddingRightThermal];

	private static FontFamily SubHeaderFontFamilyThermal => (FontFamily)Application.Current.Resources[SettingsKeys.SubHeaderFontFamilyThermal];
	private static int SubHeaderFontSizeThermal => (int)Application.Current.Resources[SettingsKeys.SubHeaderFontSizeThermal];
	private static FontWeight SubHeaderFontWeightThermal => (FontWeight)Application.Current.Resources[SettingsKeys.SubHeaderFontWeightThermal];
	private static int SubHeaderFontPaddingTopThermal => (int)Application.Current.Resources[SettingsKeys.SubHeaderFontPaddingTopThermal];
	private static int SubHeaderFontPaddingBottomThermal => (int)Application.Current.Resources[SettingsKeys.SubHeaderFontPaddingBottomThermal];
	private static int SubHeaderFontPaddingLeftThermal => (int)Application.Current.Resources[SettingsKeys.SubHeaderFontPaddingLeftThermal];
	private static int SubHeaderFontPaddingRightThermal => (int)Application.Current.Resources[SettingsKeys.SubHeaderFontPaddingRightThermal];

	private static FontFamily RegularFontFamilyThermal => (FontFamily)Application.Current.Resources[SettingsKeys.RegularFontFamilyThermal];
	private static int RegularFontSizeThermal => (int)Application.Current.Resources[SettingsKeys.RegularFontSizeThermal];
	private static FontWeight RegularFontWeightThermal => (FontWeight)Application.Current.Resources[SettingsKeys.RegularFontWeightThermal];
	private static int RegularFontPaddingTopThermal => (int)Application.Current.Resources[SettingsKeys.RegularFontPaddingTopThermal];
	private static int RegularFontPaddingBottomThermal => (int)Application.Current.Resources[SettingsKeys.RegularFontPaddingBottomThermal];
	private static int RegularFontPaddingLeftThermal => (int)Application.Current.Resources[SettingsKeys.RegularFontPaddingLeftThermal];
	private static int RegularFontPaddingRightThermal => (int)Application.Current.Resources[SettingsKeys.RegularFontPaddingRightThermal];

	private static FontFamily TableRowFontFamilyThermal => (FontFamily)Application.Current.Resources[SettingsKeys.TableRowFontFamilyThermal];
	private static int TableRowFontSizeThermal => (int)Application.Current.Resources[SettingsKeys.TableRowFontSizeThermal];
	private static FontWeight TableRowFontWeightThermal => (FontWeight)Application.Current.Resources[SettingsKeys.TableRowFontWeightThermal];
	private static int TableRowFontPaddingTopThermal => (int)Application.Current.Resources[SettingsKeys.TableRowFontPaddingTopThermal];
	private static int TableRowFontPaddingBottomThermal => (int)Application.Current.Resources[SettingsKeys.TableRowFontPaddingBottomThermal];
	private static int TableRowFontPaddingLeftThermal => (int)Application.Current.Resources[SettingsKeys.TableRowFontPaddingLeftThermal];
	private static int TableRowFontPaddingRightThermal => (int)Application.Current.Resources[SettingsKeys.TableRowFontPaddingRightThermal];

	private static FontFamily TableHeaderFontFamilyThermal => (FontFamily)Application.Current.Resources[SettingsKeys.TableHeaderFontFamilyThermal];
	private static int TableHeaderFontSizeThermal => (int)Application.Current.Resources[SettingsKeys.TableHeaderFontSizeThermal];
	private static FontWeight TableHeaderFontWeightThermal => (FontWeight)Application.Current.Resources[SettingsKeys.TableHeaderFontWeightThermal];
	private static int TableHeaderFontPaddingTopThermal => (int)Application.Current.Resources[SettingsKeys.TableHeaderFontPaddingTopThermal];
	private static int TableHeaderFontPaddingBottomThermal => (int)Application.Current.Resources[SettingsKeys.TableHeaderFontPaddingBottomThermal];
	private static int TableHeaderFontPaddingLeftThermal => (int)Application.Current.Resources[SettingsKeys.TableHeaderFontPaddingLeftThermal];
	private static int TableHeaderFontPaddingRightThermal => (int)Application.Current.Resources[SettingsKeys.TableHeaderFontPaddingRightThermal];

	private static FontFamily FooterFontFamilyThermal => (FontFamily)Application.Current.Resources[SettingsKeys.FooterFontFamilyThermal];
	private static int FooterFontSizeThermal => (int)Application.Current.Resources[SettingsKeys.FooterFontSizeThermal];
	private static FontWeight FooterFontWeightThermal => (FontWeight)Application.Current.Resources[SettingsKeys.FooterFontWeightThermal];
	private static int FooterFontPaddingTopThermal => (int)Application.Current.Resources[SettingsKeys.FooterFontPaddingTopThermal];
	private static int FooterFontPaddingBottomThermal => (int)Application.Current.Resources[SettingsKeys.FooterFontPaddingBottomThermal];
	private static int FooterFontPaddingLeftThermal => (int)Application.Current.Resources[SettingsKeys.FooterFontPaddingLeftThermal];
	private static int FooterFontPaddingRightThermal => (int)Application.Current.Resources[SettingsKeys.FooterFontPaddingRightThermal];

	private static FontFamily SeparatorFontFamilyThermal => (FontFamily)Application.Current.Resources[SettingsKeys.SeparatorFontFamilyThermal];
	private static int SeparatorFontSizeThermal => (int)Application.Current.Resources[SettingsKeys.SeparatorFontSizeThermal];
	private static FontWeight SeparatorFontWeightThermal => (FontWeight)Application.Current.Resources[SettingsKeys.SeparatorFontWeightThermal];
	private static int SeparatorFontPaddingTopThermal => (int)Application.Current.Resources[SettingsKeys.SeparatorFontPaddingTopThermal];
	private static int SeparatorFontPaddingBottomThermal => (int)Application.Current.Resources[SettingsKeys.SeparatorFontPaddingBottomThermal];
	private static int SeparatorFontPaddingLeftThermal => (int)Application.Current.Resources[SettingsKeys.SeparatorFontPaddingLeftThermal];
	private static int SeparatorFontPaddingRightThermal => (int)Application.Current.Resources[SettingsKeys.SeparatorFontPaddingRightThermal];
	private static int SeparatorDashCountThermal => (int)Application.Current.Resources[SettingsKeys.SeparatorDashCountThermal];

	#endregion

	internal static Paragraph HeaderParagraph(string text)
	{
		return new(new Run(text))
		{
			TextAlignment = TextAlignment.Center,
			FontSize = HeaderFontSizeThermal,
			FontWeight = HeaderFontWeightThermal,
			FontFamily = HeaderFontFamilyThermal,
			Margin = new Thickness(HeaderFontPaddingLeftThermal, HeaderFontPaddingTopThermal, HeaderFontPaddingRightThermal, HeaderFontPaddingBottomThermal),
			LineHeight = HeaderFontSizeThermal,
			LineStackingStrategy = LineStackingStrategy.BlockLineHeight
		};
	}

	internal static Paragraph SubHeaderParagraph(string text)
	{
		return new(new Run(text))
		{
			TextAlignment = TextAlignment.Center,
			FontSize = SubHeaderFontSizeThermal,
			FontStyle = FontStyles.Italic,
			FontFamily = SubHeaderFontFamilyThermal,
			FontWeight = SubHeaderFontWeightThermal,
			Margin = new Thickness(SubHeaderFontPaddingLeftThermal, SubHeaderFontPaddingTopThermal, SubHeaderFontPaddingRightThermal, SubHeaderFontPaddingBottomThermal),
			LineHeight = SubHeaderFontSizeThermal,
			LineStackingStrategy = LineStackingStrategy.BlockLineHeight
		};
	}

	internal static Paragraph RegularParagraph(string text)
	{
		return new(new Run(text))
		{
			FontSize = RegularFontSizeThermal,
			FontFamily = RegularFontFamilyThermal,
			Margin = new Thickness(RegularFontPaddingLeftThermal, RegularFontPaddingTopThermal, RegularFontPaddingRightThermal, RegularFontPaddingBottomThermal),
			FontWeight = RegularFontWeightThermal,
			LineHeight = RegularFontSizeThermal,
			LineStackingStrategy = LineStackingStrategy.BlockLineHeight
		};
	}

	internal static Paragraph TableHeaderParagraph(string text)
	{
		return new(new Run(text))
		{
			TextAlignment = TextAlignment.Center,
			FontSize = TableHeaderFontSizeThermal,
			FontFamily = TableHeaderFontFamilyThermal,
			Margin = new Thickness(TableHeaderFontPaddingLeftThermal, TableHeaderFontPaddingTopThermal, TableHeaderFontPaddingRightThermal, TableHeaderFontPaddingBottomThermal),
			FontWeight = TableHeaderFontWeightThermal,
			LineHeight = TableHeaderFontSizeThermal,
			LineStackingStrategy = LineStackingStrategy.BlockLineHeight
		};
	}

	internal static Paragraph TableRowParagraph(string text)
	{
		return new(new Run(text))
		{
			FontSize = TableRowFontSizeThermal,
			FontFamily = TableRowFontFamilyThermal,
			Margin = new Thickness(TableRowFontPaddingLeftThermal, TableRowFontPaddingTopThermal, TableRowFontPaddingRightThermal, TableRowFontPaddingBottomThermal),
			FontWeight = TableRowFontWeightThermal,
			LineHeight = TableRowFontSizeThermal,
			LineStackingStrategy = LineStackingStrategy.BlockLineHeight
		};
	}

	internal static Paragraph FooterParagraph(string text, bool textCenter = false)
	{
		return new(new Run(text))
		{
			TextAlignment = textCenter ? TextAlignment.Center : TextAlignment.Left,
			FontSize = FooterFontSizeThermal,
			FontFamily = FooterFontFamilyThermal,
			Margin = new Thickness(FooterFontPaddingLeftThermal, FooterFontPaddingTopThermal, FooterFontPaddingRightThermal, FooterFontPaddingBottomThermal),
			FontWeight = FooterFontWeightThermal,
			LineHeight = FooterFontSizeThermal,
			LineStackingStrategy = LineStackingStrategy.BlockLineHeight
		};
	}

	internal static Paragraph SeparatorParagraph(bool textCenter = false)
	{
		return new Paragraph(new Run(new string('-', SeparatorDashCountThermal)))
		{
			TextAlignment = textCenter ? TextAlignment.Center : TextAlignment.Left,
			FontSize = SeparatorFontSizeThermal,
			FontFamily = SeparatorFontFamilyThermal,
			Margin = new Thickness(SeparatorFontPaddingLeftThermal, SeparatorFontPaddingTopThermal, SeparatorFontPaddingRightThermal, SeparatorFontPaddingBottomThermal),
			FontWeight = SeparatorFontWeightThermal,
			LineHeight = SeparatorFontSizeThermal,
			LineStackingStrategy = LineStackingStrategy.BlockLineHeight
		};
	}

	internal static void AddTableRow(TableRowGroup group, int itemNo, string itemName, int quantity, decimal rate, decimal amount)
	{
		group.Rows.Add(new TableRow
		{
			Cells =
			{
				new TableCell(TableRowParagraph(itemNo.ToString())),
				new TableCell(TableRowParagraph(itemName))
				{
					TextAlignment = TextAlignment.Left
				},
				new TableCell(TableRowParagraph(quantity.ToString())),
				new TableCell(TableRowParagraph(rate.ToString("N2")))
				{
					TextAlignment = TextAlignment.Right
				},
				new TableCell(TableRowParagraph(amount.ToString("N2")))
				{
					TextAlignment = TextAlignment.Right
				}
			}
		});
	}

	internal static void AddTableRow(TableRowGroup group, string label, string value)
	{
		group.Rows.Add(new TableRow
		{
			Cells =
			{
				new TableCell(RegularParagraph(label)),
				new TableCell(RegularParagraph(value))
				{
					TextAlignment = TextAlignment.Right
				}
			}
		});
	}
}
