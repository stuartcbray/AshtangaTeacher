using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using MonoTouch.Foundation;
using System.Threading.Tasks;

namespace AshtangaTeacher.iOS
{
	public class CameraService : ICameraService
	{
		public async Task SaveImageAsync (ImageSource img, string fileName)
		{
			var renderer = new StreamImagesourceHandler ();
			var image = await renderer.LoadImageAsync (img);

			var docFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string pngFile = System.IO.Path.Combine (docFolder, fileName + ".PNG");

			NSData imgData = image.AsPNG ();
			NSError err;
			if (imgData.Save (pngFile, false, out err)) {
				Console.WriteLine ("Saved file " + pngFile);
			} else {
				Console.WriteLine ("NOT saved as " + pngFile + " because" + err.LocalizedDescription);
			}
		}

		public string GetImagePath (string fileName)
		{
			var docFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			return System.IO.Path.Combine (docFolder, fileName);
		}
	}
}

