using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;

namespace AshtangaTeacher
{
	public class MainViewModel : ViewModelBase
	{
		readonly IStudentsService studentsService;
		readonly INavigator navigationService;

		bool isLoading;
		RelayCommand getStudentsCommand;
		RelayCommand addStudentCommand;
		RelayCommand<StudentViewModel> showDetailsCommand;

		public bool IsLoading {
			get {
				return isLoading;
			}
			set {
				if (Set (() => IsLoading, ref isLoading, value)) {
					RaisePropertyChanged ("IsReady");
				}
			}
		}

		public bool IsReady { 
			get { 
				return !isLoading; 
			} 
		}
			
		ObservableCollection<StudentViewModel> students = new ObservableCollection<StudentViewModel> ();
		public ObservableCollection<StudentViewModel>  Students {
			get {
				return students;
			}
			set {
				Set (() => Students, ref students, value);
			}
		}

		public RelayCommand AddStudentCommand {
			get {
				return addStudentCommand
				?? (addStudentCommand = new RelayCommand (
						() => {

							var student = DependencyService.Get<IStudent>(DependencyFetchTarget.NewInstance);
							student.ShalaName =  App.Locator.Profile.Model.ShalaName;
							student.ExpiryDate = DateTime.Now;

							var vm = new AddStudentViewModel (studentsService, navigationService, student);
							navigationService.NavigateTo (ViewModelLocator.AddStudentPageKey, vm);
					}));
			}
		}

		public RelayCommand GetStudentsCommand {
			get {
				return getStudentsCommand
				?? (getStudentsCommand = new RelayCommand (
					async () => {
							Students.Clear ();
							IsLoading = true;
							try {
								Students = await studentsService.GetAllAsync (App.Locator.Profile.Model.ShalaName);
							} catch (Exception ex) {
								var dialog = ServiceLocator.Current.GetInstance<IDialogService> ();
								await dialog.ShowError (ex, "Error when refreshing", "OK", null);
							}
							IsLoading = false;
					}));
						
			}
		}

		public RelayCommand<StudentViewModel> ShowDetailsCommand {
			get {
				return showDetailsCommand
				?? (showDetailsCommand = new RelayCommand<StudentViewModel> (
					student => {
						if (!ShowDetailsCommand.CanExecute (student)) {
							return;
						}

						navigationService.NavigateTo (ViewModelLocator.StudentDetailsPageKey, student);
					},
					student => student != null));

			}
		}

		public MainViewModel (
			INavigator navigationService,
			IStudentsService studentsService
		)
		{
			this.studentsService = studentsService;
			this.navigationService = navigationService;
			IsLoading = true;
		}
	}
}

