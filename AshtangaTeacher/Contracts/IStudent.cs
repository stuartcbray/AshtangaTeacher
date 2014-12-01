using System;
using System.ComponentModel;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace AshtangaTeacher
{
	public interface IStudent
	{
		string ShalaName { get; set; }

		string StudentId { get; set; }

		string ObjectId { get; set; }

		string Name { get; set; }

		string Email { get; set; }

		bool IsDirty { get; set; }

		bool ThumbIsDirty { get; set; }

		ObservableCollection<IProgressNote> ProgressNotes { get; set; }

		ImageSource Image { get; set; }

		DateTime ExpiryDate { get; set; }

		event PropertyChangedEventHandler PropertyChanged;
	}
}

