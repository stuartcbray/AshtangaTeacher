﻿using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight;

namespace AshtangaTeacher
{
	public class AddStudentViewModel : ViewModelBase
	{
		readonly IStudentsService studentService;
		readonly INavigationService navigationService;
		readonly Student student;

		RelayCommand addStudentCommand;
		RelayCommand cancelCommand;

		string errorMessage;

		public Student Model { get { return student; } }

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
				?? (cancelCommand = new RelayCommand (() => navigationService.GoBack ()));
			}
		}

		public RelayCommand AddStudentCommand {
			get {
				return addStudentCommand
				?? (addStudentCommand = new RelayCommand (
						async () => {

							// Perform some simple validation...
							if (string.IsNullOrEmpty (Model.Name)) {
								ErrorMessage = "User Name is empty";
								return;
							}

							if (string.IsNullOrEmpty (Model.Email)) {
								ErrorMessage = "Email is empty";
								return;
							}
								
							await studentService.AddAsync (student);
							App.Locator.Main.Students.Add (new StudentViewModel (studentService, student));
							navigationService.GoBack ();
						}));
			}
		}

		public AddStudentViewModel (IStudentsService service, INavigationService nav, Student student)
		{
			studentService = service;
			navigationService = nav;
			this.student = student;
		}
	}
}

