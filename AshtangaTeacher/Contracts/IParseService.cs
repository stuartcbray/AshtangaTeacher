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

		Task SignUpAsync (ITeacher teacher, bool shalaExists);

		Task<bool> ShalaNameExists (string name);

		Task AddUserToRole (string objectId, string roleName);

		Task MakeUserAdminAsync ();

		Task LogOutAsync ();

		Task UpdateUserPropertyAsync (string name, string value);

		Task SignInAsync (string username, string password);

		Task SaveTeacherAsync (ITeacher teacher);

		Task<List<ITeacher>> GetTeachers ();

		bool ShowLogin ();

		Task<ITeacher> GetTeacherAsync ();

		string ShalaName { get; }
	}
}

