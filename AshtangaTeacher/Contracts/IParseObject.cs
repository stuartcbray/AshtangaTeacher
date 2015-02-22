using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AshtangaTeacher
{
	public interface IParseObject
	{
		Task InitializeAsync (object parseObject);

		string ObjectId { get; }

		string UID { get; }

		bool IsDirty { get; set; }

		bool ThumbIsDirty { get; set; }

		ImageSource Image { get; set; }

		Task SaveAsync ();

		Task DeleteAsync ();

		event Action IsDirtyChanged;
	}
}

