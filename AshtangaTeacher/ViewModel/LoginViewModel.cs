using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace AshtangaTeacher
{
	public class LoginViewModel : ViewModelBase
	{
		bool isLoading;
		string email;
		string passWord;
		string errorMessage;

		RelayCommand signUpCommand;
		RelayCommand facebookLoginSuccessCommand;
		RelayCommand signInCommand;
		RelayCommand facebookSignInCommand;

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
					RaisePropertyChanged ("IsReady");
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

		public RelayCommand FacebookSignInCommand {
			get {
				return facebookSignInCommand
					?? (facebookSignInCommand = new RelayCommand (
						() => {
							navigationService.NavigateTo (ViewModelLocator.FacebookSignInKey, null);
						}
					));

			}
		}

		public RelayCommand FacebookLoginSuccessCommand {
			get {
				return facebookLoginSuccessCommand
					?? (facebookLoginSuccessCommand = new RelayCommand (
						 () => {
								navigationService.PopToRoot ();
							}
						));

			}
		}

		public RelayCommand SignInCommand {
			get {
				return signInCommand
				?? (signInCommand = new RelayCommand (
					async () => {
							if (!IsLoading) {
								IsLoading = true;
								try {
									var teacher = await parseService.GetTeacherAsync ();
									if (teacher == null) {
										await parseService.SignInAsync (Email, Password);
									}	
									navigationService.GoBack (); 
								} catch (Exception e) {
									ErrorMessage = e.Message;
								}
								IsLoading = false;
							}
					}));

			}
		}

		public RelayCommand SignUpCommand {
			get {
				return signUpCommand
				?? (signUpCommand = new RelayCommand (
						() => navigationService.NavigateTo (ViewModelLocator.SignUpPageKey, App.Locator.SignUp)));
			}
		}
			
		public void ClearFields ()
		{
			ErrorMessage = "";
			Email = "";
			Password = "";
		}

		public LoginViewModel (INavigator navigationService, IParseService parseService)
		{
			this.parseService = parseService;
			this.navigationService = navigationService;
		}
	}
}

