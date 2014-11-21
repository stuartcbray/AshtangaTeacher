using System;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight;
using Xamarin.Forms.Labs.Services.Media;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public class ProfileViewModel : ViewModelBase
	{
		bool isLoading;
		bool isPhotoVisible;

		string errorMessage;

		Teacher teacher;
		readonly IParseService parseService;
		readonly INavigator navigationService;

		ImageSource imageSource;
		IMediaPicker mediaPicker;

		RelayCommand shalaTeachersCommand;
		RelayCommand addTeacherPhotoCommand;
		RelayCommand saveTeacherCommand;

		public Teacher Model {
			get {
				return teacher;
			}
			set {
				Set (() => Model, ref teacher, value);
			}
		}

		public RelayCommand ShalaTeachersCommand {
			get {
				return shalaTeachersCommand
					?? (shalaTeachersCommand = new RelayCommand (
						async () => {
							IsLoading = true;
							var teachers = await parseService.GetTeachers ();
							IsLoading = false;
							navigationService.NavigateTo (ViewModelLocator.ShalaTeachersPageKey, new ShalaTeachersViewModel(teachers));
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
					RaisePropertyChanged ("IsReady");
					SaveTeacherCommand.RaiseCanExecuteChanged();
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

		public RelayCommand SaveTeacherCommand {
			get {
				return saveTeacherCommand
					?? (saveTeacherCommand = new RelayCommand (
						async () => {
							IsLoading = true;
							await parseService.SaveTeacherAsync (Model);
							IsLoading = false;
						}, 
						() => Model.IsDirty && IsReady));
			}
		}


		public RelayCommand AddTeacherPhotoCommand {
			get {
				return addTeacherPhotoCommand
					?? (addTeacherPhotoCommand = new RelayCommand (
						async () => 
						{
							mediaPicker = DependencyService.Get<IMediaPicker>();

							imageSource = null;

							try
							{
								var mediaFile = await mediaPicker.TakePhotoAsync(new CameraMediaStorageOptions
									{
										DefaultCamera = CameraDevice.Front,
										MaxPixelDimension = 400
									});
								imageSource = ImageSource.FromStream(() => mediaFile.Source);
								IsPhotoVisible = true;

								var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();

								IsLoading = true;
								var thumb = await cameraService.GetThumbAsync(imageSource, teacher.TeacherId);
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
			Model = await parseService.GetTeacherAsync ();
			Model.IsDirty = false;
			IsPhotoVisible = Model.Image != null;

			teacher.PropertyChanged += (sender, e) => { 
				if (e.PropertyName == "IsDirty") {
					SaveTeacherCommand.RaiseCanExecuteChanged();
				}
			};
		}

		public ProfileViewModel (
			INavigator navigationService,
			IParseService parseService
		)
		{
			this.parseService = parseService;
			this.navigationService = navigationService;
			teacher = new Teacher ();
			IsLoading = true;
		}
	}
}

