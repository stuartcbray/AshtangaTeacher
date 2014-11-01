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

		bool isLoading, initialFetch;
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
						initialFetch = false;
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
							Students = await studentsService.GetAllAsync (parseService.CurrentShalaName);
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

		public void Init ()
		{
			if (parseService.ShowLogin ()) {
				navigationService.NavigateTo (ViewModelLocator.LoginPageKey, App.Locator.Login);
			} else if (string.IsNullOrEmpty (parseService.CurrentShalaName)) {
				navigationService.NavigateTo (ViewModelLocator.TeacherInfoPageKey, App.Locator.SignUp);
			} else if (!initialFetch) {
				initialFetch = true;
				GetStudentsCommand.Execute (null);
			}
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
		}
	}
}

