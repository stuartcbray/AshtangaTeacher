using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class LoginPage : ContentPage
	{	
		public LoginPage ()
		{
			InitializeComponent ();
			NavigationPage.SetHasNavigationBar (this, false);
			BindingContext = App.Locator.Login;
		}
	}
}

