using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using AshtangaTeacher.iOS;

[assembly: Xamarin.Forms.Dependency (typeof (DeviceService))]

namespace AshtangaTeacher.iOS
{
	// Borrowed from http://msdn.microsoft.com/en-us/library/01escwtf(v=vs.110).aspx
	public class DeviceService : IDeviceService
	{

		public bool IsValidEmail(string strIn)
		{
			bool invalid = false;
			if (String.IsNullOrEmpty(strIn))
				return false;

			// Use IdnMapping class to convert Unicode domain names. 
			try {
				strIn = Regex.Replace(strIn, @"(@)(.+)$", 
					(match) => 
					{
						// IdnMapping class with default property values.
						IdnMapping idn = new IdnMapping();

						string domainName = match.Groups[2].Value;
						try {
							domainName = idn.GetAscii(domainName);
						}
						catch (ArgumentException) {
							invalid = true;
						}
						return match.Groups[1].Value + domainName;
					},
					RegexOptions.None, TimeSpan.FromMilliseconds(200));
			}
			catch (RegexMatchTimeoutException) {
				return false;
			}

			if (invalid)
				return false;

			// Return true if strIn is in valid e-mail format. 
			try {
				return Regex.IsMatch(strIn,
					@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
					@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
					RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
			}
			catch (RegexMatchTimeoutException) {
				return false;
			}
		}
			
		public async Task<Byte[]> GetBytesAsync (ImageSource imageSource)
		{
			var renderer = new StreamImagesourceHandler ();
			var image = await renderer.LoadImageAsync (imageSource);
			using (NSData pngData = image.AsPNG()) {
				Byte[] data = new Byte[pngData.Length];
				Marshal.Copy(pngData.Bytes, data, 0, Convert.ToInt32(pngData.Length));
				return data;
			}
		}

		public bool SaveToFile (byte[] data, string filePath)
		{
			var nsData = NSData.FromArray (data);
			NSError err;
			if (nsData.Save (filePath, false, out err)) {
				Console.WriteLine ("Saved file " + filePath);
				return true;
			}

			Console.WriteLine ("NOT saved as " + filePath + " because" + err.LocalizedDescription);
			return false;
		}

	}
}

