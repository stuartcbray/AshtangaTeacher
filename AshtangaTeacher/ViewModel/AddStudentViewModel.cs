using Xamarin.Forms;
using Microsoft.Practices.ServiceLocation;
using XLabs.Platform.Services.Media;

namespace AshtangaTeacher
{
	public class AddStudentViewModel : ViewModelBase
	{
		bool isPhotoVisible;

		StudentsViewModel studentsViewModel;

		readonly IStudent student;

		ImageSource imageSource;
		IMediaPicker mediaPicker;

		Command addStudentCommand;
		Command cancelCommand;
		Command addStudentPhotoCommand;

		public IStudent Model { get { return student; } }

		public bool IsPhotoVisible {
			get {
				return isPhotoVisible;
			}
			set {
				Set (() => IsPhotoVisible, ref isPhotoVisible, value);
			}
		}
			
		public Command CancelCommand {
			get {
				return cancelCommand
					?? (cancelCommand = new Command (() => Navigator.GoBack ()));
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

							var deviceService = DependencyService.Get<IDeviceService>();
							if (!deviceService.IsValidEmail (Model.Email)) {
								ErrorMessage = "Invalid Email";
								return;
							}

							if (Model.Image == null) {
								ErrorMessage = "Image is empty";
								return;
							}


							IsLoading = true;
							//await student.SaveAsync ();
							IsLoading = false;

							studentsViewModel.Students.Add (new StudentViewModel (student, Navigator));

							Navigator.GoBack ();

					}
				));
			}
		}

		public AddStudentViewModel (StudentsViewModel studentsViewModel, NavigationService nav)
		{
			this.studentsViewModel = studentsViewModel;
			Navigator = nav;
			student = DependencyService.Get<IStudent>(DependencyFetchTarget.NewInstance);
		}
	}
}

