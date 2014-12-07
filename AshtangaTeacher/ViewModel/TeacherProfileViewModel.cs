using Xamarin.Forms;

namespace AshtangaTeacher
{
	public class TeacherProfileViewModel : ViewModelBase
	{
		bool isLoading;
		string errorMessage;
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

		public string ErrorMessage {
			get {
				return errorMessage;
			}
			set {
				Set (() => ErrorMessage, ref errorMessage, value);
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

		public bool IsPendingTeacher {
			get {
				return Model.Role == TeacherRole.Pending && App.Profile.Model.Role == TeacherRole.Administrator;
			}
		}

		public Command AcceptTeacherRequestCommand {
			get {
				return acceptTeacherRequestCommand
					?? (acceptTeacherRequestCommand = new Command (
						async () => {
							IsLoading = true;
							await teacher.UpdateRoleAsync (TeacherRole.Moderator);
							IsLoading = false;
							NavigationService.Instance.GoBack ();
						}));
			}
		}
			
		public TeacherProfileViewModel (ITeacher teacher)
		{
			this.teacher = teacher;
		}
	}
}

