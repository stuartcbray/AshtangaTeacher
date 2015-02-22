using System;
using Xamarin.Forms;
using XLabs.Platform.Services.Media;

namespace AshtangaTeacher
{
	public class AddShalaViewModel : ViewModelBase
	{
		bool isLoading, isPhotoVisible;

		ShalasViewModel shalasViewModel;

		readonly IShala shala;

		ImageSource imageSource;
		IMediaPicker mediaPicker;

		Command addShalaCommand;
		Command cancelCommand;
		Command addShalaPhotoCommand;

		string errorMessage;

		public IShala Model { get { return shala; } }

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
					?? (cancelCommand = new Command (() => Navigator.GoBack ()));
			}
		}

		public Command AddShalaPhotoCommand {
			get {
				return addShalaPhotoCommand
					?? (addShalaPhotoCommand = new Command (
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
								var thumb = await cameraService.GetThumbAsync(imageSource, shala.UID);
								IsLoading = false;

								shala.Image = thumb;
							}
							catch (System.Exception ex)
							{
								ErrorMessage = ex.Message;
							}
						}));
			}
		}

		public Command AddShalaCommand {
			get {
				return addShalaCommand
					?? (addShalaCommand = new Command (
						async () => {

							// Perform some simple validation...
							if (string.IsNullOrEmpty (Model.Name)) {
								ErrorMessage = "Shala Name is empty";
								return;
							}

						

							if (Model.Image == null) {
								ErrorMessage = "Image is empty";
								return;
							}

							IsLoading = true;
							await shala.SaveAsync ();
							IsLoading = false;

							shalasViewModel.Shalas.Add (new ShalaViewModel (shala, Navigator));

							Navigator.GoBack ();

						}));
			}
		}

		public AddShalaViewModel (ShalasViewModel shalasViewModel, NavigationService nav)
		{
			this.shalasViewModel = shalasViewModel;
			Navigator = nav;
			shala = DependencyService.Get<IShala>(DependencyFetchTarget.NewInstance);
		}
	}
}

