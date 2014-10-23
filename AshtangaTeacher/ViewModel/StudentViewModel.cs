using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;

namespace AshtangaTeacher
{
	public class StudentViewModel : ViewModelBase
	{
		bool isSaving;

		readonly IStudentsService studentService;

		RelayCommand addProgressNoteCommand;
		RelayCommand showProgressNotesCommand;
		RelayCommand saveStudentCommand;
		RelayCommand deleteStudentCommand;
		RelayCommand<string> saveProgressNoteCommand;

		public bool IsSaving {
			get {
				return isSaving;
			}
			set {
				if (Set (() => IsSaving, ref isSaving, value)) {
					SaveProgressNoteCommand.RaiseCanExecuteChanged ();
				}
			}
		}

		public string ImageFileName {
			get {
				return ImageUri.LocalPath;
			}
		}

		public Uri ImageUri {
			get {
				return new Uri (Model.Image);
			}
		}

		public Student Model {
			get;
			private set;
		}

		public RelayCommand ShowProgressNotesCommand {
			get {
				return showProgressNotesCommand
				?? (showProgressNotesCommand = new RelayCommand (
					async () => {
						var nav = ServiceLocator.Current.GetInstance<INavigationService> ();
						var notes = await studentService.GetStudentProgressNotesAsync (Model); 
						Model.ProgressNotes = new ObservableCollection<ProgressNote> (notes);
						nav.NavigateTo (ViewModelLocator.ProgressNotesPageKey, this);
					}));
			}
		}

		public RelayCommand AddProgressNoteCommand {
			get {
				return addProgressNoteCommand
				?? (addProgressNoteCommand = new RelayCommand (
					() => {
						var nav = ServiceLocator.Current.GetInstance<INavigationService> ();
						nav.NavigateTo (ViewModelLocator.AddProgressNotePageKey, this);
					}));
			}
		}

		public RelayCommand<string> SaveProgressNoteCommand {
			get {
				return saveProgressNoteCommand
				?? (saveProgressNoteCommand = new RelayCommand<string> (
					async text => {
						IsSaving = true;

						var note = new ProgressNote { 
							InputDate = DateTime.Now, // this will get updated when added to Parse
							Text = text
						};

						Model.ProgressNotes.Add (note);

						var result = await studentService.AddProgressNoteAsync (Model, note);

						if (!result) {
							var dialog = ServiceLocator.Current.GetInstance<IDialogService> ();
							await dialog.ShowError (
								"Error when saving, your note was not saved",
								"Error",
								"OK",
								null);
						}

						IsSaving = false;
						var nav = ServiceLocator.Current.GetInstance<INavigationService> ();
						nav.GoBack ();
					},
					text => !string.IsNullOrEmpty (text) && !IsSaving));
			}
		}

		public RelayCommand SaveStudentCommand {
			get {
				return saveStudentCommand
					?? (saveStudentCommand = new RelayCommand (
						async () => {
							IsSaving = true;
							await studentService.SaveAsync (Model);
							IsSaving = false;
							var nav = ServiceLocator.Current.GetInstance<INavigationService> ();
							nav.GoBack ();
						}, 
					() => Model.IsDirty));
			}
		}

		public RelayCommand DeleteStudentCommand {
			get {
				return deleteStudentCommand
					?? (deleteStudentCommand = new RelayCommand (
						async () => {
							IsSaving = true;
							await studentService.DeleteAsync (Model);
							App.Locator.Main.Students.Remove (this);
							IsSaving = false;
							var nav = ServiceLocator.Current.GetInstance<INavigationService> ();
							nav.GoBack ();
						}));
			}
		}

		public StudentViewModel (IStudentsService studentService, Student model)
		{
			this.studentService = studentService;
			Model = model;
			Model.PropertyChanged += (sender, e) => { 
				if (e.PropertyName == "IsDirty") {
					SaveStudentCommand.RaiseCanExecuteChanged();
				}
			};
		}
	}
}