using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class ResetPasswordPage : ContentPage
	{	
		public ResetPasswordPage (SignUpViewModel vm)
		{
			InitializeComponent ();
			NavigationPage.SetHasNavigationBar (this, false);
			BindingContext = vm;
		}
	}
}

