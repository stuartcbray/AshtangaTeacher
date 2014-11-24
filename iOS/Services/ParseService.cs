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

		ITeacher currentTeacher;
		public async Task<ITeacher> GetTeacherAsync() 
		{

			if (ParseUser.CurrentUser != null) {
				if (currentTeacher == null) {
				
					currentTeacher = DependencyService.Get<ITeacher> (DependencyFetchTarget.NewInstance);
					currentTeacher.UserObj = ParseUser.CurrentUser;

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

		public async Task SignUpAsync (
			string name, 
			string userName, 
			string email, 
			string shalaName, 
			string password, 
			bool shalaExists) 
		{
			var user = new ParseUser () {
				Username = userName,
				Password = password,
				Email = email
			};

			user ["shalaName"] = shalaName;
			user ["shalaNameLC"] = shalaName.ToLower ();
			user ["name"] = name;
			user ["teacherId"] = Guid.NewGuid().ToString();
			user ["role"] = shalaExists ? (long)TeacherRole.None : (long)TeacherRole.Administrator;

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

		public async Task<List<ITeacher>> GetTeachers ()
		{
			var query = ParseUser.Query.Where (teacher => teacher.Get<string> ("shalaNameLC") == currentTeacher.ShalaName.ToLower ());
			IEnumerable<ParseUser> results = await query.FindAsync();

			var adminRole = await GetRoleAsync (AdminRole);
			var modsRole = await GetRoleAsync (ModeratorRole);

			var teachers = new List<ITeacher> ();

			foreach (var o in results) {

				var t = DependencyService.Get<ITeacher>(DependencyFetchTarget.NewInstance);
				t.UserObj = o;
									
				var users = await adminRole.Users.Query.FindAsync ();
				if (users != null && users.Any(x => x.ObjectId == o.ObjectId)) {
					t.Role = TeacherRole.Administrator;
				} else {
					users = await modsRole.Users.Query.FindAsync ();
					if (users != null && users.Any(x => x.ObjectId == o.ObjectId))
						t.Role = TeacherRole.Moderator;
				}

				// Try the local cache first
				var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();
				var imgPath = cameraService.GetImagePath (t.TeacherId);

				bool fetchImage = true;
				if (File.Exists (imgPath)) {
					var dt = File.GetLastWriteTimeUtc (imgPath);
					if (o.UpdatedAt != null && o.UpdatedAt <= dt) {
						t.Image = ImageSource.FromFile (imgPath);
						fetchImage = false;
					} 
				}

				if (fetchImage) {

					byte[] imageData = null;
					if (o.ContainsKey ("image")) {
						var parseImg = o.Get<ParseFile> ("image");
						imageData = await new HttpClient ().GetByteArrayAsync (parseImg.Url);
					} else if (o.ContainsKey ("facebookImageUrl")) {
						var url = o.Get<string> ("facebookImageUrl");
						imageData = await new HttpClient ().GetByteArrayAsync (url);
					}

					if (imageData != null) {
						t.Image = ImageSource.FromStream (() => new MemoryStream (imageData));

						var deviceService = ServiceLocator.Current.GetInstance<IDeviceService> ();
						deviceService.SaveToFile (imageData, imgPath);
					}
				}

				teachers.Add (t);
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

