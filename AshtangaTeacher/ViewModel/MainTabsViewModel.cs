using System;
using GalaSoft.MvvmLight;
using Xamarin.Forms;

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

		public bool Init ()
		{
			bool isReady = false;
			if (parseService.ShowLogin ()) {
				navigationService.NavigateTo (ViewModelLocator.LoginPageKey, App.Locator.Login);
			} else if (string.IsNullOrEmpty (parseService.CurrentShalaName)) {
				navigationService.NavigateTo (ViewModelLocator.TeacherInfoPageKey, App.Locator.SignUp);
			} else {
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

