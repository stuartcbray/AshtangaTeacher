using System;
using System.Drawing;
using System.Threading.Tasks;
using MonoTouch.UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using AshtangaTeacher.iOS;

[assembly: Xamarin.Forms.Dependency (typeof (CameraService))]

namespace AshtangaTeacher.iOS
{
	public class CameraService : ICameraService
	{

		bool IsRetina {
			get {
				return UIScreen.MainScreen.Scale > 1.0;
			}
		}

		public async Task<ImageSource> GetThumbAsync (ImageSource img, string fileName)
		{
			var renderer = new StreamImagesourceHandler ();
			var image = await renderer.LoadImageAsync (img);
			var thumb = CreateThumb (image, 100, 100);
			return ImageSource.FromStream(() => thumb.AsPNG().AsStream());
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

