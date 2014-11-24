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

		public async Task SaveAsync ()
		{
			if (ThumbIsDirty) {
				await SaveThumb (Image, TeacherId);
				ThumbIsDirty = false;
			}
			await parseUser.SaveAsync ();
			IsDirty = false;
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
					await SaveAsync ();
				} catch {
					// https://developers.facebook.com/bugs/789062014466095/
				}

				var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();
				var deviceService = ServiceLocator.Current.GetInstance<IDeviceService> ();
				deviceService.SaveToFile (data, cameraService.GetImagePath(id));
			}
		}

		public Teacher() 
		{
			parseUser = new ParseUser ();
		}
	}
}
