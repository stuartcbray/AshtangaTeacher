using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using AshtangaTeacher;
using AshtangaTeacher.iOS;
using MonoTouch.FacebookConnect;
using Parse;
using System;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(FacebookLoginButton), typeof(FacebookLoginRenderer))]

namespace AshtangaTeacher.iOS
{
	public class FacebookLoginRenderer : ButtonRenderer
	{

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				UIButton button = Control;

				button.TouchUpInside += delegate
				{
					HandleFacebookLoginClicked();
				};
			}
		}

		async Task ParseLogin()
		{
			var me = await FBRequestConnection.GetMeAsync();

			if (me == null || me.Result == null)
				throw new Exception("Failed to connect to facebook");

			var graphObject = (FBGraphObject) me.Result;

			// Do some initial gathering of data we need
			var name = graphObject.ObjectForKey ("name").ToString ();
			var userId = graphObject.ObjectForKey ("id").ToString ();
			var email = graphObject.ObjectForKey ("email").ToString ();
			var accessToken = FBSession.ActiveSession.AccessTokenData.AccessToken;
			var expiry = DeviceService.NSDateToDateTime (FBSession.ActiveSession.AccessTokenData.ExpirationDate);

			var user = await ParseFacebookUtils.LogInAsync(userId, accessToken, expiry);

			user["name"] = name.Trim ();
			user.Email = email;
			user["facebookImageUrl"] = string.Format("https://graph.facebook.com/{0}/picture?width=300&height=300", userId);
			user["uid"] = Guid.NewGuid().ToString();

			try {
				await user.SaveAsync ();
			}
			catch (Exception ex) {
				await user.DeleteAsync ();
				ParseUser.LogOut ();
				await DialogService.Instance.ShowError (ex.Message, "Login Error", "OK", null);
			}
		}

		async void HandleFacebookLoginClicked()
		{
			if (FBSession.ActiveSession.IsOpen)
			{
				await ParseLogin ();

				App.PostSuccessFacebookAction(FBSession.ActiveSession.AccessTokenData.AccessToken);
			}
			else
			{
				FBSession.ActiveSession.Open(FBSessionLoginBehavior.UseSystemAccountIfPresent, async (aSession, status, error) =>
					{
						if (error == null)
						{
							await ParseLogin ();
							App.PostSuccessFacebookAction(aSession.AccessTokenData.AccessToken);
						}
					});
			}

		}
	}
}
