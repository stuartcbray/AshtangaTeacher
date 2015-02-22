using Xamarin.Forms;

namespace AshtangaTeacher
{	
	public partial class ShalaPage : ContentPage
	{	
		public ShalaPage (ShalaViewModel vm)
		{
			InitializeComponent ();
			BindingContext = vm;
		}
	}
}

