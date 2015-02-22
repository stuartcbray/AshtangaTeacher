using System;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using XLabs.Platform.Services.Media;
using XLabs.Platform.Device;

namespace AshtangaTeacher
{
	public class StudentViewModel : ViewModelBase
	{
		bool isLoading;
		string errorMessage;

		Command addProgressNoteCommand;
		Command showProgressNotesCommand;
		Command saveStudentCommand;
		Command deleteStudentCommand;
		Command updatePhotoCommand;
		Command<string> saveProgressNoteCommand;

		ImageSource imageSource;
		IMediaPicker mediaPicker;

		public event Action OnDeleted = () => {};

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
					SaveProgressNoteCommand.ChangeCanExecute ();
					OnPropertyChanged ("IsReady");
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

		public Command UpdatePhotoCommand {
			get {
				return updatePhotoCommand
					?? (updatePhotoCommand = new Command (
						async () => 
						{
							var device = XLabs.Ioc.Resolver.Resolve<IDevice>();
							mediaPicker = DependencyService.Get<IMediaPicker> () ?? device.MediaPicker;

							imageSource = null;

							try
							{
								var mediaFile = await mediaPicker.SelectPhotoAsync(new CameraMediaStorageOptions
									{
										DefaultCamera = CameraDevice.Front,
										MaxPixelDimension = 400
									});
								imageSource = ImageSource.FromStream(() => mediaFile.Source);

								var cameraService = DependencyService.Get<ICameraService> ();

								IsLoading = true;
								var imageUrl = await cameraService.GetThumbAsync (imageSource, Model.UID);
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

		public Command ShowProgressNotesCommand {
			get {
				return showProgressNotesCommand
				?? (showProgressNotesCommand = new Command (
						() => Navigator.NavigateTo (PageLocator.ProgressNotesPageKey, this)));
			}
		}

		public Command AddProgressNoteCommand {
			get {
				return addProgressNoteCommand
				?? (addProgressNoteCommand = new Command (
						() => Navigator.NavigateTo (PageLocator.AddProgressNotePageKey, this)));
			}
		}

		public Command<string> SaveProgressNoteCommand {
			get {
				return saveProgressNoteCommand
				?? (saveProgressNoteCommand = new Command<string> (
					async text => {
						
						var note = DependencyService.Get<IProgressNote>(DependencyFetchTarget.NewInstance);
						note.InputDate = DateTime.Now; 
						note.Text = text;

						IsLoading = true;
						var result = await Model.AddProgressNoteAsync (note);
						IsLoading = false;

						if (!result) {
							await DialogService.Instance.ShowError (
								"Error when saving, your note was not saved",
								"Error",
								"OK",
								null);
						}
								
						Navigator.GoBack ();
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

		public Command SaveStudentCommand {
			get {
				return saveStudentCommand
					?? (saveStudentCommand = new Command (
						async () => {
							IsLoading = true;
							await Model.SaveAsync ();
							IsLoading = false;
							Navigator.GoBack ();
						}, 
					() => Model.IsDirty));
			}
		}

		public Command DeleteStudentCommand {
			get {
				return deleteStudentCommand
					?? (deleteStudentCommand = new Command (
						async () => {
							IsLoading = true;
							await Model.DeleteAsync ();
							OnDeleted ();
							IsLoading = false;
							Navigator.GoBack ();
						}));
			}
		}

		public StudentViewModel (IStudent model, NavigationService nav)
		{
			Navigator = nav;
			Model = model;
			Model.IsDirtyChanged += () => { 
				SaveStudentCommand.ChangeCanExecute();
			};
		}
	}
}