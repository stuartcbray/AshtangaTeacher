using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AshtangaTeacher
{
	public class StudentsViewModel : ViewModelBase
	{
		ShalaViewModel shalaViewModel;

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
						() => Navigator.NavigateTo (PageLocator.AddStudentPageKey, new AddStudentViewModel (this, Navigator))));
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
								Students = await shalaViewModel.Model.GetStudentsAsync ();
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
							student.OnDeleted += () => Students.Remove (student);
							Navigator.NavigateTo (PageLocator.StudentDetailsPageKey, student);
						},
						student => student != null));

			}
		}

		public StudentsViewModel (ShalaViewModel shalaViewModel, NavigationService nav)
		{
			this.shalaViewModel = shalaViewModel;
			Navigator = nav;
			IsLoading = true;
		}
	}
}

