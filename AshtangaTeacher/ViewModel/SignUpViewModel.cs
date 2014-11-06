using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

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
		string shalaName;
		string errorMessage;

		RelayCommand signUpCommand;
		RelayCommand cancelCommand;
		RelayCommand saveShalaCommand;

		readonly IParseService parseService;
		readonly INavigator navigationService;
		readonly IEmailValidator regExUtils;

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

		public RelayCommand SaveShalaCommand {
			get {
				return saveShalaCommand
					?? (saveShalaCommand = new RelayCommand (
						async () => {
							if (string.IsNullOrEmpty(ShalaName)) {
								ErrorMessage = "Shala Name cannot be empty";
								return;
							}

							var exists = await parseService.ShalaNameExists(ShalaName);
							if (exists) {
								ErrorMessage = "Shala name " + ShalaName + " is taken.";
								return;
							}

							await parseService.UpdateUserPropertyAsync("shalaName", ShalaName);
							navigationService.GoBack ();
						}));
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

							if (!regExUtils.IsValidEmail (Email)) {
								ErrorMessage = "Invalid Email";
								return;
							}

							if (string.IsNullOrEmpty (ShalaName)) {
								ErrorMessage = "Shala Name is empty";
								return;
							}

							IsLoading = true;
							try {
								var exists = await parseService.ShalaNameExists(ShalaName);
								if (exists) {
									ErrorMessage = "Shala name " + ShalaName + " is taken.";
									return;
								}

								var teacher = new Teacher 
								{ 
									Name = name,
									UserName = userName,
									Email = email,
									ShalaName = shalaName,
									Password = passWord
								};

								await parseService.SignUpAsync (teacher);
								navigationService.PopToRoot ();
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

		public void Reset ()
		{
			ErrorMessage = UserName = Name = Password =  PasswordDupe = Email = ShalaName = "";
		}

		public SignUpViewModel (INavigator navigationService, IParseService parseService, IEmailValidator regExUtils)
		{
			this.parseService = parseService;
			this.navigationService = navigationService;
			this.regExUtils = regExUtils;
		}
	}
}

