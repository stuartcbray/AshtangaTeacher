using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class AddProgressNotePage : ContentPage
	{	
		public AddProgressNotePage (StudentViewModel vm)
		{
			InitializeComponent ();
			BindingContext = vm;

			ViewModel.SaveProgressNoteCommand.CanExecuteChanged += (s, e) => CheckSaveCommentEnabled();
			CheckSaveCommentEnabled();

			SaveNoteButton.Clicked += (s, e) => ViewModel.SaveProgressNoteCommand.Execute (NoteText.Text);

			NoteText.TextChanged += (s, e) => CheckSaveCommentEnabled ();
		}

		public StudentViewModel ViewModel
		{
			get
			{
				return (StudentViewModel)BindingContext;
			}
		}

		private void CheckSaveCommentEnabled()
		{
			SaveNoteButton.IsEnabled = ViewModel.SaveProgressNoteCommand.CanExecute(NoteText.Text);
		}
	}
}

