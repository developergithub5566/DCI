using QRCoder;

namespace DCI.Core.Helpers
{
	public class QRCodeGeneratorHelper
	{
		public byte[] GenerateQRCode(string text)
		{
			byte[] QRCode = new byte[0];
			if ((!string.IsNullOrEmpty(text)))
			{
				QRCodeGenerator qrCodeGen = new QRCodeGenerator();
				QRCodeData data = qrCodeGen.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
				BitmapByteQRCode bitmap = new BitmapByteQRCode(data);
				QRCode = bitmap.GetGraphic(20);
			}
			return QRCode;
		}
	}
}
