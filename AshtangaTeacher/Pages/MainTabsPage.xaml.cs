using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class MainTabsPage : TabbedPage
	{	
		public MainTabsViewModel ViewModel
		{
			get	{ return (MainTabsViewModel)BindingContext; }
		}

		public MainTabsPage(MainTabsViewModel vm)
		{
			InitializeComponent();
			BindingContext = vm;
			NavigationPage.SetHasNavigationBar (this, false);
			this.Children.Add(new NavigationPage(new MainPage(App.Locator.Main)) { Title="Students", Icon="students.png" });
			this.Children.Add(new NavigationPage(new ProfilePage(App.Locator.Profile)) { Title="My Profile", Icon="profile.png" });
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing ();
			await ViewModel.Init ();

			if (ViewModel.IsReady) {
				ViewModel.UpdateRootNavigation (CurrentPage as NavigationPage);
			}
		}

		protected override void OnCurrentPageChanged()
		{
			base.OnCurrentPageChanged ();
			var navPage = CurrentPage as NavigationPage;
			if (navPage != null) {
				ViewModel.UpdateRootNavigation (navPage);
			}
		}
	}
}

