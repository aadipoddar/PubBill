using System.Windows;
using System.Windows.Media;

namespace PubBill.Common;

internal static class SettingsManager
{
	internal static async Task LoadSettings()
	{
		var resources = Application.Current.Resources;

		resources[SettingsKeys.PubOpenTime] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.PubOpenTime));
		resources[SettingsKeys.PubCloseTime] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.PubCloseTime));

		resources[SettingsKeys.InactivityTime] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.InactivityTime));
		resources[SettingsKeys.RefreshBillTimer] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.RefreshBillTimer));
		resources[SettingsKeys.RefreshKOTTimer] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.RefreshKOTTimer));
		resources[SettingsKeys.RefreshReportTimer] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.RefreshReportTimer));

		resources[SettingsKeys.PageWidthThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.PageWidthThermal));
		resources[SettingsKeys.PagePaddingTopThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.PagePaddingTopThermal));
		resources[SettingsKeys.PagePaddingBottomThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.PagePaddingBottomThermal));
		resources[SettingsKeys.PagePaddingLeftThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.PagePaddingLeftThermal));
		resources[SettingsKeys.PagePaddingRightThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.PagePaddingRightThermal));

		resources[SettingsKeys.HeaderFontFamilyThermal] = new FontFamily(await SettingsData.LoadSettingsByKey(SettingsKeys.HeaderFontFamilyThermal));
		resources[SettingsKeys.HeaderFontSizeThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.HeaderFontSizeThermal));
		resources[SettingsKeys.HeaderFontWeightThermal] = (FontWeight)new FontWeightConverter().ConvertFromString(await SettingsData.LoadSettingsByKey(SettingsKeys.HeaderFontWeightThermal));
		resources[SettingsKeys.HeaderFontPaddingTopThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.HeaderFontPaddingTopThermal));
		resources[SettingsKeys.HeaderFontPaddingBottomThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.HeaderFontPaddingBottomThermal));
		resources[SettingsKeys.HeaderFontPaddingLeftThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.HeaderFontPaddingLeftThermal));
		resources[SettingsKeys.HeaderFontPaddingRightThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.HeaderFontPaddingRightThermal));

		resources[SettingsKeys.SubHeaderFontFamilyThermal] = new FontFamily(await SettingsData.LoadSettingsByKey(SettingsKeys.SubHeaderFontFamilyThermal));
		resources[SettingsKeys.SubHeaderFontSizeThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.SubHeaderFontSizeThermal));
		resources[SettingsKeys.SubHeaderFontWeightThermal] = (FontWeight)new FontWeightConverter().ConvertFromString(await SettingsData.LoadSettingsByKey(SettingsKeys.SubHeaderFontWeightThermal));
		resources[SettingsKeys.SubHeaderFontPaddingTopThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.SubHeaderFontPaddingTopThermal));
		resources[SettingsKeys.SubHeaderFontPaddingBottomThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.SubHeaderFontPaddingBottomThermal));
		resources[SettingsKeys.SubHeaderFontPaddingLeftThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.SubHeaderFontPaddingLeftThermal));
		resources[SettingsKeys.SubHeaderFontPaddingRightThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.SubHeaderFontPaddingRightThermal));

		resources[SettingsKeys.RegularFontFamilyThermal] = new FontFamily(await SettingsData.LoadSettingsByKey(SettingsKeys.RegularFontFamilyThermal));
		resources[SettingsKeys.RegularFontSizeThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.RegularFontSizeThermal));
		resources[SettingsKeys.RegularFontWeightThermal] = (FontWeight)new FontWeightConverter().ConvertFromString(await SettingsData.LoadSettingsByKey(SettingsKeys.RegularFontWeightThermal));
		resources[SettingsKeys.RegularFontPaddingTopThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.RegularFontPaddingTopThermal));
		resources[SettingsKeys.RegularFontPaddingBottomThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.RegularFontPaddingBottomThermal));
		resources[SettingsKeys.RegularFontPaddingLeftThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.RegularFontPaddingLeftThermal));
		resources[SettingsKeys.RegularFontPaddingRightThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.RegularFontPaddingRightThermal));

		resources[SettingsKeys.TableHeaderFontFamilyThermal] = new FontFamily(await SettingsData.LoadSettingsByKey(SettingsKeys.TableHeaderFontFamilyThermal));
		resources[SettingsKeys.TableHeaderFontSizeThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.TableHeaderFontSizeThermal));
		resources[SettingsKeys.TableHeaderFontWeightThermal] = (FontWeight)new FontWeightConverter().ConvertFromString(await SettingsData.LoadSettingsByKey(SettingsKeys.TableHeaderFontWeightThermal));
		resources[SettingsKeys.TableHeaderFontPaddingTopThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.TableHeaderFontPaddingTopThermal));
		resources[SettingsKeys.TableHeaderFontPaddingBottomThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.TableHeaderFontPaddingBottomThermal));
		resources[SettingsKeys.TableHeaderFontPaddingLeftThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.TableHeaderFontPaddingLeftThermal));
		resources[SettingsKeys.TableHeaderFontPaddingRightThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.TableHeaderFontPaddingRightThermal));

		resources[SettingsKeys.TableRowFontFamilyThermal] = new FontFamily(await SettingsData.LoadSettingsByKey(SettingsKeys.TableRowFontFamilyThermal));
		resources[SettingsKeys.TableRowFontSizeThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.TableRowFontSizeThermal));
		resources[SettingsKeys.TableRowFontWeightThermal] = (FontWeight)new FontWeightConverter().ConvertFromString(await SettingsData.LoadSettingsByKey(SettingsKeys.TableRowFontWeightThermal));
		resources[SettingsKeys.TableRowFontPaddingTopThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.TableRowFontPaddingTopThermal));
		resources[SettingsKeys.TableRowFontPaddingBottomThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.TableRowFontPaddingBottomThermal));
		resources[SettingsKeys.TableRowFontPaddingLeftThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.TableRowFontPaddingLeftThermal));
		resources[SettingsKeys.TableRowFontPaddingRightThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.TableRowFontPaddingRightThermal));

		resources[SettingsKeys.FooterFontFamilyThermal] = new FontFamily(await SettingsData.LoadSettingsByKey(SettingsKeys.FooterFontFamilyThermal));
		resources[SettingsKeys.FooterFontSizeThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.FooterFontSizeThermal));
		resources[SettingsKeys.FooterFontWeightThermal] = (FontWeight)new FontWeightConverter().ConvertFromString(await SettingsData.LoadSettingsByKey(SettingsKeys.FooterFontWeightThermal));
		resources[SettingsKeys.FooterFontPaddingTopThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.FooterFontPaddingTopThermal));
		resources[SettingsKeys.FooterFontPaddingBottomThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.FooterFontPaddingBottomThermal));
		resources[SettingsKeys.FooterFontPaddingLeftThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.FooterFontPaddingLeftThermal));
		resources[SettingsKeys.FooterFontPaddingRightThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.FooterFontPaddingRightThermal));

		resources[SettingsKeys.SeparatorFontFamilyThermal] = new FontFamily(await SettingsData.LoadSettingsByKey(SettingsKeys.SeparatorFontFamilyThermal));
		resources[SettingsKeys.SeparatorFontSizeThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.SeparatorFontSizeThermal));
		resources[SettingsKeys.SeparatorFontWeightThermal] = (FontWeight)new FontWeightConverter().ConvertFromString(await SettingsData.LoadSettingsByKey(SettingsKeys.SeparatorFontWeightThermal));
		resources[SettingsKeys.SeparatorFontPaddingTopThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.SeparatorFontPaddingTopThermal));
		resources[SettingsKeys.SeparatorFontPaddingBottomThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.SeparatorFontPaddingBottomThermal));
		resources[SettingsKeys.SeparatorFontPaddingLeftThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.SeparatorFontPaddingLeftThermal));
		resources[SettingsKeys.SeparatorFontPaddingRightThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.SeparatorFontPaddingRightThermal));
		resources[SettingsKeys.SeparatorDashCountThermal] = int.Parse(await SettingsData.LoadSettingsByKey(SettingsKeys.SeparatorDashCountThermal));

		resources[SettingsKeys.HeaderLine1] = await SettingsData.LoadSettingsByKey(SettingsKeys.HeaderLine1);
		resources[SettingsKeys.HeaderLine2] = await SettingsData.LoadSettingsByKey(SettingsKeys.HeaderLine2);
		resources[SettingsKeys.HeaderLine3] = await SettingsData.LoadSettingsByKey(SettingsKeys.HeaderLine3);

		resources[SettingsKeys.FooterLine] = await SettingsData.LoadSettingsByKey(SettingsKeys.FooterLine);

		resources[SettingsKeys.UPIId] = await SettingsData.LoadSettingsByKey(SettingsKeys.UPIId);
	}
}
