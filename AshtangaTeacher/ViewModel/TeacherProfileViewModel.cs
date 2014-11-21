using System;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight;
using Xamarin.Forms.Labs.Services.Media;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public class TeacherProfileViewModel : ViewModelBase
	{
		bool isLoading;
		string errorMessage;
		Teacher teacher;

		IParseService parseService;

		RelayCommand acceptTeacherRequestCommand;
		RelayCommand ignoreTeacherRequestCommand;

		public Teacher Model {
			get {
				return teacher;
			}
			set {
				Set (() => Model, ref teacher, value);
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

		public bool IsAdministrator {
			get {
				return App.Locator.Profile.Model.Role == TeacherRole.Administrator;
			}
		}


		public RelayCommand AcceptTeacherRequestCommand {
			get {
				return acceptTeacherRequestCommand
					?? (acceptTeacherRequestCommand = new RelayCommand (
						async () => {
							IsLoading = true;
							await parseService.AddUserToRole(teacher.ObjectId, "Moderator");
							IsLoading = false;

							App.Locator.ShalaTeachers.AcceptTeacher (Model);

							var navigationService = ServiceLocator.Current.GetInstance<INavigator> ();
							navigationService.GoBack ();
						}));
			}
		}


		public RelayCommand IgnoreTeacherRequestCommand {
			get {
				return ignoreTeacherRequestCommand
					?? (ignoreTeacherRequestCommand = new RelayCommand (
					 () => {
							IsLoading = true;
							//await parseService.IgnoreTeacher (teacher);
							IsLoading = false;

							var navigationService = ServiceLocator.Current.GetInstance<INavigator> ();
							navigationService.GoBack ();
						}));
			}
		}
			
		public TeacherProfileViewModel (Teacher teacher)
		{
			this.parseService = ServiceLocator.Current.GetInstance<IParseService> ();
			this.teacher = teacher;
		}
	}
}

