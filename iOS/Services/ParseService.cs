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

