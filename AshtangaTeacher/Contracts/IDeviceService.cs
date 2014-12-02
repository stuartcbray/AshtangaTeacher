using Xamarin.Forms;
using System.Threading.Tasks;
using System;

namespace AshtangaTeacher
{
	public interface IDeviceService
	{
		bool IsValidEmail (string strIn);
		bool SaveToFile (byte[] data, string fileName);
		Task<Byte[]> GetBytesAsync (ImageSource imageSource);
	}
}

