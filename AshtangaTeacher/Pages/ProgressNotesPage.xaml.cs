using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class ProgressNotesPage : ContentPage
	{	
		public ProgressNotesPage (StudentViewModel vm)
		{
			InitializeComponent ();
			BindingContext = vm;
		}
	}
}

