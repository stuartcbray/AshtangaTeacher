using System;
using CoreGraphics;
using System.Threading.Tasks;
using UIKit;
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
			nfloat edgeLength = (nfloat) Math.Min (img.Size.Width, img.Size.Height);

			if (IsRetina)
				UIGraphics.BeginImageContextWithOptions(new CGSize(edgeLength, edgeLength), false, 2.0f);
			else
				UIGraphics.BeginImageContext(new CGSize(edgeLength, edgeLength));

			var context = UIGraphics.GetCurrentContext();

			var clippedRect = new CGRect(0, 0, edgeLength, edgeLength);
			context.ClipToRect(clippedRect);
			var drawRect = new CGRect(0, 0, img.Size.Width, img.Size.Height);
			img.Draw(drawRect);
			var squareImg = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			// Now resize to thumb 
			var resizeFactor = Math.Min(maxWidth / squareImg.Size.Width, maxHeight / squareImg.Size.Height);

			if (resizeFactor > 1) { 
				return squareImg;
			}

			nfloat width = (nfloat) resizeFactor * squareImg.Size.Width;
			nfloat height = (nfloat) resizeFactor * squareImg.Size.Height;

			if (IsRetina)
				UIGraphics.BeginImageContextWithOptions(new CGSize(width, height), false, 2.0f);
			else
				UIGraphics.BeginImageContext(new CGSize(width, height));

			squareImg.Draw(new CGRect(0, 0, width, height));
			var thumbImg = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return thumbImg;
		}
	}
}

