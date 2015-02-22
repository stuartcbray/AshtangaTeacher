using Foundation;
using UIKit;

using Xamarin.Forms;
using XLabs.Forms;
using MonoTouch.FacebookConnect;
using XLabs.Platform.Device;
using XLabs.Ioc;

namespace AshtangaTeacher.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate
	{

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			SetIoc();

			global::Xamarin.Forms.Forms.Init ();

			FBSettings.DefaultAppID = App.FacebookAppId;
			FBSettings.DefaultDisplayName = App.FacebookAppDisplayName;

			LoadApplication (new App ());  // method is new in 1.3
			return base.FinishedLaunching (app, options);
		}

		public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			base.OpenUrl(application, url, sourceApplication, annotation);
			return FBSession.ActiveSession.HandleOpenURL(url);
		}

		public override void OnActivated(UIApplication application)
		{
			base.OnActivated(application);
			// We need to properly handle activation of the application with regards to SSO
			// (e.g., returning from iOS 6.0 authorization dialog or from fast app switching).
			FBSession.ActiveSession.HandleDidBecomeActive();
		}
			
		void SetIoc ()
		{
			var resolverContainer = new SimpleContainer ();
			resolverContainer.Register<IDevice> (t => AppleDevice.CurrentDevice)
				.Register<IDisplay> (t => t.Resolve<IDevice> ().Display)
				.Register<IDependencyContainer> (t => resolverContainer);
			Resolver.SetResolver (resolverContainer.GetResolver ());
		}
	}
}

