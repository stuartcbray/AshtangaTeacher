using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AshtangaTeacher
{
	public interface IParseService
	{
		void Initialize(string appId, string key, string facebookAppId);

		Task InitializeRoles ();

		Task SignUpAsync (string name, 
			string userName, 
			string email, 
			string shalaName, 
			string password, 
			bool shalaExists);

		Task<bool> ShalaNameExists (string name);

		Task AddUserToRole (string objectId, string roleName);

		Task MakeUserAdminAsync ();

		Task LogOutAsync ();

		Task UpdateUserPropertyAsync (string name, string value);

		Task SignInAsync (string username, string password);

		Task<List<ITeacher>> GetTeachers ();

		bool ShowLogin ();

		Task<ITeacher> GetTeacherAsync ();

		string ShalaName { get; }
	}
}

