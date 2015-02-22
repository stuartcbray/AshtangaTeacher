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

			ViewModel.InitializeTabViewModels ();

			var shalasNavPage = new NavigationPage (new ShalasPage (ViewModel.ShalasVm)) { Title = "Shalas", Icon = "shalas.png" };
			ViewModel.ShalasVm.Navigator.Initialize(shalasNavPage, PageLocator.PagesByKey);

			var profileNavPage = new NavigationPage (new ProfilePage (ViewModel.ProfileVm)) { Title = "My Profile", Icon = "profile.png" };
			ViewModel.ProfileVm.Navigator.Initialize(profileNavPage, PageLocator.PagesByKey);

			Children.Add(shalasNavPage);	
			Children.Add(profileNavPage);

			CurrentPage = profileNavPage;
		}
	}
}

