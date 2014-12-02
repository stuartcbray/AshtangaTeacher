using System;
using System.Json;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Auth;
using Xamarin.Forms;
using AshtangaTeacher.iOS;
using AshtangaTeacher;
using Parse;
using Facebook;
using System.Collections.Generic;


[assembly: ExportRenderer (typeof (FacebookLoginPage), typeof (FacebookLoginRenderer))]

namespace AshtangaTeacher.iOS
{
	// Inspired from https://github.com/jsauve/OAuthTwoDemo.XForms
	public class FacebookLoginRenderer : PageRenderer
	{
	
		bool hasShown;

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (!hasShown)
			{
				hasShown = true;
				var auth = new OAuth2Authenticator (
					clientId: App.FacebookAppId,
					scope: "email",
					authorizeUrl: new Uri ("https://m.facebook.com/dialog/oauth/"),
					redirectUrl: new Uri ("http://www.facebook.com/connect/login_success.html"));

				auth.Completed += async (sender, e) => {

					DismissViewController (true, null);

					if (e.IsAuthenticated) {
						var accessToken = e.Account.Properties["access_token"].ToString ();
						var expiresIn = Convert.ToDouble(e.Account.Properties["expires_in"]);
						var expiryDate = DateTime.Now + TimeSpan.FromSeconds (expiresIn);

						// Now that we're logged in, make a OAuth2 request to get the user's id
						var request = new OAuth2Request ("GET", new Uri ("https://graph.facebook.com/me"), null, e.Account);
						var response = await request.GetResponseAsync();

						var obj = JsonValue.Parse (response.GetResponseText());
						var id = obj["id"].ToString().Replace("\"",""); 

						var user = await ParseFacebookUtils.LogInAsync(id, accessToken, expiryDate);
						var firstName = obj["first_name"].ToString().Replace("\"",""); 
						var lastName = obj["last_name"].ToString().Replace("\"",""); 
						var email = obj["email"].ToString().Replace("\"",""); 
						var name = firstName + " " + lastName;
						user["name"] = name.Trim ();
						user.Email = email;
						user["facebookImageUrl"] = string.Format("https://graph.facebook.com/{0}/picture?width=300&height=300", id);
						user["uid"] = Guid.NewGuid().ToString();

						try {
							await user.SaveAsync ();
						}
						catch (Exception ex) {
							await user.DeleteAsync ();
							ParseUser.LogOut ();
							App.Locator.Login.ErrorMessage = ex.Message;
						}
					} 

					App.Locator.Login.FacebookLoginSuccessCommand.Execute (null); 
				};

				PresentViewController (auth.GetUI (), true, null);

			}
		}
	}

}

