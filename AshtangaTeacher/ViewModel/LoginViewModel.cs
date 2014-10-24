using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace AshtangaTeacher
{
	public class LoginViewModel : ViewModelBase
	{
		bool isLoading;
		string userName;
		string passWord;
		string errorMessage;

		RelayCommand signUpCommand;
		RelayCommand logOutCommand;
		RelayCommand signInCommand;

		readonly IParseService parseService;
		readonly INavigationService navigationService;

		public IParseService ParseService { get { return parseService; } }

		public string ApplicationName { get { return "Ashtanga Teacher"; } }

		public void NotifyChanged ()
		{
			RaisePropertyChanged (() => SignInVisible);
			RaisePropertyChanged (() => SwitchUserVisible);
			RaisePropertyChanged (() => SignInButtonText);
			ErrorMessage = null;
			UserName = Password = "";
		}

		public bool IsLoading {
			get {
				return isLoading;
			}
			set {
				Set (() => IsLoading, ref isLoading, value);
			}
		}

		public bool SignInVisible {
			get {
				return parseService.ShowLogin ();
			}
		}

		public bool SwitchUserVisible {
			get {
				return !SignInVisible;
			}
		}

		public string SignInButtonText {
			get {
				return SignInVisible ? "Sign In" : "Continue as " + parseService.CurrentUser;
			}
		}

		public string UserName {
			get {
				return userName;
			}
			set {
				Set (() => UserName, ref userName, value);
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

		public RelayCommand LogOutCommand {
			get {
				return logOutCommand
				?? (logOutCommand = new RelayCommand (
					async () => {
						try {
							await parseService.LogOutAsync ();
							NotifyChanged ();
						} catch (Exception e) {
							ErrorMessage = e.Message;
						}

					}));
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
									if (parseService.CurrentUser == null) {
										await parseService.SignInAsync (UserName, Password);
									}	
									App.Locator.Main.GetStudentsCommand.Execute (null);
								} catch (Exception e) {
									ErrorMessage = e.Message;
								}
								IsLoading = false;
								navigationService.NavigateTo (ViewModelLocator.MainPageKey, App.Locator.Main);
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

		public LoginViewModel (INavigationService navigationService, IParseService parseService)
		{
			this.parseService = parseService;
			this.navigationService = navigationService;
		}
	}
}

