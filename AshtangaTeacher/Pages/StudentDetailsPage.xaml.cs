using System;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class StudentDetailsPage : ContentPage
	{	
		public StudentDetailsPage (StudentViewModel vm)
		{
			InitializeComponent();
			BindingContext = vm;
		}
	}
}

