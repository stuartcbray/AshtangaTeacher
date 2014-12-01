﻿using System;
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

		ITeacher teacher;
		readonly IParseService parseService;
		readonly INavigator navigationService;

		ImageSource imageSource;
		IMediaPicker mediaPicker;

		RelayCommand shalaTeachersCommand;
		RelayCommand addTeacherPhotoCommand;
		RelayCommand saveTeacherCommand;
		RelayCommand logOutCommand;

		public ITeacher Model {
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
							await Model.SaveAsync ();
							IsLoading = false;
						}, 
						() => Model.IsDirty && IsReady));
			}
		}

		public RelayCommand LogOutCommand {
			get {
				return logOutCommand
					?? (logOutCommand = new RelayCommand (
						async () => {
							await parseService.LogOutAsync ();
							navigationService.SetRootNavigation(App.RootNavPage);

							// Reset view models and ensure we re-load the new Teacher 
							App.Locator.MainTabs.IsLoading = true;
							ViewModelLocator.Reset ();

							navigationService.NavigateTo (ViewModelLocator.LoginPageKey, App.Locator.Login);
						}));
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
			IsLoading = true;
			await teacher.InitializeAsync (parseService.CurrentUser);
			IsPhotoVisible = Model.Image != null;
			IsLoading = false;
		}

		public ProfileViewModel (
			INavigator navigationService,
			IParseService parseService
		)
		{
			this.parseService = parseService;
			this.navigationService = navigationService;
			teacher = DependencyService.Get<ITeacher> (DependencyFetchTarget.NewInstance);

			teacher.PropertyChanged += (sender, e) => { 
				if (e.PropertyName == "IsDirty") {
					SaveTeacherCommand.RaiseCanExecuteChanged();
				}
			};
		}
	}
}

