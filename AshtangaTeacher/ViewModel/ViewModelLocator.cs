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
		public const string TeacherProfilePageKey = "TeacherProfilePage";

		public MainViewModel Main {
			get {
				if (!SimpleIoc.Default.IsRegistered<MainViewModel> ()) {
					SimpleIoc.Default.Register<MainViewModel> ();
				}
				return ServiceLocator.Current.GetInstance<MainViewModel> ();
			}
		}

		public ProfileViewModel Profile {
			get {
				if (!SimpleIoc.Default.IsRegistered<ProfileViewModel> ()) {
					SimpleIoc.Default.Register<ProfileViewModel> ();
				}
				return ServiceLocator.Current.GetInstance<ProfileViewModel> ();
			}
		}

		public MainTabsViewModel MainTabs {
			get {
				if (!SimpleIoc.Default.IsRegistered<MainTabsViewModel> ()) {
					SimpleIoc.Default.Register<MainTabsViewModel> ();
				}
				return ServiceLocator.Current.GetInstance<MainTabsViewModel> ();
			}
		}

		public LoginViewModel Login {
			get {
				if (!SimpleIoc.Default.IsRegistered<LoginViewModel> ()) {
					SimpleIoc.Default.Register<LoginViewModel> ();
				}
				return ServiceLocator.Current.GetInstance<LoginViewModel> ();
			}
		}

		public SignUpViewModel SignUp {
			get {
				if (!SimpleIoc.Default.IsRegistered<SignUpViewModel> ()) {
					SimpleIoc.Default.Register<SignUpViewModel> ();
				}
				return ServiceLocator.Current.GetInstance<SignUpViewModel> ();
			}
		}

		public ShalaTeachersViewModel ShalaTeachers {
			get {
				if (!SimpleIoc.Default.IsRegistered<ShalaTeachersViewModel> ()) {
					SimpleIoc.Default.Register<ShalaTeachersViewModel> ();
				}
				return ServiceLocator.Current.GetInstance<ShalaTeachersViewModel> ();
			}
		}

		public static void Reset ()
		{
			// These need to be re-instantiated with each login
			SimpleIoc.Default.Unregister<ShalaTeachersViewModel> ();
			SimpleIoc.Default.Unregister<LoginViewModel> ();
			SimpleIoc.Default.Unregister<SignUpViewModel> ();
		}
	}
}