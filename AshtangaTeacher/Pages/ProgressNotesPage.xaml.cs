using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class ProgressNotesPage : ContentPage
	{	
		public StudentViewModel ViewModel
		{
			get	{ return (StudentViewModel)BindingContext; }
		}

		public ProgressNotesPage (StudentViewModel vm)
		{
			InitializeComponent ();
			BindingContext = vm;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing ();
			await ViewModel.GetProgressNotesAsync ();
		}
	}
}

