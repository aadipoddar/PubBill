using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

using MessagingToolkit.QRCode.Codec;

namespace PubBill.Billing.Bill.Printing;

internal static class ThermalReceipt
{
	internal static FlowDocument Print(decimal amount)
	{
		// Create a FlowDocument for the receipt
		FlowDocument document = new()
		{
			PageWidth = 280,
			PagePadding = new Thickness(20, 20, 20, 20),
			ColumnWidth = double.MaxValue
		};

		// Generate the UPI ID
		string upiId = amount == 0
			? "upi://pay?pa=aadipoddarmail@okaxis&pn=Aadi%20Poddar&aid=uGICAgIDfjJ3pCw"
			: $"upi://pay?pa=aadipoddarmail@okaxis&pn=Aadi%20Poddar&am={amount}&cu=INR&aid=uGICAgIDfjJ3pCw";

		// Generate the QR code
		QRCodeEncoder encoder = new();
		Bitmap bitmap = encoder.Encode(upiId);

		// Convert the Bitmap to a BitmapImage for WPF
		BitmapImage qrCodeImage = ConvertBitmapToBitmapImage(bitmap);

		// Add the QR code to the FlowDocument
		var image = new System.Windows.Controls.Image
		{
			Source = qrCodeImage,
			Width = 200,
			Height = 200,
			Margin = new Thickness(0, 10, 0, 10)
		};

		var block = new BlockUIContainer(image);
		document.Blocks.Add(block);

		return document;
	}

	private static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
	{
		using MemoryStream memory = new();
		bitmap.Save(memory, ImageFormat.Png);
		memory.Position = 0;

		BitmapImage bitmapImage = new();
		bitmapImage.BeginInit();
		bitmapImage.StreamSource = memory;
		bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
		bitmapImage.EndInit();
		bitmapImage.Freeze();
		return bitmapImage;
	}
}
