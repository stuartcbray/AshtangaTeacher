using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class MainPage : ContentPage
	{	
		public MainViewModel ViewModel
		{
			get
			{
				return (MainViewModel)BindingContext;
			}
		}

		public MainPage(MainViewModel vm)
		{
			InitializeComponent();

			BindingContext = vm;
			NavigationPage.SetHasNavigationBar (this, false);

			StudentsList.ItemTapped += (s, e) => ViewModel.ShowDetailsCommand.Execute (e.Item);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing ();
			ViewModel.Init ();
		}
	}
}

