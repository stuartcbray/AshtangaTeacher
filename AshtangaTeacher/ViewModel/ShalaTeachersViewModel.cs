using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public class ShalaTeachersViewModel : ViewModelBase
	{
		bool isLoading;

		public bool InitialLoad { get; set; }

		Command<ITeacher> showTeacherCommand;

		ObservableCollection<ITeacher> shalaTeachers = new ObservableCollection<ITeacher> ();
		public ObservableCollection<ITeacher>  ShalaTeachers {
			get {
				return shalaTeachers;
			}
			set {
				Set (() => ShalaTeachers, ref shalaTeachers, value);
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

		public Command<ITeacher> ShowTeacherCommand {
			get {
				return showTeacherCommand
					?? (showTeacherCommand = new Command<ITeacher> (
						teacher => {
							if (!ShowTeacherCommand.CanExecute (teacher)) {
								return;
							}
							NavigationService.Instance.NavigateTo(PageLocator.TeacherProfilePageKey, new TeacherProfileViewModel(teacher));
						},
						teacher => teacher != null));

			}
		}

		public async Task Init ()
		{
			if (!InitialLoad) {
				IsLoading = true;
				ShalaTeachers = await App.Profile.Model.GetTeachersAsync ();
				IsLoading = false;
				InitialLoad = true;
			}
		}
	}
}

