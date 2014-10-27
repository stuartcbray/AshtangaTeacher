using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AshtangaTeacher
{
	public interface IParseService
	{
		void Initialize(string appId, string key, string facebookAppId);

		Task SignUpAsync(Teacher teacher);

		Task LogOutAsync ();

		Task UpdateUserPropertyAsync (string name, string value);

		Task SignInAsync (string username, string password);

		bool ShowLogin ();

		string CurrentUser { get; }

		string CurrentShalaName { get; }

	}
}

