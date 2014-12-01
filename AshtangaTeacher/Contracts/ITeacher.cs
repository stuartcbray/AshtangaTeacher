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

	public interface ITeacher
	{
		Task InitializeAsync (object userObj);

		string TeacherId { get; set; }

		string ObjectId { get; }

		TeacherRole Role { get; set; }

		bool IsDirty { get; set; }

		bool ThumbIsDirty { get; set; }

		ImageSource Image { get; set; }

		string Name { get; set; }

		string UserName { get; set; }

		string Email { get; set; }

		string ShalaName { get; set; }

		object UserObj { get; set; }

		Task<bool> ShalaExistsAsync (string name);

		Task UpdateRoleAsync (TeacherRole role);

		Task UpdatePropertyAsync<T> (string name, T value);

		Task SaveAsync ();

		event PropertyChangedEventHandler PropertyChanged;
	}
}

