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
		const string Field_ShalaName = "shalaName";
		const string Field_ShalaNameLC = "shalaNameLC";

		public bool IsDirty {
			get {
				return isDirty;
			}
			set {
				isDirty = value;
				OnPropertyChanged ();
			}
		}

		public bool ThumbIsDirty {
			get {
				return thumbIsDirty;
			}
			set {
				thumbIsDirty = value;
				OnPropertyChanged ();
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
				image = value;
				ThumbIsDirty = true;
				IsDirty = true;
				OnPropertyChanged ("Image");
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
						
		public string ShalaName {
			get {
				return ParseObj.ContainsKey (Field_ShalaName) ? ParseObj.Get<string> (Field_ShalaName) : "";
			}
			set {
				if (ShalaName != value) {
					ParseObj [Field_ShalaName] = value;
					ParseObj [Field_ShalaNameLC] = value.ToLower ();
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		protected async Task GetImageAsync()
		{
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

		public async Task SaveAsync ()
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
			await GetImageAsync ();
			ThumbIsDirty = false;
			IsDirty = false;
		}
	}
}

