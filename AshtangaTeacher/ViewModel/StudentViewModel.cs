using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Labs.Services.Media;

namespace AshtangaTeacher
{
	public class StudentViewModel : ViewModelBase
	{
		bool isSaving;
		string errorMessage;

		readonly IStudentsService studentService;

		RelayCommand addProgressNoteCommand;
		RelayCommand showProgressNotesCommand;
		RelayCommand saveStudentCommand;
		RelayCommand deleteStudentCommand;
		RelayCommand updatePhotoCommand;
		RelayCommand<string> saveProgressNoteCommand;

		ImageSource imageSource;
		IMediaPicker mediaPicker;

		public string ErrorMessage {
			get {
				return errorMessage;
			}
			set {
				Set (() => ErrorMessage, ref errorMessage, value);
			}
		}

		public bool IsSaving {
			get {
				return isSaving;
			}
			set {
				if (Set (() => IsSaving, ref isSaving, value)) {
					SaveProgressNoteCommand.RaiseCanExecuteChanged ();
					RaisePropertyChanged ("IsReady");
				}
			}
		}

		public bool IsReady {
			get {
				return !isSaving;
			}
		}

		public Student Model {
			get;
			private set;
		}

		public RelayCommand UpdatePhotoCommand {
			get {
				return updatePhotoCommand
					?? (updatePhotoCommand = new RelayCommand (
						async () => 
						{
							mediaPicker = DependencyService.Get<IMediaPicker>();
							imageSource = null;

							try
							{
								var mediaFile = await mediaPicker.SelectPhotoAsync(new CameraMediaStorageOptions
									{
										DefaultCamera = CameraDevice.Front,
										MaxPixelDimension = 400
									});
								imageSource = ImageSource.FromStream(() => mediaFile.Source);

								var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();

								IsSaving = true;
								var imageUrl = await cameraService.GetThumbAsync(imageSource, Model.StudentId);
								IsSaving = false;

								Model.Image = imageUrl;
							}
							catch (Exception ex)
							{
								ErrorMessage = ex.Message;
							}
						}));
			}
		}

		public RelayCommand ShowProgressNotesCommand {
			get {
				return showProgressNotesCommand
				?? (showProgressNotesCommand = new RelayCommand (
					async () => {
							var nav = ServiceLocator.Current.GetInstance<INavigator> ();
							IsSaving = true;
							var notes = await studentService.GetStudentProgressNotesAsync (Model); 
							Model.ProgressNotes = new ObservableCollection<IProgressNote> (notes);
							IsSaving = false;
							nav.NavigateTo (ViewModelLocator.ProgressNotesPageKey, this);
						}));
			}
		}

		public RelayCommand AddProgressNoteCommand {
			get {
				return addProgressNoteCommand
				?? (addProgressNoteCommand = new RelayCommand (
					() => {
						var nav = ServiceLocator.Current.GetInstance<INavigator> ();
						nav.NavigateTo (ViewModelLocator.AddProgressNotePageKey, this);
					}));
			}
		}

		public RelayCommand<string> SaveProgressNoteCommand {
			get {
				return saveProgressNoteCommand
				?? (saveProgressNoteCommand = new RelayCommand<string> (
					async text => {
						
						var note = DependencyService.Get<IProgressNote>(DependencyFetchTarget.NewInstance);
						note.InputDate = DateTime.Now; // this will get updated when added to Parse
						note.Text = text;

						Model.ProgressNotes.Add (note);
						
						IsSaving = true;
						var result = await studentService.AddProgressNoteAsync (Model, note);
						IsSaving = false;

						if (!result) {
							var dialog = ServiceLocator.Current.GetInstance<IDialogService> ();
							await dialog.ShowError (
								"Error when saving, your note was not saved",
								"Error",
								"OK",
								null);
						}

						var nav = ServiceLocator.Current.GetInstance<INavigator> ();
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
							var nav = ServiceLocator.Current.GetInstance<INavigator> ();
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
							var nav = ServiceLocator.Current.GetInstance<INavigator> ();
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