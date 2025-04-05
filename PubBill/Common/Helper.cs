using System.Globalization;

namespace PubBill;

public static class Helper
{
	public static String RemoveSpace(this String str) => str.Replace(" ", "");

	public static String FormatIndianCurrency(this Decimal rate)
	{
		var hindi = new CultureInfo("hi-IN");
		return string.Format(hindi, "{0:C}", rate);
	}
}
