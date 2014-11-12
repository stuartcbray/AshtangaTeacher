using System;
using GalaSoft.MvvmLight;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public class MainTabsViewModel : ViewModelBase
	{
		readonly IParseService parseService;
		readonly INavigator navigationService;

		public IParseService ParseService { get { return parseService; } }

		public MainTabsViewModel (
			INavigator navigationService,
			IParseService parseService
		)
		{
			this.parseService = parseService;
			this.navigationService = navigationService;
		}

		public async Task<bool> Init ()
		{
			bool isReady = false;
			if (parseService.ShowLogin ()) {
				navigationService.NavigateTo (ViewModelLocator.LoginPageKey, App.Locator.Login);
			} else if (string.IsNullOrEmpty (parseService.ShalaName)) {
				navigationService.NavigateTo (ViewModelLocator.TeacherInfoPageKey, App.Locator.SignUp);
			} else {
				await App.Locator.Profile.InitializeTeacher ();
				App.Locator.Main.GetStudentsCommand.Execute (null);
				isReady = true;
			}
			return isReady;
		}

		public void UpdateRootNavigation (NavigationPage page)
		{
			navigationService.SetRootNavigation (page);
		}
	}
}

