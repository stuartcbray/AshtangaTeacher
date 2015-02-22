using System;
using Xamarin.Forms;

namespace AshtangaTeacher
{
	public class SignUpViewModel : ViewModelBase
	{
		bool isLoading;

		string userName;
		string name;
		string email;
		string passWord;
		string passWordDupe;
		string errorMessage;

		Command signUpCommand;
		Command cancelCommand;
		Command resetPasswordCommand;

		readonly IParseService parseService;
		readonly IDeviceService deviceService;

		public IParseService ParseService { get { return parseService; } }

		public string Name {
			get {
				return name;
			}
			set {
				Set (() => Name, ref name, value);
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
			
		public string Email {
			get {
				return email;
			}
			set {
				Set (() => Email, ref email, value);
				UserName = value;
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

		public string ErrorMessage {
			get {
				return errorMessage;
			}
			set {
				Set (() => ErrorMessage, ref errorMessage, value);
			}
		}

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

		public Command CancelCommand {
			get {
				return cancelCommand
					?? (cancelCommand = new Command (Navigator.GoBack));
			}
		}

		public Command ResetPasswordCommand {
			get {
				return resetPasswordCommand
					?? (resetPasswordCommand = new Command (
						async () => {

							if (!deviceService.IsValidEmail (Email)) {
								ErrorMessage = "Invalid Email";
								return;
							}

							IsLoading = true;
							await parseService.ResetPasswordAsync (Email);
							IsLoading = false;

							await DialogService.Instance.ShowMessage ("You have been sent a password reset email.", "Password Reset");
							Navigator.GoBack ();
						}));
			}
		}

		public Command SignUpCommand {
			get {
				return signUpCommand
				?? (signUpCommand = new Command (
					async () => {
							// Perform some simple validation...
							if (string.IsNullOrEmpty (Name)) {
								ErrorMessage = "Name is empty";
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

							if (!deviceService.IsValidEmail (Email)) {
								ErrorMessage = "Invalid Email";
								return;
							}

							IsLoading = true;
							try {
								await parseService.SignUpAsync (name, userName, email, passWord);
								Navigator.NavigateTo (PageLocator.MainTabsPageKey, MainTabsViewModel.Instance);
							}
							catch (Exception e) {
								ErrorMessage = e.Message;
							}
							finally {
								IsLoading = false;
							}
					}));

			}
		}

		public SignUpViewModel (NavigationService nav)
		{
			parseService = DependencyService.Get<IParseService>();
			Navigator = nav;
			deviceService = DependencyService.Get<IDeviceService>();
		}
	}
}

