using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using MonoTouch.Foundation;
using System.Threading.Tasks;
using MonoTouch.UIKit;
using System.Drawing;
using Microsoft.Practices.ServiceLocation;

namespace AshtangaTeacher.iOS
{
	public class CameraService : ICameraService
	{

		bool IsRetina {
			get {
				return UIScreen.MainScreen.Scale > 1.0;
			}
		}

		public async Task<string> SaveThumbAsync (ImageSource img, string fileName)
		{
			var renderer = new StreamImagesourceHandler ();
			var image = await renderer.LoadImageAsync (img);

			var thumb = CreateThumb (image, 80, 80);

			// Save a local copy
			var docFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string pngFile = System.IO.Path.Combine (docFolder, fileName + ".PNG");

			NSData thumbData = thumb.AsPNG ();
			NSError err;
			if (thumbData.Save (pngFile, false, out err)) {
				Console.WriteLine ("Saved file " + pngFile);
			} else {
				Console.WriteLine ("NOT saved as " + pngFile + " because" + err.LocalizedDescription);
			}

			return pngFile;
		}

		public string GetImagePath (string id)
		{
			var docFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			return System.IO.Path.Combine (docFolder, id + ".PNG"); 
		}

		UIImage CreateThumb(UIImage img, float maxWidth, float maxHeight)
		{
			// First Create the square image
			var edgeLength = Math.Min (img.Size.Width, img.Size.Height);

			if (IsRetina)
				UIGraphics.BeginImageContextWithOptions(new SizeF(edgeLength, edgeLength), false, 2.0f);
			else
				UIGraphics.BeginImageContext(new SizeF(edgeLength, edgeLength));

			var context = UIGraphics.GetCurrentContext();

			var clippedRect = new RectangleF(0, 0, edgeLength, edgeLength);
			context.ClipToRect(clippedRect);
			var drawRect = new RectangleF(0, 0, img.Size.Width, img.Size.Height);
			img.Draw(drawRect);
			var squareImg = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			// Now resize to thumb 
			var resizeFactor = Math.Min(maxWidth / squareImg.Size.Width, maxHeight / squareImg.Size.Height);

			if (resizeFactor > 1) { 
				return squareImg;
			}

			var width = resizeFactor * squareImg.Size.Width;
			var height = resizeFactor * squareImg.Size.Height;

			if (IsRetina)
				UIGraphics.BeginImageContextWithOptions(new SizeF(width, height), false, 2.0f);
			else
				UIGraphics.BeginImageContext(new SizeF(width, height));

			squareImg.Draw(new RectangleF(0, 0, width, height));
			var thumbImg = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return thumbImg;
		}
	}
}

