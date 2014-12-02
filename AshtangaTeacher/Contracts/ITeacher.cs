using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public enum TeacherRole {
		Administrator,
		Moderator,
		Pending,
		None
	};

	public interface ITeacher : IUser
	{
		TeacherRole Role { get; set; }

		string UserName { get; set; }
	
		Task<bool> ShalaExistsAsync (string name);

		Task UpdateRoleAsync (TeacherRole role);

		Task UpdatePropertyAsync<T> (string name, T value);
	}
}

