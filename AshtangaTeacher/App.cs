using Xamarin.Forms;
using System;

namespace AshtangaTeacher
{
    public class App : Application
    {
        const string AppId = "";
        const string DotNetId = "";
        public const string FacebookAppId = "";
        public const string FacebookAppDisplayName = "Ashtanga Teacher";

        public static Action<string> PostSuccessFacebookAction { get; set; } 
            
        public readonly NavigationService RootNavigator;

        public static App Instance { get { return (App) Application.Current; } }
    
        public App ()
        {
            var parseService = DependencyService.Get<IParseService> ();
            parseService.Initialize (AppId, DotNetId, FacebookAppId);

            RootNavigator = new NavigationService ();
            var rootNavPage = new NavigationPage (new LoginPage(new LoginViewModel(RootNavigator)));
            RootNavigator.Initialize(rootNavPage, PageLocator.PagesByKey);

            DialogService.Instance.Initialize (rootNavPage);

            MainPage = rootNavPage;
        }
    }
}
