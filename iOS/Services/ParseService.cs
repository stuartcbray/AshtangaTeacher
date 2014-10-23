using Parse;
using System.Threading.Tasks;

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

		public string CurrentShalaName {
			get {
				if (ParseUser.CurrentUser != null)
					return ParseUser.CurrentUser.Get<string> ("shalaname");
				return null;
			}
		}

		public async Task SignUpAsync (Teacher teacher)
		{
			var user = new ParseUser () {
				Username = teacher.Name,
				Password = teacher.Password,
				Email = teacher.Email
			};
					
			user ["shalaname"] = teacher.ShalaName;

			await user.SignUpAsync ();
		}

		public void Initialize (string appId, string key)
		{
			ParseClient.Initialize (appId, key);
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

