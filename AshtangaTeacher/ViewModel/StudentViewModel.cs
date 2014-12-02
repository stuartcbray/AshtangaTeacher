using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Labs.Services.Media;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public class StudentViewModel : ViewModelBase
	{
		bool isLoading;
		string errorMessage;

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

		public bool IsLoading {
			get {
				return isLoading;
			}
			set {
				if (Set (() => IsLoading, ref isLoading, value)) {
					SaveProgressNoteCommand.RaiseCanExecuteChanged ();
					RaisePropertyChanged ("IsReady");
				}
			}
		}

		public bool IsReady {
			get {
				return !isLoading;
			}
		}

		public IStudent Model {
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

								IsLoading = true;
								var imageUrl = await cameraService.GetThumbAsync(imageSource, Model.UID);
								IsLoading = false;

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
					 () => {
							var nav = ServiceLocator.Current.GetInstance<INavigator> ();
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
						note.InputDate = DateTime.Now; 
						note.Text = text;

						IsLoading = true;
						var result = await Model.AddProgressNoteAsync (note);
						IsLoading = false;

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
					text => !string.IsNullOrEmpty (text) && !IsLoading));
			}
		}

		public async Task GetProgressNotesAsync ()
		{
			IsLoading = true;
			await Model.GetProgressNotesAsync ();
			IsLoading = false;
		}

		public RelayCommand SaveStudentCommand {
			get {
				return saveStudentCommand
					?? (saveStudentCommand = new RelayCommand (
						async () => {
							IsLoading = true;
							await Model.SaveAsync ();
							IsLoading = false;
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
							IsLoading = true;
							await Model.DeleteAsync ();
							App.Locator.Main.Students.Remove (this);
							IsLoading = false;
							var nav = ServiceLocator.Current.GetInstance<INavigator> ();
							nav.GoBack ();
						}));
			}
		}

		public StudentViewModel (IStudent model)
		{
			Model = model;
			Model.PropertyChanged += (sender, e) => { 
				if (e.PropertyName == "IsDirty") {
					SaveStudentCommand.RaiseCanExecuteChanged();
				}
			};
		}
	}
}