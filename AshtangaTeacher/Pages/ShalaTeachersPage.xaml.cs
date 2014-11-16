using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class ShalaTeachersPage : ContentPage
	{	
		public ShalaTeachersPage (ShalaTeachersViewModel vm)
		{
			InitializeComponent ();
			BindingContext = vm;
		}
	}
}

