using System;
using Xamarin.Forms;
using XLabs.Platform.Services.Media;
using XLabs.Platform.Device;

namespace AshtangaTeacher
{
	public class ShalaViewModel : ViewModelBase
	{
		bool isLoading;
		string errorMessage;

		Command saveShalaCommand;
		Command deleteShalaCommand;
		Command updatePhotoCommand;
		Command showStudentsCommand;

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
					saveShalaCommand.ChangeCanExecute ();
					OnPropertyChanged ("IsReady");
				}
			}
		}

		public bool IsReady {
			get {
				return !isLoading;
			}
		}

		public IShala Model {
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

		public Command ShowStudentsCommand {
			get {
				return showStudentsCommand
					?? (showStudentsCommand = new Command (
						() => Navigator.NavigateTo (PageLocator.StudentsPageKey, new StudentsViewModel(this, Navigator))));
			}
		}

		public Command SaveShalaCommand {
			get {
				return saveShalaCommand
					?? (saveShalaCommand = new Command (
						async () => {
							IsLoading = true;
							await Model.SaveAsync ();
							IsLoading = false;
							Navigator.GoBack ();
						}, 
						() => Model.IsDirty));
			}
		}

		public Command DeleteShalaCommand {
			get {
				return deleteShalaCommand
					?? (deleteShalaCommand = new Command (
						async () => {
							IsLoading = true;
							await Model.DeleteAsync ();
							MainTabsViewModel.Instance.ShalasVm.Shalas.Remove (this);
							IsLoading = false;
							Navigator.GoBack ();
						}));
			}
		}

		public ShalaViewModel (IShala model, NavigationService nav)
		{
			Navigator = nav;
			Model = model;
			Model.IsDirtyChanged += () => { 
				SaveShalaCommand.ChangeCanExecute();
			};
		}
	}
}

