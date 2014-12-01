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
			isLoading = true;
		}

		public async Task Init ()
		{
			await parseService.InitializeRoles ();

			if (parseService.ShowLogin ()) {
 				navigationService.NavigateTo (ViewModelLocator.LoginPageKey, App.Locator.Login);
			} else if (IsLoading) {
				await App.Locator.Profile.InitializeTeacher ();

				if (string.IsNullOrEmpty (App.Locator.Profile.Model.ShalaName)) {
					navigationService.NavigateTo (ViewModelLocator.TeacherInfoPageKey, App.Locator.SignUp);
				} else {
					App.Locator.Main.GetStudentsCommand.Execute (null);
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

