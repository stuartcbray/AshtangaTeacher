using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AshtangaTeacher
{
	public partial class AddShalaPage : ContentPage
	{
		public AddShalaPage (AddShalaViewModel vm)
		{
			InitializeComponent ();
			BindingContext = vm;
		}
	}
}

