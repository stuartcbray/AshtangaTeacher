using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class TeacherInfoPage : ContentPage
	{	
		public TeacherInfoPage (SignUpViewModel vm)
		{
			InitializeComponent ();
			vm.Reset ();
			BindingContext = vm;
		}
	}
}

