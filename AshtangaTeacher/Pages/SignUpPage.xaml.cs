using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class SignUpPage : ContentPage
	{	
		public SignUpPage (SignUpViewModel vm)
		{
			InitializeComponent ();
			NavigationPage.SetHasNavigationBar (this, false);
			BindingContext = vm;
		}
	}
}

