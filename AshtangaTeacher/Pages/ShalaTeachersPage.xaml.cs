using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class ShalaTeachersPage : ContentPage
	{	
		public ShalaTeachersViewModel ViewModel
		{
			get
			{
				return (ShalaTeachersViewModel)BindingContext;
			}
		}

		public ShalaTeachersPage (ShalaTeachersViewModel vm)
		{
			InitializeComponent ();
			BindingContext = vm;
			Title = App.Locator.Profile.Model.ShalaName +  " Teachers";

			TeachersList.ItemTapped += (s, e) => ViewModel.ShowTeacherCommand.Execute (e.Item);
			PendingTeachersList.ItemTapped += (s, e) => ViewModel.ShowTeacherCommand.Execute (e.Item);
		}
	}
}

