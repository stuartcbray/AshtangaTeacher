using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class ShalasPage : ContentPage
	{	
		public ShalasViewModel ViewModel
		{
			get
			{
				return (ShalasViewModel)BindingContext;
			}
		}

		public ShalasPage (ShalasViewModel vm)
		{
			InitializeComponent ();

			BindingContext = vm;
			NavigationPage.SetHasNavigationBar (this, false);

			ShalasList.ItemTapped += (s, e) => ViewModel.ShalaDetailsCommand.Execute (e.Item);
		}
	}
}

