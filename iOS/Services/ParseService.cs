using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Parse;
using Xamarin.Forms;
using AshtangaTeacher.iOS;

[assembly: Xamarin.Forms.Dependency (typeof (ParseService))]

namespace AshtangaTeacher.iOS
{
	public class ParseService : IParseService
	{

		bool rolesInitialized;
		const string AdminRole = "Administrator";
		const string ModeratorRole = "Moderator";

		public object CurrentUser { 
			get {
				return ParseUser.CurrentUser;
			} 
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
			user ["uid"] = Guid.NewGuid().ToString();
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
			await Task.Run (() => ParseUser.LogOut ());
		}

		public async Task SignInAsync (string username, string password)
		{
			await ParseUser.LogInAsync (username, password);
		}

		public async Task<List<ITeacher>> GetTeachers (string shalaName)
		{
			var query = ParseUser.Query.Where (teacher => teacher.Get<string> ("shalaNameLC") == shalaName.ToLower ());
			IEnumerable<ParseUser> results = await query.FindAsync();

			var adminRole = await GetRoleAsync (AdminRole);
			var modsRole = await GetRoleAsync (ModeratorRole);

			var teachers = new List<ITeacher> ();

			foreach (var o in results) {

				var t = DependencyService.Get<ITeacher>(DependencyFetchTarget.NewInstance);
				await t.InitializeAsync (o);
									
				var users = await adminRole.Users.Query.FindAsync ();
				if (users != null && users.Any(x => x.ObjectId == o.ObjectId)) {
					t.Role = TeacherRole.Administrator;
				} else {
					users = await modsRole.Users.Query.FindAsync ();
					if (users != null && users.Any(x => x.ObjectId == o.ObjectId))
						t.Role = TeacherRole.Moderator;
				}

				// Try the local cache first
				var cameraService = DependencyService.Get<ICameraService> ();
				var imgPath = cameraService.GetImagePath (t.UID);

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

						var deviceService = DependencyService.Get<IDeviceService> ();
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

