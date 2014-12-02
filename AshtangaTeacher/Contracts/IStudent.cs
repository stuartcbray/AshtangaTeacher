using System;
using System.ComponentModel;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AshtangaTeacher
{
	public interface IStudent : IUser
	{
		Task GetProgressNotesAsync ();

		Task<bool> AddProgressNoteAsync (IProgressNote note);

		Task DeleteAsync ();

		ObservableCollection<IProgressNote> ProgressNotes { get; set; }

		DateTime ExpiryDate { get; set; }
	}
}

