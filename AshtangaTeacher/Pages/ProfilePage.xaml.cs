using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class ProfilePage : ContentPage
	{	
		public ProfileViewModel ViewModel
		{
			get	{ return (ProfileViewModel)BindingContext; }
		}

		public ProfilePage (ProfileViewModel vm)
		{
			InitializeComponent();
			BindingContext = vm;
			NavigationPage.SetHasNavigationBar (this, false);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing ();
			ViewModel.Init ();
		}
	}
}

