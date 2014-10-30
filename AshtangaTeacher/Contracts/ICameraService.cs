using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public interface ICameraService
	{
		Task SaveImageAsync (ImageSource imgSrc, string fileName);

		string GetImagePath (string fileName);
	}
}