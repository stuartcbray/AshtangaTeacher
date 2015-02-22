using System;
using Xamarin.Forms;
using Parse;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.ComponentModel;

namespace AshtangaTeacher.iOS
{
	public class ParseObjectBase : ViewModelBase, IParseObject
	{
		public event Action IsDirtyChanged = delegate {};

		bool isDirty, thumbIsDirty, hasImage;
		ImageSource image;

		protected ParseObject ParseObj;

		protected const string FieldUid = "uid";

		public bool IsDirty {
			get {
				return isDirty;
			}
			set {
				if (Set (() => IsDirty, ref isDirty, value))
					IsDirtyChanged ();
			}
		}

		public bool ThumbIsDirty {
			get {
				return thumbIsDirty;
			}
			set {
				Set (() => ThumbIsDirty, ref thumbIsDirty, value);
			}
		}

		public string ObjectId { 
			get {
				return ParseObj.ObjectId;
			}
		}

		public ImageSource Image {
			get {
				return image;
			}
			set {
				ThumbIsDirty = IsDirty |= Set (() => Image, ref image, value);
			}
		}

		public string UID { 
			get {
				return ParseObj.ContainsKey (FieldUid) ? ParseObj.Get<string> (FieldUid) : "";
			}
			set {
				ParseObj [FieldUid] = value;
				IsDirty = true;
				OnPropertyChanged ();
			}
		}

		public virtual async Task SaveAsync ()
		{
			if (ThumbIsDirty) {

				var deviceService = DependencyService.Get<IDeviceService> ();
				Byte[] data = await deviceService.GetBytesAsync (Image);

				ParseFile parseImg = new ParseFile(UID + ".PNG", data);

				try {
					await parseImg.SaveAsync ();
					ParseObj ["image"] = parseImg;
				} catch {
					// https://developers.facebook.com/bugs/789062014466095/
				}
				var cameraService = DependencyService.Get<ICameraService> ();
				deviceService.SaveToFile (data, cameraService.GetImagePath(UID));

				ThumbIsDirty = false;
			}

			try {
				await ParseObj.SaveAsync ();
			} catch {
				// https://developers.facebook.com/bugs/789062014466095/
			}

			IsDirty = false;
		}

		public async Task DeleteAsync ()
		{
			await ParseObj.DeleteAsync ();
		}

		protected async Task GetImageAsync()
		{
			Image = null;

			var cameraService = DependencyService.Get<ICameraService> ();
			var imgPath = cameraService.GetImagePath (UID);

			if (File.Exists (imgPath)) {
				var dt = File.GetLastWriteTimeUtc (imgPath);
				if (ParseObj.UpdatedAt != null && ParseObj.UpdatedAt <= dt) {
					Image = ImageSource.FromFile (imgPath);
					return;
				} 
			}

			// If it's not on disk, download and save the image to the local cache
			byte[] imageData = null;
			if (ParseObj.ContainsKey ("image")) {
				var parseImg = ParseObj.Get<ParseFile> ("image");
				imageData = await new HttpClient ().GetByteArrayAsync (parseImg.Url);
			} else if (ParseObj.ContainsKey ("facebookImageUrl")) {
				var url = ParseObj.Get<string> ("facebookImageUrl");
				imageData = await new HttpClient ().GetByteArrayAsync (url);
			}

			if (imageData != null) {
				var deviceService = DependencyService.Get<IDeviceService> ();
				deviceService.SaveToFile (imageData, imgPath);
				Image = ImageSource.FromStream (() => new MemoryStream (imageData));
			}
		}
			
		public virtual async Task InitializeAsync (object parseObject)
		{
			ParseObj = (ParseObject) parseObject;
			await GetImageAsync ();
			ThumbIsDirty = false;
			IsDirty = false;
		}
	}
}

