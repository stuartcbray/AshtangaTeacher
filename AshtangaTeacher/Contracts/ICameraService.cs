using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public interface ICameraService
	{
		Task<ImageSource> GetThumbAsync (ImageSource imgSrc, string fileName);

		string GetImagePath (string id);
	}
}