using Xamarin.Forms;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public class MainTabsViewModel : ViewModelBase
	{
		bool isLoading;

		readonly IParseService parseService;
		readonly INavigator navigationService;

		public IParseService ParseService { get { return parseService; } }

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

		public MainTabsViewModel ()
		{
			parseService = DependencyService.Get<IParseService>();
			navigationService = NavigationService.Instance;
			isLoading = true;
		}

		public async Task Init ()
		{
			await parseService.InitializeRoles ();

			if (parseService.ShowLogin ()) {
				navigationService.NavigateTo (PageLocator.LoginPageKey, new LoginViewModel ());
			} else if (IsLoading) {
				await App.Profile.InitializeTeacher ();

				if (string.IsNullOrEmpty (App.Profile.Model.ShalaName)) {
					navigationService.NavigateTo (PageLocator.TeacherInfoPageKey, new SignUpViewModel ());
				} else {
					App.Students.GetStudentsCommand.Execute (null);
					IsLoading = false;
				}
			}
		}

		public void UpdateRootNavigation (NavigationPage page)
		{
			navigationService.SetRootNavigation (page);
		}
	}
}

