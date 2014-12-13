using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Parse;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using System.Net.Http;

namespace AshtangaTeacher.iOS
{
	public abstract class User : ViewModelBase
	{
		bool isDirty, thumbIsDirty;
		ImageSource image;

		protected ParseObject ParseObj = null;

		const string Field_UID = "uid";
		const string Field_Name = "name";
		protected const string FieldShalaName = "shalaName";
		protected const string FieldShalaNameLC = "shalaNameLC";

		public bool IsDirty {
			get {
				return isDirty;
			}
			set {
				Set (() => IsDirty, ref isDirty, value);
			}
		}


		string shalaName;
		public string ShalaName {
			get {
				return shalaName;
			}
			set {
				IsDirty |= Set (() => ShalaName, ref shalaName, value);
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

		public string UID { 
			get {
				return ParseObj.ContainsKey (Field_UID) ? ParseObj.Get<string> (Field_UID) : "";
			}
			set {
				ParseObj [Field_UID] = value;
				IsDirty = true;
				OnPropertyChanged ();
			}
		}

		public object UserObj { 
			get {
				return ParseObj;
			}
			set {
				this.ParseObj = (ParseObject) value;
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

		public string Name {
			get {
				return ParseObj.ContainsKey (Field_Name) ? ParseObj.Get<string> (Field_Name) : "";
			}
			set {
				if (Name != value) {
					ParseObj [Field_Name] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
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

		public virtual async Task InitializeAsync (object userObj)
		{
			UserObj = userObj;

			if (ParseObj.ContainsKey (FieldShalaName))
				ShalaName = ParseObj.Get<string> (FieldShalaName);

			await GetImageAsync ();
			ThumbIsDirty = false;
			IsDirty = false;
		}
	}
}

