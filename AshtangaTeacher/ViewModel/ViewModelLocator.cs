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

		public MainViewModel Main {
			get {
				return ServiceLocator.Current.GetInstance<MainViewModel> ();
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

		static ViewModelLocator ()
		{
			ServiceLocator.SetLocatorProvider (() => SimpleIoc.Default);

			// Need to register these as they take dependencies as parameters
			SimpleIoc.Default.Register<MainViewModel> ();
			SimpleIoc.Default.Register<LoginViewModel> ();
			SimpleIoc.Default.Register<SignUpViewModel> ();
		}
	}
}