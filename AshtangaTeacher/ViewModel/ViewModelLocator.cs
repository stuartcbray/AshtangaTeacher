using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace AshtangaTeacher
{
	public class ViewModelLocator
	{
		public const string AddProgressNotePageKey = "AddProgressNotePage";
		public const string ProgressNotesPageKey = "ProgressNotesPage";
		public const string AddStudentPageKey = "AddStudentPage";
		public const string StudentDetailsPageKey = "StudentDetailsPage";
		public const string MainPageKey = "MainPage";
		public const string LoginPageKey = "LoginPage";
		public const string SignUpPageKey = "SignUpPage";
		public const string FacebookSignInKey = "FacebookLoginPage";
		public const string TeacherInfoPageKey = "TeacherInfoPage";
		public const string MainTabsPageKey = "MainTabsPage";
		public const string ProfilePageKey = "ProfilePage";
		public const string ShalaTeachersPageKey = "ShalaTeachersPage";

		public MainViewModel Main {
			get {
				return ServiceLocator.Current.GetInstance<MainViewModel> ();
			}
		}

		public ProfileViewModel Profile {
			get {
				return ServiceLocator.Current.GetInstance<ProfileViewModel> ();
			}
		}

		public MainTabsViewModel MainTabs {
			get {
				return ServiceLocator.Current.GetInstance<MainTabsViewModel> ();
			}
		}

		public LoginViewModel Login {
			get {
				return ServiceLocator.Current.GetInstance<LoginViewModel> ();
			}
		}

		public SignUpViewModel SignUp {
			get {
				return ServiceLocator.Current.GetInstance<SignUpViewModel> ();
			}
		}

		public ShalaTeachersViewModel ShalaTeachers {
			get {
				return ServiceLocator.Current.GetInstance<ShalaTeachersViewModel> ();
			}
		}

		static ViewModelLocator ()
		{
			ServiceLocator.SetLocatorProvider (() => SimpleIoc.Default);

			// Need to register these as they take dependencies as parameters
			SimpleIoc.Default.Register<MainViewModel> ();
			SimpleIoc.Default.Register<MainTabsViewModel> ();
			SimpleIoc.Default.Register<ProfileViewModel> ();
			SimpleIoc.Default.Register<ShalaTeachersViewModel> ();
			SimpleIoc.Default.Register<LoginViewModel> ();
			SimpleIoc.Default.Register<SignUpViewModel> ();
		}
	}
}