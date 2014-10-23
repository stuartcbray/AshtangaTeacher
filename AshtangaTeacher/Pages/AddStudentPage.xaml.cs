using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class AddStudentPage : ContentPage
	{	
		public AddStudentPage (AddStudentViewModel vm)
		{
			InitializeComponent ();
			BindingContext = vm;
		}
	}
}

