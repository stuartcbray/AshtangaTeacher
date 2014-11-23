using System;
using Xamarin.Forms;
using System.ComponentModel;

namespace AshtangaTeacher
{
	public enum TeacherRole {
		Administrator,
		Moderator,
		None
	};

	public interface ITeacher
	{
		string TeacherId { get; set; }

		string ObjectId { get; set; }

		TeacherRole Role { get; set; }

		bool IsDirty { get; set; }

		bool ThumbIsDirty { get; set; }

		ImageSource Image { get; set; }

		string Name { get; set; }

		string UserName { get; set; }

		string Email { get; set; }

		string Password { get; set; }

		string ShalaName { get; set; }

		event PropertyChangedEventHandler PropertyChanged;
	}
}

