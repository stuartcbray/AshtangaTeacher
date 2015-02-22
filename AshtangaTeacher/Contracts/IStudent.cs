using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public interface IStudent : IUser
	{
		Task GetProgressNotesAsync ();

		Task<bool> AddProgressNoteAsync (IProgressNote note);

		ObservableCollection<IProgressNote> ProgressNotes { get; set; }

		DateTime ExpiryDate { get; set; }
	}
}

