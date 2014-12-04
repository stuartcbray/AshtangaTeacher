using System.Collections.ObjectModel;
using System.Collections.Generic;
using Xamarin.Forms.Labs.Mvvm;
using Xamarin.Forms;

namespace AshtangaTeacher
{
	public class ShalaTeachersViewModel : ViewModelBase
	{
		Command<ITeacher> showTeacherCommand;
		INavigator navigationService;

		bool showPendingTeachers;
		public bool ShowPendingTeachers {
			get {
				return showPendingTeachers;
			}
			set {
				Set (() => ShowPendingTeachers, ref showPendingTeachers, value);
			}
		}

		readonly ObservableCollection<ITeacher> shalaTeachers = new ObservableCollection<ITeacher> ();
		public ObservableCollection<ITeacher> ShalaTeachers {
			get {
				return shalaTeachers;
			}
		}

		readonly ObservableCollection<ITeacher> pendingTeachers = new ObservableCollection<ITeacher> ();
		public ObservableCollection<ITeacher> PendingTeachers {
			get {
				return pendingTeachers;
			}
		}

		public Command<ITeacher> ShowTeacherCommand {
			get {
				return showTeacherCommand
					?? (showTeacherCommand = new Command<ITeacher> (
						teacher => {
							if (!ShowTeacherCommand.CanExecute (teacher)) {
								return;
							}
							navigationService.NavigateTo(ViewModelLocator.TeacherProfilePageKey, new TeacherProfileViewModel(teacher));
						},
						teacher => teacher != null));

			}
		}

		public void AcceptTeacher (ITeacher teacher)
		{
			PendingTeachers.Remove (teacher);
			ShalaTeachers.Add (teacher);
			ShowPendingTeachers = PendingTeachers.Count > 0;
		}

		public void Init(List<ITeacher> teachers) 
		{
			ShalaTeachers.Clear ();
			PendingTeachers.Clear ();

			foreach (var t in teachers) {
				if (t.Role != TeacherRole.None)
					ShalaTeachers.Add (t);
				else
					PendingTeachers.Add (t);
			}

			ShowPendingTeachers = PendingTeachers.Count > 0;
		}


		public ShalaTeachersViewModel ()
		{
			navigationService = NavigationService.Instance;
		}
	}
}

