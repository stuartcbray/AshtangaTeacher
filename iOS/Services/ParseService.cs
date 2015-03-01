using System;
using System.Threading.Tasks;
using Parse;
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
			string password) 
		{
			var user = new ParseUser () {
				Username = userName,
				Password = password,
				Email = email
			};

			user ["name"] = name;
			user ["uid"] = Guid.NewGuid().ToString();

			await user.SignUpAsync ();
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
			
		public async Task InitializeRoles ()
		{
			if (!rolesInitialized) {
				// If there is no Admin role, then assume they both need to be created.
				var adminRoleName = AdminRole + "-" + ParseUser.CurrentUser.Username;
				var adminRole = await ParseRole.Query.Where (x => x.Name == adminRoleName).FirstOrDefaultAsync ();
				if (adminRole == null) {
					adminRole = new ParseRole (adminRoleName, new ParseACL { PublicReadAccess = true });
					await adminRole.SaveAsync ();

					var moderatorRoleName = ModeratorRole + "-" + ParseUser.CurrentUser.Username;
					var moderatorRole = new ParseRole (moderatorRoleName, new ParseACL { PublicReadAccess = true });
					moderatorRole.Roles.Add (adminRole);
					await moderatorRole.SaveAsync ();
				}
				rolesInitialized = true;
			}
		}

		public async Task ResetPasswordAsync (string email)
		{
			await ParseUser.RequestPasswordResetAsync (email);
		}

		async Task<ParseRole> GetRoleAsync(string roleName)
		{
			return await ParseRole.Query.Where (x => x.Name == roleName).FirstOrDefaultAsync ();
		}

	}
}

