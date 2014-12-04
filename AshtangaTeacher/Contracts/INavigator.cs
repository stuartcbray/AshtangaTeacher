using Xamarin.Forms;

namespace AshtangaTeacher
{
	public interface INavigator
	{
		string CurrentPageKey { get; }

		void PopToRoot ();

		void GoBack ();

		void NavigateTo (string pageKey);

		void NavigateTo (string pageKey, object parameter);
	
		void SetRootNavigation (NavigationPage navigation);
	}
}

