using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Linq;

namespace AshtangaTeacher
{
	public partial class EditProfilePage : ContentPage
	{
		public ProfileViewModel ViewModel
		{
			get	{ return (ProfileViewModel)BindingContext; }
		}

		public EditProfilePage (ProfileViewModel vm)
		{
			InitializeComponent();
			BindingContext = vm;
			NavigationPage.SetHasBackButton (this, false);

			ToolbarItems.Add (new ToolbarItem ("Cancel", "", () => {
				Navigation.PopAsync();
			}));

			ToolbarItems.Add (new ToolbarItem ("Save", "", () => {
				Navigation.PopAsync();
			}, 0, 1));
		}
	}
}

