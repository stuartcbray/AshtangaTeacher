using System;
using Xamarin.Forms;
using XLabs.Platform.Services.Media;

namespace AshtangaTeacher
{
	public class AddShalaViewModel : ViewModelBase
	{
		bool isPhotoVisible;

		ShalasViewModel shalasViewModel;

		readonly IShala shala;

		ImageSource imageSource;
		IMediaPicker mediaPicker;

		Command addShalaCommand;
		Command cancelCommand;
		Command addShalaPhotoCommand;

		public IShala Model { get { return shala; } }

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

