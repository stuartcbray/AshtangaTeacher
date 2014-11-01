using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight;
using Xamarin.Forms.Labs.Services.Media;
using Xamarin.Forms;
using Microsoft.Practices.ServiceLocation;

namespace AshtangaTeacher
{
	public class AddStudentViewModel : ViewModelBase
	{
		bool isLoading, addStudentEnabled, isPhotoVisible;

		readonly IStudentsService studentService;
		readonly INavigator navigationService;
		readonly Student student;

		ImageSource imageSource;
		IMediaPicker mediaPicker;

		RelayCommand addStudentCommand;
		RelayCommand cancelCommand;
		RelayCommand addStudentPhotoCommand;

		string errorMessage;

		public Student Model { get { return student; } }

		public string ErrorMessage {
			get {
				return errorMessage;
			}
			set {
				Set (() => ErrorMessage, ref errorMessage, value);
			}
		}

		public bool AddStudentEnabled {
			get {
				return addStudentEnabled;
			}
			set {
				Set (() => AddStudentEnabled, ref addStudentEnabled, value);
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
					AddStudentEnabled = !IsLoading;
				}
			}
		}

		public RelayCommand CancelCommand {
			get {
				return cancelCommand
				?? (cancelCommand = new RelayCommand (() => navigationService.GoBack ()));
			}
		}

		public RelayCommand AddStudentPhotoCommand {
			get {
				return addStudentPhotoCommand
					?? (addStudentPhotoCommand = new RelayCommand (
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

								var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();
								var imageUrl = await cameraService.SaveThumbAsync(imageSource, student.StudentId);
								student.ImageUrl = imageUrl;
							}
							catch (System.Exception ex)
							{
								ErrorMessage = ex.Message;
							}
						}));
			}
		}

		public RelayCommand AddStudentCommand {
			get {
				return addStudentCommand
				?? (addStudentCommand = new RelayCommand (
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
								
							IsLoading = true;
							await studentService.AddAsync (student);
							IsLoading = false;

							App.Locator.Main.Students.Add (new StudentViewModel (studentService, student));

							navigationService.GoBack ();
							
						}));
			}
		}

		public AddStudentViewModel (IStudentsService service, INavigator nav, Student student)
		{
			studentService = service;
			navigationService = nav;
			addStudentEnabled = true;
			this.student = student;
		}
	}
}

