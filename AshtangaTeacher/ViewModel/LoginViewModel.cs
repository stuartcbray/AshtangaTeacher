using System;
using Xamarin.Forms;
using Xamarin.Forms.Labs.Mvvm;

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
		Command facebookSignInCommand;

		readonly IParseService parseService;
		readonly INavigator navigationService;

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

		public Command FacebookSignInCommand {
			get {
				return facebookSignInCommand
					?? (facebookSignInCommand = new Command (
						() => {
							navigationService.NavigateTo (PageLocator.FacebookSignInKey, null);
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
									navigationService.GoBack (); 
								} catch (Exception e) {
									ErrorMessage = e.Message;
								}
								IsLoading = false;
							}
					}));

			}
		}

		public Command SignUpCommand {
			get {
				return signUpCommand
				?? (signUpCommand = new Command (
						() => navigationService.NavigateTo (PageLocator.SignUpPageKey, new SignUpViewModel ())));
			}
		}

		public LoginViewModel ()
		{
			parseService = DependencyService.Get<IParseService>();
			navigationService = NavigationService.Instance;
		}
	}
}

