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
		public const string MainPageKey = "MainPage";
		public const string LoginPageKey = "LoginPage";
		public const string SignUpPageKey = "SignUpPage";
		public const string FacebookSignInKey = "FacebookLoginPage";
		public const string TeacherInfoPageKey = "TeacherInfoPage";
		public const string MainTabsPageKey = "MainTabsPage";
		public const string ProfilePageKey = "ProfilePage";
		public const string ShalaTeachersPageKey = "ShalaTeachersPage";
		public const string TeacherProfilePageKey = "TeacherProfilePage";

		static Dictionary<string, Type> pagesByKey = new Dictionary<string, Type> () {
			{ AddProgressNotePageKey, typeof(AddProgressNotePage) },
			{ ProgressNotesPageKey, typeof(ProgressNotesPage) },
			{ AddStudentPageKey, typeof(AddStudentPage) },
			{ StudentDetailsPageKey, typeof(StudentDetailsPage) },
			{ MainPageKey, typeof(MainPage) },
			{ LoginPageKey, typeof(LoginPage) },
			{ SignUpPageKey, typeof(SignUpPage) },
			{ FacebookSignInKey, typeof(FacebookLoginPage) },
			{ TeacherInfoPageKey, typeof(TeacherInfoPage) },
			{ MainTabsPageKey, typeof(MainTabsPage) },
			{ ProfilePageKey, typeof(ProfilePage) },
			{ ShalaTeachersPageKey, typeof(ShalaTeachersPage) },
			{ TeacherProfilePageKey, typeof(TeacherProfilePage) }
		};

		public static Dictionary<string, Type> PagesByKey {
			get {
				return pagesByKey;
			}
		}
	}
}