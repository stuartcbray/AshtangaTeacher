using System;
using Xamarin.Forms;

namespace AshtangaTeacher
{
	public class LoginViewModel : ViewModelBase
	{
		bool isLoading;
		string email;
		string passWord;
		string errorMessage;

		Command signUpCommand;
		Command signInCommand;
		Command resetPasswordCommand;
		Command facebookSignInCommand;

		readonly IParseService parseService;

		public IParseService ParseService { get { return parseService; } }

		public string ApplicationName { get { return "Ashtanga Teacher"; } }

		public bool IsLoading {
			get {
				return isLoading;
			}
			set {
				if (Set (() => IsLoading, ref isLoading, value)) {
					OnPropertyChanged ("IsReady");
				}
			}
		}

		public bool IsReady {
			get {
				return !isLoading;
			}
		}

		public bool SignInVisible {
			get {
				return parseService.ShowLogin ();
			}
		}

		public string Email {
			get {
				return email;
			}
			set {
				Set (() => Email, ref email, value);
			}
		}
			
		public string Password {
			get {
				return passWord;
			}
			set {
				Set (() => Password, ref passWord, value);
			}
		}
			
		public string ErrorMessage {
			get {
				return errorMessage;
			}
			set {
				Set (() => ErrorMessage, ref errorMessage, value);
			}
		}

		public Command ResetPasswordCommand {
			get {
				return resetPasswordCommand
					?? (resetPasswordCommand = new Command (
						() => {
							Navigator.NavigateTo (PageLocator.ResetPasswordPageKey, new SignUpViewModel (Navigator));
						}
					));
			}
		}

		public Command SignInCommand {
			get {
				return signInCommand
				?? (signInCommand = new Command (
					async () => {
							if (!IsLoading) {
								IsLoading = true;
								try {
									await parseService.SignInAsync (Email, Password);
									Navigator.GoBack (); 
								} catch (Exception e) {
									ErrorMessage = e.Message;
								}
								IsLoading = false;
							}
					}));

			}
		}

		public Command FacebookSignInCommand {
			get {
				return facebookSignInCommand
					?? (facebookSignInCommand = new Command (
						() => {
							IsLoading = true;
						}));
			}
		}

		public Command SignUpCommand {
			get {
				return signUpCommand
				?? (signUpCommand = new Command (
						() => Navigator.NavigateTo (PageLocator.SignUpPageKey, new SignUpViewModel (Navigator))));
			}
		}

		public void Init()
		{
			if (!parseService.ShowLogin ())
				Authenticated ();
		}

		public LoginViewModel (NavigationService navService)
		{
			parseService = DependencyService.Get<IParseService>();
			Navigator = navService;
		
			App.PostSuccessFacebookAction = token => Authenticated ();
		}

		public void Authenticated ()
		{
			IsLoading = false;
			Navigator.NavigateTo (PageLocator.MainTabsPageKey, MainTabsViewModel.Instance);
		}
	}
}

