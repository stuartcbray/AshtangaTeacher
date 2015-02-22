using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace AshtangaTeacher
{
	public class ShalasViewModel : ViewModelBase
	{
		bool isLoading;

		Command getShalasCommand;
		Command<ShalaViewModel> shalaDetailsCommand;

		public ShalaViewModel CurrentShala {
			get;
			private set;
		}

		public bool IsLoading {
			get {
				return isLoading;
			}
			set {
				if (Set ("IsLoading", ref isLoading, value)) {
					OnPropertyChanged ("IsReady");
				}
			}
		}

		public bool IsReady { 
			get { 
				return !isLoading; 
			} 
		}

		ObservableCollection<ShalaViewModel> shalas = new ObservableCollection<ShalaViewModel> ();
		public ObservableCollection<ShalaViewModel>  Shalas {
			get {
				return shalas;
			}
			set {
				Set (() => Shalas, ref shalas, value);
			}
		}

		public Command GetShalasCommand {
			get {
				return getShalasCommand
					?? (getShalasCommand = new Command (
						async () => {
							Shalas.Clear ();
							IsLoading = true;
							try {
								//Shalas = await App.Instance.ProfileVm.Model.GetShalasAsync ();
							} catch (Exception ex) {
								await DialogService.Instance.ShowError (ex, "Error when refreshing", "OK", null);
							}
							IsLoading = false;
						}));

			}
		}

		public Command ShalaDetailsCommand {
			get {
				return shalaDetailsCommand
					?? (shalaDetailsCommand = new Command<ShalaViewModel> (
						shala => {
							if (!ShalaDetailsCommand.CanExecute (shala)) {
								return;
							}

							CurrentShala = shala;

							Navigator.NavigateTo (PageLocator.ShalaPageKey, shala);
						},
						shala => shala != null));

			}
		}

		public ShalasViewModel()
		{
			Navigator = new NavigationService ();
		}
	}
}

