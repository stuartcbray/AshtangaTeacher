using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AshtangaTeacher
{
	public interface IParseService
	{
		void Initialize(string appId, string key, string facebookAppId);

		object CurrentUser { get; }

		Task InitializeRoles ();

		Task SignUpAsync (string name, 
			string userName, 
			string email, 
			string shalaName, 
			string password, 
			bool shalaExists);

		Task LogOutAsync ();

		Task SignInAsync (string username, string password);

		bool ShowLogin ();
	}
}

