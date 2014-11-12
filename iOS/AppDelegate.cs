using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Xamarin.Forms;
using GalaSoft.MvvmLight.Ioc;
using Parse;
using Xamarin.Forms.Labs.iOS;

namespace AshtangaTeacher.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Forms.Init ();

			SimpleIoc.Default.Register<IParseService, ParseService>();
			SimpleIoc.Default.Register<IStudentsService, StudentsService>();
			SimpleIoc.Default.Register<IDeviceService, DeviceService>();
			SimpleIoc.Default.Register<ICameraService, CameraService>();

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			window.RootViewController = App.GetMainPage ().CreateViewController ();
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

