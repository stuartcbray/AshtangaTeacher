using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class LoginPage : ContentPage
	{	
		LoginViewModel ViewModel
		{
			get {
				return (LoginViewModel)BindingContext;
			}
		}

		public LoginPage (LoginViewModel vm)
		{
			InitializeComponent ();
			NavigationPage.SetHasNavigationBar (this, false);
			BindingContext = vm;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			ViewModel.Init ();
		}
	}
}

