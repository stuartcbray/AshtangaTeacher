using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class StudentsPage : ContentPage
	{	
		public StudentsViewModel ViewModel
		{
			get
			{
				return (StudentsViewModel)BindingContext;
			}
		}

		public StudentsPage(StudentsViewModel vm)
		{
			InitializeComponent();

			BindingContext = vm;
			NavigationPage.SetHasNavigationBar (this, false);

			StudentsList.ItemTapped += (s, e) => ViewModel.ShowDetailsCommand.Execute (e.Item);
		}
	}
}

