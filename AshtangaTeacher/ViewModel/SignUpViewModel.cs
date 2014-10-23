using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace AshtangaTeacher
{
	public class SignUpViewModel : ViewModelBase
	{
		string userName;
		string email;
		string passWord;
		string passWordDupe;
		string shalaName;
		string errorMessage;

		RelayCommand signUpCommand;
		RelayCommand cancelCommand;

		readonly IParseService parseService;
		readonly INavigationService navigationService;
		readonly IEmailValidator regExUtils;

		public IParseService ParseService { get { return parseService; } }

		public string UserName {
			get {
				return userName;
			}
			set {
				Set (() => UserName, ref userName, value);
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

		public string PasswordDupe {
			get {
				return passWordDupe;
			}
			set {
				Set (() => PasswordDupe, ref passWordDupe, value);
			}
		}

		public string ShalaName {
			get {
				return shalaName;
			}
			set {
				Set (() => ShalaName, ref shalaName, value);
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

		public RelayCommand CancelCommand {
			get {
				return cancelCommand
				?? (cancelCommand = new RelayCommand (
					() => {
						UserName = Email = Password = PasswordDupe = ShalaName = "";
						navigationService.GoBack ();
					}));
			}
		}

		public RelayCommand SignUpCommand {
			get {
				return signUpCommand
				?? (signUpCommand = new RelayCommand (
					async () => {
						// Perform some simple validation...
						if (string.IsNullOrEmpty (UserName)) {
							ErrorMessage = "User Name is empty";
							return;
						}

						if (string.IsNullOrEmpty (Password) || string.IsNullOrEmpty (PasswordDupe)) {
							ErrorMessage = "Password is empty";
							return;
						}

						if (Password != PasswordDupe) {
							ErrorMessage = "Passwords don't match";
							Password = PasswordDupe = "";
							return;
						}

						if (!regExUtils.IsValidEmail (Email)) {
							ErrorMessage = "Invalid Email";
							return;
						}

						if (string.IsNullOrEmpty (ShalaName)) {
							ErrorMessage = "Shala Name is empty";
							return;
						}

						var teacher = new Teacher { 
							Name = UserName,
							Email = Email,
							ShalaName = ShalaName,
							Password = Password
						};

						try {
							await parseService.SignUpAsync (teacher);

							App.Locator.Login.NotifyChanged ();
							navigationService.GoBack ();
						} catch (Exception e) {
							ErrorMessage = e.Message;
						}

					}));

			}
		}

		public SignUpViewModel (INavigationService navigationService, IParseService parseService, IEmailValidator regExUtils)
		{
			this.parseService = parseService;
			this.navigationService = navigationService;
			this.regExUtils = regExUtils;
		}
	}
}

