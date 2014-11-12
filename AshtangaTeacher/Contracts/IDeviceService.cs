using Xamarin.Forms;

namespace AshtangaTeacher
{
	public interface IDeviceService
	{
		bool IsValidEmail (string strIn);
		bool SaveToFile (byte[] data, string fileName);
	}
}

