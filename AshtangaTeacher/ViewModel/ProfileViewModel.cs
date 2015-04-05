using System;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;
using System.Threading.Tasks;
using XLabs.Platform.Services.Media;
using XLabs.Platform.Device;
using System.Diagnostics;
using XLabs.Ioc;

namespace AshtangaTeacher
{
	public class ProfileViewModel : ViewModelBase
	{
		ITeacher teacher;
		readonly IParseService parseService;
		bool initialized;

		ImageSource imageSource;
		IMediaPicker mediaPicker;

		Command addTeacherPhotoCommand;
		Command saveTeacherCommand;
		Command logOutCommand;
		Command addShalaCommand;
		Command editProfileCommand;
		Command cancelEditCommand;

		bool isEditMode;
		public bool IsEditMode {
			get {
				return isEditMode;
			}
			set {
				Set (() => IsEditMode, ref isEditMode, value);
			}
		}

		string statusMessage;
		public string StatusMessage {
			get {
				return statusMessage;
			}
			set {
				Set (() => StatusMessage, ref statusMessage, value);
			}
		}

		public ITeacher Model {
			get {
				return teacher;
			}
			set {
				Set (() => Model, ref teacher, value);
			}
		}
			
		public Command AddShalaCommand {
			get {
				return addShalaCommand
					?? (addShalaCommand = new Command (
						() => Navigator.NavigateTo (PageLocator.AddShalaPageKey, new AddShalaViewModel (MainTabsViewModel.Instance.ShalasVm, Navigator))));
			}
		}

		public Command EditProfileCommand {
			get {
				return editProfileCommand
					?? (editProfileCommand = new Command (
					() => Navigator.NavigateTo (PageLocator.EditProfileKey, this)));
			}
		}

		public Command CancelEditCommand {
			get {
				return cancelEditCommand
					?? (cancelEditCommand = new Command (
						() => IsEditMode = false));
			}
		}
			
		public Command SaveTeacherCommand {
			get {
				return saveTeacherCommand
					?? (saveTeacherCommand = new Command (
						async () => {
							StatusMessage = "Saving ...";
							IsEditMode = false;
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
								Debug.WriteLine(ex.Message);
							}
						}));
			}
		}

		public async Task Init ()
		{
			if (!initialized) {
				await parseService.InitializeRoles ();
				await teacher.InitializeAsync (parseService.CurrentUser);
				OnPropertyChanged ("Model");
				IsLoading = false;
				initialized = true;
			}
		}

		public ProfileViewModel ()
		{

			IsLoading = true;
			StatusMessage = "Loading ...";

			parseService = DependencyService.Get<IParseService>();

			Navigator = new NavigationService ();

			teacher = DependencyService.Get<ITeacher> (DependencyFetchTarget.NewInstance);

			teacher.IsDirtyChanged += SaveTeacherCommand.ChangeCanExecute;

			IsLoadingChanged += () =>  {
				OnPropertyChanged ("IsReady");
				SaveTeacherCommand.ChangeCanExecute();
			};
		}
	}
}

