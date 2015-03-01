using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public class ShalaTeachersViewModel : ViewModelBase
	{
		public bool InitialLoad { get; set; }

		public string ShalaName { get; private set; }

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

		public Command<ITeacher> ShowTeacherCommand {
			get {
				return showTeacherCommand
					?? (showTeacherCommand = new Command<ITeacher> (
						teacher => {
							if (!ShowTeacherCommand.CanExecute (teacher)) {
								return;
							}
							Navigator.NavigateTo(PageLocator.TeacherProfilePageKey, new TeacherProfileViewModel(teacher, Navigator));
						},
						teacher => teacher != null));

			}
		}

		public async Task Init ()
		{
			if (!InitialLoad) {
				IsLoading = true;
				//ShalaTeachers = await App.Profile.Model.GetTeachersAsync (ShalaName);
				IsLoading = false;
				InitialLoad = true;
			}

		}

		// This should wrap an IShala object
		public ShalaTeachersViewModel (string shalaName, NavigationService nav)
		{
			ShalaName = shalaName;
			Navigator = nav;
		}
	}
}

