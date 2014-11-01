using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public interface ICameraService
	{
		Task<string> SaveThumbAsync (ImageSource imgSrc, string fileName);

		string GetImagePath (string id);
	}
}