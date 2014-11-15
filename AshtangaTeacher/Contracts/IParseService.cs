using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AshtangaTeacher
{
	public interface IParseService
	{
		void Initialize(string appId, string key, string facebookAppId);

		Task SignUpAsync (Teacher teacher);

		Task<bool> ShalaNameExists (string name);

		Task LogOutAsync ();

		Task UpdateUserPropertyAsync (string name, string value);

		Task SignInAsync (string username, string password);

		Task SaveTeacherAsync (Teacher teacher);

		Task<ObservableCollection<Teacher>> GetTeachers ();

		Task<ObservableCollection<Teacher>> GetPendingTeachers ();

		bool ShowLogin ();

		Task<Teacher> GetTeacherAsync ();

		string ShalaName { get; }
	}
}

