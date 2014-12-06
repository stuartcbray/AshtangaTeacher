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
			Title = App.Profile.Model.ShalaName +  " Teachers";

			TeachersList.ItemTapped += (s, e) => ViewModel.ShowTeacherCommand.Execute (e.Item);
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing ();
			await ViewModel.Init ();
		}
	}
}

