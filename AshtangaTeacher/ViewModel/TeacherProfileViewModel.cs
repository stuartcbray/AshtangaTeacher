using Xamarin.Forms;

namespace AshtangaTeacher
{
	public class TeacherProfileViewModel : ViewModelBase
	{
		ITeacher teacher;

		Command acceptTeacherRequestCommand;

		public ITeacher Model {
			get {
				return teacher;
			}
			set {
				Set (() => Model, ref teacher, value);
			}
		}

		public bool IsPendingTeacher {
			get {
				return Model.Role == TeacherRole.Pending && MainTabsViewModel.Instance.ProfileVm.Model.Role == TeacherRole.Administrator;
			}
		}

		public Command AcceptTeacherRequestCommand {
			get {
				return acceptTeacherRequestCommand
					?? (acceptTeacherRequestCommand = new Command (
						async () => {
							IsLoading = true;
							IsLoading = false;
							Navigator.GoBack ();
						}));
			}
		}
			
		public TeacherProfileViewModel (ITeacher teacher, NavigationService nav)
		{
			this.teacher = teacher;
			Navigator = nav;
		}
	}
}

