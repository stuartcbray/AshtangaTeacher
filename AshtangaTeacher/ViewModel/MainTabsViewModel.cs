using System;
using GalaSoft.MvvmLight;
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
					RaisePropertyChanged ("IsReady");
				}
			}
		}

		public bool IsReady {
			get {
				return !isLoading;
			}
		}

		public MainTabsViewModel (
			INavigator navigationService,
			IParseService parseService
		)
		{
			this.parseService = parseService;
			this.navigationService = navigationService;
			IsLoading = true;
		}

		public async Task Init ()
		{
			await parseService.InitializeRoles ();

			if (parseService.ShowLogin ()) {
 				navigationService.NavigateTo (ViewModelLocator.LoginPageKey, App.Locator.Login);
			} else if (string.IsNullOrEmpty (parseService.ShalaName)) {
				navigationService.NavigateTo (ViewModelLocator.TeacherInfoPageKey, App.Locator.SignUp);
			} else if (!IsReady) {
				await App.Locator.Profile.InitializeTeacher ();
				App.Locator.Main.GetStudentsCommand.Execute (null);
				IsLoading = false;
				App.Locator.Profile.IsLoading = false;
				App.Locator.Main.IsLoading = false;
			}
		}

		public void UpdateRootNavigation (NavigationPage page)
		{
			navigationService.SetRootNavigation (page);
		}
	}
}

