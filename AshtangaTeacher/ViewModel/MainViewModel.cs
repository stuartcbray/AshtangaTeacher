using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace AshtangaTeacher
{
	public class MainViewModel : ViewModelBase
	{
		readonly IStudentsService studentsService;
		readonly IParseService parseService;
		readonly INavigator navigationService;

		bool isLoading;
		RelayCommand getStudentsCommand;
		RelayCommand logOutCommand;
		RelayCommand addStudentCommand;
		RelayCommand<StudentViewModel> showDetailsCommand;

		public IParseService ParseService { get { return parseService; } }


		public bool IsLoading {
			get {
				return isLoading;
			}
			set {
				Set (() => IsLoading, ref isLoading, value);
			}
		}

		public ObservableCollection<StudentViewModel> Students {
			get;
			private set;
		}

		public RelayCommand AddStudentCommand {
			get {
				return addStudentCommand
				?? (addStudentCommand = new RelayCommand (
					() => {
							var student = new Student { 
								ShalaName = parseService.CurrentShalaName,
								ExpiryDate = DateTime.Now
							};
							var vm = new AddStudentViewModel (studentsService, navigationService, student);
						navigationService.NavigateTo (ViewModelLocator.AddStudentPageKey, vm);
					}));
			}
		}

		public RelayCommand LogOutCommand {
			get {
				return logOutCommand
				?? (logOutCommand = new RelayCommand (
					async () => {
						Students.Clear ();
						await parseService.LogOutAsync ();
						navigationService.NavigateTo (ViewModelLocator.LoginPageKey, App.Locator.Login);
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
							var list = await studentsService.GetAllAsync (parseService.CurrentShalaName);

							foreach (var student in list) {
								Students.Add (new StudentViewModel (studentsService, student));
							}

							IsLoading = false;
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

		public void EnsureAuthenticated ()
		{
			if (parseService.ShowLogin ()) {
				navigationService.NavigateTo (ViewModelLocator.LoginPageKey, App.Locator.Login);
			} 
			// Check here if the user has a valid Shala - new user from Facebook?
		}

		public MainViewModel (
			INavigator navigationService,
			IParseService parseService,
			IStudentsService studentsService
		)
		{
			this.parseService = parseService;
			this.studentsService = studentsService;
			this.navigationService = navigationService;
			Students = new ObservableCollection<StudentViewModel> ();
		}
	}
}

