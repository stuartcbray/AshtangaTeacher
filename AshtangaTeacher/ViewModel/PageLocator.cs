using System.Collections.Generic;
using System;

namespace AshtangaTeacher
{
	public static class PageLocator
	{
		public const string AddProgressNotePageKey = "AddProgressNotePage";
		public const string ProgressNotesPageKey = "ProgressNotesPage";
		public const string AddStudentPageKey = "AddStudentPage";
		public const string StudentDetailsPageKey = "StudentDetailsPage";
		public const string StudentsPageKey = "StudentsPage";
		public const string MainPageKey = "MainPage";
		public const string LoginPageKey = "LoginPage";
		public const string SignUpPageKey = "SignUpPage";
		public const string MainTabsPageKey = "MainTabsPage";
		public const string ProfilePageKey = "ProfilePage";
		public const string ShalaTeachersPageKey = "ShalaTeachersPage";
		public const string TeacherProfilePageKey = "TeacherProfilePage";
		public const string ResetPasswordPageKey = "ResetPasswordPage";
		public const string AddShalaPageKey = "AddShalaPage";
		public const string ShalaPageKey = "ShalaPage";

		static Dictionary<string, Type> pagesByKey = new Dictionary<string, Type> () {
			{ AddProgressNotePageKey, typeof(AddProgressNotePage) },
			{ ProgressNotesPageKey, typeof(ProgressNotesPage) },
			{ AddStudentPageKey, typeof(AddStudentPage) },
			{ StudentDetailsPageKey, typeof(StudentDetailsPage) },
			{ MainPageKey, typeof(StudentsPage) },
			{ LoginPageKey, typeof(LoginPage) },
			{ SignUpPageKey, typeof(SignUpPage) },
			{ MainTabsPageKey, typeof(MainTabsPage) },
			{ ProfilePageKey, typeof(ProfilePage) },
			{ ShalaTeachersPageKey, typeof(ShalaTeachersPage) },
			{ TeacherProfilePageKey, typeof(TeacherProfilePage) },
			{ ResetPasswordPageKey, typeof(ResetPasswordPage) },
			{ AddShalaPageKey, typeof(AddShalaPage) },
			{ ShalaPageKey, typeof(ShalaPage) },
			{ StudentsPageKey, typeof(StudentsPage) }
		};

		public static Dictionary<string, Type> PagesByKey {
			get {
				return pagesByKey;
			}
		}
	}
}