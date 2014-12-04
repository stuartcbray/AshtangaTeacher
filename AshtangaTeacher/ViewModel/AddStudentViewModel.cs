using Xamarin.Forms.Labs.Services.Media;
using Xamarin.Forms;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms.Labs.Mvvm;

namespace AshtangaTeacher
{
	public class AddStudentViewModel : ViewModelBase
	{
		bool isLoading, isPhotoVisible;

		readonly INavigator navigationService;
		readonly IStudent student;

		ImageSource imageSource;
		IMediaPicker mediaPicker;

		Command addStudentCommand;
		Command cancelCommand;
		Command addStudentPhotoCommand;

		string errorMessage;

		public IStudent Model { get { return student; } }

		public string ErrorMessage {
			get {
				return errorMessage;
			}
			set {
				Set (() => ErrorMessage, ref errorMessage, value);
			}
		}

		public bool IsPhotoVisible {
			get {
				return isPhotoVisible;
			}
			set {
				Set (() => IsPhotoVisible, ref isPhotoVisible, value);
			}
		}

		public bool IsLoading {
			get {
				return isLoading;
			}
			set {
				if (Set (() => IsLoading, ref isLoading, value)) {
					OnPropertyChanged ("IsReady");
				}
			}
		}

		public bool IsReady {
			get {
				return !isLoading;
			}
		}
			
		public Command CancelCommand {
			get {
				return cancelCommand
				?? (cancelCommand = new Command (() => navigationService.GoBack ()));
			}
		}

		public Command AddStudentPhotoCommand {
			get {
				return addStudentPhotoCommand
					?? (addStudentPhotoCommand = new Command (
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
								IsPhotoVisible = true;

								var cameraService = DependencyService.Get<ICameraService> ();

								IsLoading = true;
								var thumb = await cameraService.GetThumbAsync(imageSource, student.UID);
								IsLoading = false;

								student.Image = thumb;
							}
							catch (System.Exception ex)
							{
								ErrorMessage = ex.Message;
							}
						}));
			}
		}

		public Command AddStudentCommand {
			get {
				return addStudentCommand
				?? (addStudentCommand = new Command (
						async () => {
						
							// Perform some simple validation...
							if (string.IsNullOrEmpty (Model.Name)) {
								ErrorMessage = "User Name is empty";
								return;
							}

							if (string.IsNullOrEmpty (Model.Email)) {
								ErrorMessage = "Email is empty";
								return;
							}

							if (Model.Image == null) {
								ErrorMessage = "Image is empty";
								return;
							}
								
							IsLoading = true;
							await student.SaveAsync ();
							IsLoading = false;

							App.Locator.Main.Students.Add (new StudentViewModel (student));

							navigationService.GoBack ();
							
						}));
			}
		}

		public AddStudentViewModel ()
		{
			navigationService = NavigationService.Instance;
			student = DependencyService.Get<IStudent>(DependencyFetchTarget.NewInstance);
		}
	}
}

