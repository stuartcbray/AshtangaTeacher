using System;
using System.Collections.ObjectModel;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;
using Xamarin.Forms.Labs.Mvvm;

namespace AshtangaTeacher
{
	public class StudentsViewModel : ViewModelBase
	{
		readonly INavigator navigationService;

		bool isLoading;
		Command getStudentsCommand;
		Command addStudentCommand;
		Command<StudentViewModel> showDetailsCommand;

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
			
		ObservableCollection<StudentViewModel> students = new ObservableCollection<StudentViewModel> ();
		public ObservableCollection<StudentViewModel>  Students {
			get {
				return students;
			}
			set {
				Set (() => Students, ref students, value);
			}
		}

		public Command AddStudentCommand {
			get {
				return addStudentCommand
				?? (addStudentCommand = new Command (
						() => navigationService.NavigateTo (PageLocator.AddStudentPageKey, new AddStudentViewModel ())));
			}
		}

		public Command GetStudentsCommand {
			get {
				return getStudentsCommand
				?? (getStudentsCommand = new Command (
					async () => {
							Students.Clear ();
							IsLoading = true;
							try {
								Students = await App.Profile.Model.GetStudentsAsync ();
							} catch (Exception ex) {
								await DialogService.Instance.ShowError (ex, "Error when refreshing", "OK", null);
							}
							IsLoading = false;
					}));
						
			}
		}

		public Command<StudentViewModel> ShowDetailsCommand {
			get {
				return showDetailsCommand
				?? (showDetailsCommand = new Command<StudentViewModel> (
					student => {
						if (!ShowDetailsCommand.CanExecute (student)) {
							return;
						}

							navigationService.NavigateTo (PageLocator.StudentDetailsPageKey, student);
					},
					student => student != null));

			}
		}

		public StudentsViewModel ()
		{
			navigationService = NavigationService.Instance;
			IsLoading = true;
		}
	}
}

