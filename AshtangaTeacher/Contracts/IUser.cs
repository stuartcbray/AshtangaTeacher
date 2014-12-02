using System;
using System.ComponentModel;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public interface IUser
	{
		Task InitializeAsync (object userObj);

		string UID { get; set; }

		string ObjectId { get; }

		bool IsDirty { get; set; }

		bool ThumbIsDirty { get; set; }

		ImageSource Image { get; set; }

		string Name { get; set; }

		string Email { get; set; }

		string ShalaName { get; set; }

		object UserObj { get; set; }

		Task SaveAsync ();

		event PropertyChangedEventHandler PropertyChanged;
	}
}

