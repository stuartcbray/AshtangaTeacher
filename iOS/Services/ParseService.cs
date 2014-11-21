using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using MonoTouch.Foundation;
using Parse;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace AshtangaTeacher.iOS
{
	public class ParseService : IParseService
	{

		bool rolesInitialized;
		const string AdminRole = "Administrator";
		const string ModeratorRole = "Moderator";


		public string CurrentUserName { 
			get {
				if (ParseUser.CurrentUser != null)
					return ParseUser.CurrentUser.Username;
				return null;
			} 
		}

		public async Task UpdateUserPropertyAsync(string name, string value)
		{
			ParseUser.CurrentUser [name] = value;
			try {
				await ParseUser.CurrentUser.SaveAsync ();
			} catch {
				// https://developers.facebook.com/bugs/789062014466095/
			}
		}

		public async Task MakeUserAdminAsync ()
		{
			var adminRole = await GetRoleAsync (AdminRole);
			adminRole.Users.Add (ParseUser.CurrentUser);
			await adminRole.SaveAsync ();
		}

		public string ShalaName {
			get {
				if (ParseUser.CurrentUser != null && ParseUser.CurrentUser.ContainsKey("shalaName")) {
					return ParseUser.CurrentUser.Get<string> ("shalaName");
				}
				return null;
			}
		}

		Teacher currentTeacher;
		public async Task<Teacher> GetTeacherAsync() 
		{

			if (ParseUser.CurrentUser != null) {
				if (currentTeacher == null) {
					currentTeacher = new Teacher {
						ShalaName = ParseUser.CurrentUser.Get<string> ("shalaName"),
						Name = ParseUser.CurrentUser.Get<string> ("name"),
						TeacherId = ParseUser.CurrentUser.Get<string> ("teacherId"),
						Email = ParseUser.CurrentUser.Email,
						UserName = ParseUser.CurrentUser.Username,
						ObjectId = ParseUser.CurrentUser.ObjectId
					};

					var adminRole = await GetRoleAsync (AdminRole);
					var modsRole = await GetRoleAsync (ModeratorRole);

					var users = await adminRole.Users.Query.FindAsync ();
					if (users != null && users.Any(x => x.ObjectId == currentTeacher.ObjectId)) {
						currentTeacher.Role = TeacherRole.Administrator;
					} else {
						users = await modsRole.Users.Query.FindAsync ();
						if (users != null && users.Any(x => x.ObjectId == currentTeacher.ObjectId))
							currentTeacher.Role = TeacherRole.Moderator;
					}

					// Try the local cache first
					var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();
					var imgPath = cameraService.GetImagePath (currentTeacher.TeacherId);

					bool fetchImage = true;
					if (File.Exists (imgPath)) {
						var dt = File.GetLastWriteTimeUtc (imgPath);
						if (ParseUser.CurrentUser.UpdatedAt != null && ParseUser.CurrentUser.UpdatedAt <= dt) {
							currentTeacher.Image = ImageSource.FromFile (imgPath);
							fetchImage = false;
						} 
					}

					if (fetchImage) {

						byte[] imageData = null;
						if (ParseUser.CurrentUser.ContainsKey ("image")) {
							var parseImg = ParseUser.CurrentUser.Get<ParseFile> ("image");
							imageData = await new HttpClient ().GetByteArrayAsync (parseImg.Url);
						} else if (ParseUser.CurrentUser.ContainsKey ("facebookImageUrl")) {
							var url = ParseUser.CurrentUser.Get<string> ("facebookImageUrl");
							imageData = await new HttpClient ().GetByteArrayAsync (url);
						}

						if (imageData != null) {
							currentTeacher.Image = ImageSource.FromStream (() => new MemoryStream (imageData));

							var deviceService = ServiceLocator.Current.GetInstance<IDeviceService> ();
							deviceService.SaveToFile (imageData, imgPath);
						}
					}
				}
			}
			return currentTeacher;
		}

		public async Task<bool> ShalaNameExists (string name)
		{
			var query = ParseUser.Query.WhereEqualTo("shalaNameLC", name.ToLower());
			IEnumerable<ParseObject> results = await query.FindAsync ();
			return results.Any ();
		}

		public async Task SignUpAsync (Teacher teacher, bool shalaExists)
		{
			var user = new ParseUser () {
				// username is the same as Email
				Username = teacher.UserName,
				Password = teacher.Password,
				Email = teacher.Email
			};
					
			user ["shalaName"] = teacher.ShalaName;
			user ["shalaNameLC"] = teacher.ShalaName.ToLower ();
			user ["name"] = teacher.Name;
			user ["teacherId"] = teacher.TeacherId;
			user ["role"] = (long) teacher.Role;

			await user.SignUpAsync ();

			if (!shalaExists) {
				var adminRole = await GetRoleAsync(AdminRole);
				adminRole.Users.Add (user);
				await adminRole.SaveAsync ();
			} 
		}

		public void Initialize (string appId, string key, string facebookAppId)
		{
			ParseClient.Initialize (appId, key);
			ParseFacebookUtils.Initialize (facebookAppId);
		}

		public bool ShowLogin ()
		{
			return ParseUser.CurrentUser == null;
		}

		public async Task SaveTeacherAsync (Teacher teacher)
		{
			ParseUser.CurrentUser ["shalaName"] = teacher.ShalaName;
			ParseUser.CurrentUser ["shalaNameLC"] = teacher.ShalaName.ToLower ();
			ParseUser.CurrentUser ["name"] = teacher.Name;
			ParseUser.CurrentUser ["teacherId"] = teacher.TeacherId;
			ParseUser.CurrentUser ["role"] = (long) teacher.Role;
			ParseUser.CurrentUser.Email = teacher.Email;
			ParseUser.CurrentUser.Username = teacher.UserName;

			if (teacher.ThumbIsDirty) {
				await SaveTeacherThumb (teacher);
			}
			await ParseUser.CurrentUser.SaveAsync ();
			teacher.IsDirty = false;
		}

		async Task SaveTeacherThumb(Teacher teacher)
		{
			var renderer = new StreamImagesourceHandler ();
			var image = await renderer.LoadImageAsync (teacher.Image);
			using (NSData pngData = image.AsPNG()) {

				Byte[] data = new Byte[pngData.Length];
				Marshal.Copy(pngData.Bytes, data, 0, Convert.ToInt32(pngData.Length));

				ParseFile parseImg = new ParseFile(teacher.TeacherId + ".PNG", data);

				try {
					await parseImg.SaveAsync ();
					ParseUser.CurrentUser ["image"] = parseImg;
					await ParseUser.CurrentUser.SaveAsync ();
				} catch {
					// https://developers.facebook.com/bugs/789062014466095/
				}

				var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();
				var deviceService = ServiceLocator.Current.GetInstance<IDeviceService> ();
				deviceService.SaveToFile (data, cameraService.GetImagePath(teacher.TeacherId));
			}
			teacher.ThumbIsDirty = false;
		}

		public async Task LogOutAsync ()
		{
			currentTeacher = null;
			await Task.Run (() => ParseUser.LogOut ());
		}

		public async Task AddUserToRole (string objectId, string roleName)
		{
			var user = await ParseUser.Query.GetAsync (objectId);
			if (user != null) {
				var role = await ParseRole.Query.Where (x => x.Name == roleName).FirstOrDefaultAsync ();
				role.Users.Add (user);
				await role.SaveAsync ();
			}
		}

		public async Task SignInAsync (string username, string password)
		{
			await ParseUser.LogInAsync (username, password);
		}

		public async Task<List<Teacher>> GetTeachers ()
		{
			var query = ParseUser.Query.Where (teacher => teacher.Get<string> ("shalaNameLC") == currentTeacher.ShalaName.ToLower ());
			IEnumerable<ParseUser> results = await query.FindAsync();

			var adminRole = await GetRoleAsync (AdminRole);
			var modsRole = await GetRoleAsync (ModeratorRole);

			var teachers = new List<Teacher> ();

			foreach (var t in results) {

				var teacher = new Teacher {
					ShalaName = t.Get<string> ("shalaName"),
					Name = t.Get<string> ("name"),
					TeacherId = t.Get<string> ("teacherId"),
					Email = t.Email,
					UserName = t.Username,
					ObjectId = t.ObjectId
				};
						
				var users = await adminRole.Users.Query.FindAsync ();
				if (users != null && users.Any(x => x.ObjectId == t.ObjectId)) {
					teacher.Role = TeacherRole.Administrator;
				} else {
					users = await modsRole.Users.Query.FindAsync ();
					if (users != null && users.Any(x => x.ObjectId == t.ObjectId))
						teacher.Role = TeacherRole.Moderator;
				}

				// Try the local cache first
				var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();
				var imgPath = cameraService.GetImagePath (teacher.TeacherId);

				bool fetchImage = true;
				if (File.Exists (imgPath)) {
					var dt = File.GetLastWriteTimeUtc (imgPath);
					if (t.UpdatedAt != null && t.UpdatedAt <= dt) {
						teacher.Image = ImageSource.FromFile (imgPath);
						fetchImage = false;
					} 
				}

				if (fetchImage) {

					byte[] imageData = null;
					if (t.ContainsKey ("image")) {
						var parseImg = t.Get<ParseFile> ("image");
						imageData = await new HttpClient ().GetByteArrayAsync (parseImg.Url);
					} else if (t.ContainsKey ("facebookImageUrl")) {
						var url = t.Get<string> ("facebookImageUrl");
						imageData = await new HttpClient ().GetByteArrayAsync (url);
					}

					if (imageData != null) {
						teacher.Image = ImageSource.FromStream (() => new MemoryStream (imageData));

						var deviceService = ServiceLocator.Current.GetInstance<IDeviceService> ();
						deviceService.SaveToFile (imageData, imgPath);
					}
				}

				teachers.Add (teacher);
			}

			return teachers;
		}

		public async Task InitializeRoles ()
		{
			if (!rolesInitialized) {
				// If there is no Admin role, then assume they both need to be created.
				var adminRole = await ParseRole.Query.Where (x => x.Name == AdminRole).FirstOrDefaultAsync ();
				if (adminRole == null) {
					adminRole = new ParseRole (AdminRole, new ParseACL { PublicReadAccess = true, PublicWriteAccess = true });
					await adminRole.SaveAsync ();
					var moderatorRole = new ParseRole (ModeratorRole, new ParseACL { PublicReadAccess = true, PublicWriteAccess = true });
					moderatorRole.Roles.Add (adminRole);
					await moderatorRole.SaveAsync ();
				}
				rolesInitialized = true;
			}
		}

		async Task<ParseRole> GetRoleAsync(string roleName)
		{
			return await ParseRole.Query.Where (x => x.Name == roleName).FirstOrDefaultAsync ();
		}

	}
}

