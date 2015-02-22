using System;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;
using System.Threading.Tasks;
using XLabs.Platform.Services.Media;
using XLabs.Platform.Device;

namespace AshtangaTeacher
{
	public class ProfileViewModel : ViewModelBase
	{
		bool isLoading;
		bool isPhotoVisible;

		string errorMessage;

		ITeacher teacher;
		readonly IParseService parseService;

		ImageSource imageSource;
		IMediaPicker mediaPicker;

		Command addTeacherPhotoCommand;
		Command saveTeacherCommand;
		Command logOutCommand;
		Command addShalaCommand;

		public ITeacher Model {
			get {
				return teacher;
			}
			set {
				Set (() => Model, ref teacher, value);
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

		public Command AddShalaCommand {
			get {
				return addShalaCommand
					?? (addShalaCommand = new Command (
						() => Navigator.NavigateTo (PageLocator.AddShalaPageKey, new AddShalaViewModel (MainTabsViewModel.Instance.ShalasVm, Navigator))));
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
							ErrorMessage = "";
						}, 
						() => Model != null && Model.IsDirty && IsReady));
			}
		}

		public Command LogOutCommand {
			get {
				return logOutCommand
					?? (logOutCommand = new Command (
						async () => {
							await parseService.LogOutAsync ();
							Navigator.PopToRoot();
							App.Instance.RootNavigator.PopToRoot ();
						}));
			}
		}


		public Command AddTeacherPhotoCommand {
			get {
				return addTeacherPhotoCommand
					?? (addTeacherPhotoCommand = new Command (
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

		public async Task Init ()
		{
			await parseService.InitializeRoles ();
			await teacher.InitializeAsync (parseService.CurrentUser);

			OnPropertyChanged ("Model");

			IsLoading = false;
		}

		public ProfileViewModel ()
		{
			IsLoading = true;

			parseService = DependencyService.Get<IParseService>();
			Navigator = new NavigationService ();

			teacher = DependencyService.Get<ITeacher> (DependencyFetchTarget.NewInstance);

			teacher.IsDirtyChanged += SaveTeacherCommand.ChangeCanExecute;
		}
	}
}

