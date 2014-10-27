using Parse;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AshtangaTeacher.iOS
{
	public class ParseService : IParseService
	{
		public string CurrentUser { 
			get {
				if (ParseUser.CurrentUser != null)
					return ParseUser.CurrentUser.Username;
				return null;
			} 
		}

		public async Task UpdateUserPropertyAsync(string name, string value)
		{
			ParseUser.CurrentUser [name] = value;
			await ParseUser.CurrentUser.SaveAsync ();
		}

		public string CurrentShalaName {
			get {
				if (ParseUser.CurrentUser != null && ParseUser.CurrentUser.ContainsKey("shalaname")) {
					return ParseUser.CurrentUser.Get<string> ("shalaname");
				}
				return null;
			}
		}

		public async Task SignUpAsync (Teacher teacher)
		{
			var user = new ParseUser () {
				// username is the same as Email
				Username = teacher.UserName,
				Password = teacher.Password,
				Email = teacher.Email
			};
					
			user ["shalaname"] = teacher.ShalaName;
			user ["name"] = teacher.Name;

			await user.SignUpAsync ();
		}

		public void Initialize (string appId, string key, string facebookAppId)
		{
			ParseClient.Initialize (appId, key);
			ParseFacebookUtils.Initialize(facebookAppId);
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
	}
}

