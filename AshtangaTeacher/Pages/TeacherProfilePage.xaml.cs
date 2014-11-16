using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class TeacherProfilePage : ContentPage
	{	
		public TeacherProfilePage (TeacherProfileViewModel vm)
		{
			InitializeComponent();
			BindingContext = vm;
		}
	}
}

