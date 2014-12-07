using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AshtangaTeacher
{
	public enum TeacherRole {
		None,
		Administrator,
		Moderator,
		Pending
	};

	public interface ITeacher : IUser
	{
		TeacherRole Role { get; set; }

		string RoleDisplay { get; }

		string UserName { get; set; }
	
		Task<bool> ShalaExistsAsync (string name);

		Task UpdateRoleAsync (TeacherRole role);

		Task<ObservableCollection<StudentViewModel>> GetStudentsAsync ();

		Task<ObservableCollection<ITeacher>> GetTeachersAsync ();
	}
}

