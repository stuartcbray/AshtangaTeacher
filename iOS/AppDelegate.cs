using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Xamarin.Forms;
using GalaSoft.MvvmLight.Ioc;
using Parse;

namespace AshtangaTeacher.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Forms.Init ();

			SimpleIoc.Default.Register<IParseService, ParseService>();
			SimpleIoc.Default.Register<IStudentsService, StudentsService>();
			SimpleIoc.Default.Register<IEmailValidator, EmailValidator>();

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			window.RootViewController = App.GetMainPage ().CreateViewController ();
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

