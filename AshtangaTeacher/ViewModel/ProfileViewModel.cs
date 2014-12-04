using System;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms.Labs.Services.Media;
using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Forms.Labs.Mvvm;

namespace AshtangaTeacher
{
	public class ProfileViewModel : ViewModelBase
	{
		bool isLoading;
		bool isPhotoVisible;

		string errorMessage;

		ITeacher teacher;
		readonly IParseService parseService;
		readonly INavigator navigationService;

		ImageSource imageSource;
		IMediaPicker mediaPicker;

		Command shalaTeachersCommand;
		Command addTeacherPhotoCommand;
		Command saveTeacherCommand;
		Command logOutCommand;

		public ITeacher Model {
			get {
				return teacher;
			}
			set {
				Set (() => Model, ref teacher, value);
			}
		}

		public Command ShalaTeachersCommand {
			get {
				return shalaTeachersCommand
					?? (shalaTeachersCommand = new Command (
						async () => {
							IsLoading = true;
							var teachers = await parseService.GetTeachers (Model.ShalaName);
							IsLoading = false;
							App.Locator.ShalaTeachers.Init (teachers);

							navigationService.NavigateTo (ViewModelLocator.ShalaTeachersPageKey, App.Locator.ShalaTeachers);
						}));
			}
		}

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
					OnPropertyChanged ("IsReady");
					SaveTeacherCommand.ChangeCanExecute();
				}
			}
		}

		public bool IsReady {
			get {
				return !isLoading;
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

		public Command SaveTeacherCommand {
			get {
				return saveTeacherCommand
					?? (saveTeacherCommand = new Command (
						async () => {
							IsLoading = true;
							await Model.SaveAsync ();
							IsLoading = false;
						}, 
						() => Model.IsDirty && IsReady));
			}
		}

		public Command LogOutCommand {
			get {
				return logOutCommand
					?? (logOutCommand = new Command (
						async () => {
							await parseService.LogOutAsync ();
							navigationService.SetRootNavigation(App.RootNavPage);

							// Reset view models and ensure we re-load the new Teacher 
							App.Locator.MainTabs.IsLoading = true;
							App.Locator.Reset ();

							navigationService.NavigateTo (ViewModelLocator.LoginPageKey, App.Locator.Login);
						}));
			}
		}


		public Command AddTeacherPhotoCommand {
			get {
				return addTeacherPhotoCommand
					?? (addTeacherPhotoCommand = new Command (
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
								var thumb = await cameraService.GetThumbAsync(imageSource, teacher.UID);
								IsLoading = false;

								teacher.Image = thumb;
							}
							catch (Exception ex)
							{
								ErrorMessage = ex.Message;
							}
						}));
			}
		}

		public async Task InitializeTeacher ()
		{
			IsLoading = true;
			await teacher.InitializeAsync (parseService.CurrentUser);
			IsPhotoVisible = Model.Image != null;
			IsLoading = false;
		}

		public ProfileViewModel ()
		{
			parseService = DependencyService.Get<IParseService>();
			navigationService = NavigationService.Instance;
			teacher = DependencyService.Get<ITeacher> (DependencyFetchTarget.NewInstance);

			teacher.PropertyChanged += (sender, e) => { 
				if (e.PropertyName == "IsDirty") {
					SaveTeacherCommand.ChangeCanExecute();
				}
			};
		}
	}
}

