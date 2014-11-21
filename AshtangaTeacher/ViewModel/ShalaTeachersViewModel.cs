using System;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;

namespace AshtangaTeacher
{
	public class ShalaTeachersViewModel : ViewModelBase
	{
		RelayCommand<Teacher> showTeacherCommand;
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

		readonly ObservableCollection<Teacher> shalaTeachers = new ObservableCollection<Teacher> ();
		public ObservableCollection<Teacher> ShalaTeachers {
			get {
				return shalaTeachers;
			}
		}

		readonly ObservableCollection<Teacher> pendingTeachers = new ObservableCollection<Teacher> ();
		public ObservableCollection<Teacher> PendingTeachers {
			get {
				return pendingTeachers;
			}
		}

		public RelayCommand<Teacher> ShowTeacherCommand {
			get {
				return showTeacherCommand
					?? (showTeacherCommand = new RelayCommand<Teacher> (
						teacher => {
							if (!ShowTeacherCommand.CanExecute (teacher)) {
								return;
							}
							navigationService.NavigateTo(ViewModelLocator.TeacherProfilePageKey, new TeacherProfileViewModel(teacher));
						},
						teacher => teacher != null));

			}
		}

		public void AcceptTeacher (Teacher teacher)
		{
			PendingTeachers.Remove (teacher);
			ShalaTeachers.Add (teacher);
			ShowPendingTeachers = PendingTeachers.Count > 0;
		}

		public void Init(List<Teacher> teachers) 
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


		public ShalaTeachersViewModel (INavigator navigationService)
		{
			this.navigationService = navigationService;
		}
	}
}

