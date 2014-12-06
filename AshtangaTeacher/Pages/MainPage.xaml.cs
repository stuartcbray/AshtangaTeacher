using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class MainPage : ContentPage
	{	
		public StudentsViewModel ViewModel
		{
			get
			{
				return (StudentsViewModel)BindingContext;
			}
		}

		public MainPage(StudentsViewModel vm)
		{
			InitializeComponent();

			BindingContext = vm;
			NavigationPage.SetHasNavigationBar (this, false);

			StudentsList.ItemTapped += (s, e) => ViewModel.ShowDetailsCommand.Execute (e.Item);
		}
	}
}

