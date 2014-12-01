using System;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;
using AshtangaTeacher.iOS;
using Parse;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.iOS;
using MonoTouch.Foundation;
using System.Runtime.InteropServices;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;

[assembly: Xamarin.Forms.Dependency (typeof (Teacher))]

namespace AshtangaTeacher.iOS
{
	public class Teacher : ViewModelBasee, ITeacher
	{
		ParseUser parseUser;
		bool thumbIsDirty, isDirty;
		ImageSource image;

		const string Field_TeacherId = "teacherId";
		const string Field_Role = "role";
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

		public ImageSource Image {
			get {
				return image;
			}
			set {
				image = value;
				ThumbIsDirty = true;
				IsDirty = true;
				OnPropertyChanged ();
			}
		}
			
		public string Name {
			get {
				return parseUser.ContainsKey (Field_Name) ? parseUser.Get<string> (Field_Name) : "";
			}
			set {
				if (Name != value) {
					parseUser[Field_Name] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public string UserName {
			get {
				return parseUser.Username ?? "";
			}
			set {
				if (UserName != value) {
					parseUser.Username = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public string Email {
			get {
				return parseUser.Email ?? "";
			}
			set {
				if (parseUser.Email != value) {
					parseUser.Email = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public string Password {
			set {
				parseUser.Password = value;
				IsDirty = true;
				OnPropertyChanged ();
			}
		}

		public string ShalaName {
			get {
				return parseUser.ContainsKey (Field_ShalaName) ? parseUser.Get<string> (Field_ShalaName) : "";
			}
			set {
				if (ShalaName != value) {
					parseUser [Field_ShalaName] = value;
					parseUser [Field_ShalaNameLC] = value.ToLower ();
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public string TeacherId { 
			get {
				return parseUser.ContainsKey (Field_TeacherId) ? parseUser.Get<string> (Field_TeacherId) : "";
			}
			set {
				parseUser [Field_TeacherId] = value;
			}
		}

		public TeacherRole Role {
			get {
				return parseUser.ContainsKey (Field_Role) ? 
					(TeacherRole)parseUser.Get<long> (Field_Role) : TeacherRole.None;
			}
			set {
				parseUser [Field_Role] = (long) value;
				OnPropertyChanged ();
			}
		}
			
		public object UserObj { 
			get {
				return parseUser;
			}
			set {
				this.parseUser = (ParseUser) value;
			}
		}

		public string ObjectId { 
			get {
				return parseUser.ObjectId;
			}
		}

		public async Task<bool> ShalaExistsAsync (string name)
		{
			var query = ParseUser.Query.WhereEqualTo("shalaNameLC", name.ToLower());
			IEnumerable<ParseObject> results = await query.FindAsync ();
			return results.Any ();
		}

		public async Task SaveAsync ()
		{
			if (ThumbIsDirty) {
				await SaveThumb (Image, TeacherId);
				ThumbIsDirty = false;
			}

			try {
				await parseUser.SaveAsync ();
			} catch {
				// https://developers.facebook.com/bugs/789062014466095/
			}
	
			IsDirty = false;
		}

		public async Task InitializeAsync (object userObj)
		{
			this.UserObj = userObj;
			await GetImageAsync ();

			// Raise a PropertyChanged for the visible properties so they show up in the UI
			OnPropertyChanged ("ShalaName");
			OnPropertyChanged ("Email");
			OnPropertyChanged ("Name");
			OnPropertyChanged ("UserName");
			OnPropertyChanged ("Role");

			IsDirty = false;
			ThumbIsDirty = false;
		}

		async Task GetImageAsync()
		{
			var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();
			var imgPath = cameraService.GetImagePath (TeacherId);

			if (File.Exists (imgPath)) {
				var dt = File.GetLastWriteTimeUtc (imgPath);
				if (parseUser.UpdatedAt != null && parseUser.UpdatedAt <= dt) {
					Image = ImageSource.FromFile (imgPath);
					return;
				} 
			}

			// If it's not on disk, download and save the image to the local cache
			byte[] imageData = null;
			if (parseUser.ContainsKey ("image")) {
				var parseImg = parseUser.Get<ParseFile> ("image");
				imageData = await new HttpClient ().GetByteArrayAsync (parseImg.Url);
			} else if (parseUser.ContainsKey ("facebookImageUrl")) {
				var url = parseUser.Get<string> ("facebookImageUrl");
				imageData = await new HttpClient ().GetByteArrayAsync (url);
			}

			if (imageData != null) {
				var deviceService = ServiceLocator.Current.GetInstance<IDeviceService> ();
				deviceService.SaveToFile (imageData, imgPath);
				Image = ImageSource.FromStream (() => new MemoryStream (imageData));
			}
		}
			
		async Task SaveThumb(ImageSource imageSource, string id)
		{
			var renderer = new StreamImagesourceHandler ();
			var image = await renderer.LoadImageAsync (imageSource);
			using (NSData pngData = image.AsPNG()) {

				Byte[] data = new Byte[pngData.Length];
				Marshal.Copy(pngData.Bytes, data, 0, Convert.ToInt32(pngData.Length));

				ParseFile parseImg = new ParseFile(id + ".PNG", data);

				try {
					await parseImg.SaveAsync ();
					parseUser ["image"] = parseImg;
					await parseUser.SaveAsync ();
				} catch {
					// https://developers.facebook.com/bugs/789062014466095/
				}
					
				var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();
				var deviceService = ServiceLocator.Current.GetInstance<IDeviceService> ();
				deviceService.SaveToFile (data, cameraService.GetImagePath(id));
			}
		}

		public async Task UpdateRoleAsync(TeacherRole role)
		{
			parseUser ["role"] = (long)role;
			await SaveAsync ();
				
			var parseRole = await ParseRole.Query.Where (x => x.Name == role.ToString()).FirstOrDefaultAsync ();
			parseRole.Users.Add (parseUser);
			await parseRole.SaveAsync ();
		}

		public async Task UpdatePropertyAsync<T> (string name, T value)
		{
			parseUser [name] = value;
			await SaveAsync ();
		}

		public Teacher() 
		{
			parseUser = new ParseUser ();
		}
	}
}
