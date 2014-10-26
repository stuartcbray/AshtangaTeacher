using System;
using GalaSoft.MvvmLight.Views;

namespace AshtangaTeacher
{
	public interface INavigator : INavigationService
	{
		void PopToRoot ();
	}
}

