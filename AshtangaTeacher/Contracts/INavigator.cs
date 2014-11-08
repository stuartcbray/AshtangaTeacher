using System;
using GalaSoft.MvvmLight.Views;
using Xamarin.Forms;

namespace AshtangaTeacher
{
	public interface INavigator : INavigationService
	{
		void PopToRoot ();
		void SetRootNavigation (NavigationPage navigation);
	}
}

