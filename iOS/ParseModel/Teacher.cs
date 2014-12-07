using System;
using Microsoft.Practices.ServiceLocation;
using AshtangaTeacher.iOS;
using Parse;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Net.Http;
using System.IO;

[assembly: Xamarin.Forms.Dependency (typeof (Teacher))]

namespace AshtangaTeacher.iOS
{
	public class Teacher : User, ITeacher
	{
		const string Field_Role = "role";
		const string Field_Name = "name";

		public string UserName {
			get {
				return ((ParseUser)ParseObj).Username ?? "";
			}
			set {
				if (UserName != value) {
					((ParseUser)ParseObj).Username = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public string Email {
			get {
				return ((ParseUser)ParseObj).Email ?? "";
			}
			set {
				if (((ParseUser)ParseObj).Email != value) {
					((ParseUser)ParseObj).Email = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public string Password {
			set {
				((ParseUser)ParseObj).Password = value;
				IsDirty = true;
				OnPropertyChanged ();
			}
		}

		public string RoleDisplay { 
			get {
				return Role.ToString ();
			}
		}

		TeacherRole role = TeacherRole.None;
		public TeacherRole Role {
			get {
				return role;
			}
			set {
				role = value;
				OnPropertyChanged ();
				OnPropertyChanged ("RoleDisplay");
			}
		}

		public async Task<bool> ShalaExistsAsync (string name)
		{
			var query = ParseUser.Query.WhereEqualTo("shalaNameLC", name.ToLower());
			IEnumerable<ParseObject> results = await query.FindAsync ();
			return results.Any ();
		}

		public override async Task SaveAsync ()
		{
			await base.SaveAsync ();

			// See if we've updated the Shala Name
			if (Role == TeacherRole.Administrator) {
				foreach (StudentViewModel vm in App.Students.Students) {
					if (vm.Model.ShalaName != ShalaName) {
						vm.Model.ShalaName = ShalaName;
						await vm.Model.SaveAsync ();
					}
				}
			}
		}

		public override async Task InitializeAsync (object userObj)
		{
			await base.InitializeAsync (userObj);

			var adminRole = await ParseRole.Query.Where (x => x.Name == TeacherRole.Administrator.ToString()).FirstOrDefaultAsync ();
			var modsRole = await ParseRole.Query.Where (x => x.Name == TeacherRole.Moderator.ToString()).FirstOrDefaultAsync ();

			var users = await adminRole.Users.Query.FindAsync ();
			if (users != null && users.Any (x => x.ObjectId == ObjectId)) {
				Role = TeacherRole.Administrator;
			} else {
				users = await modsRole.Users.Query.FindAsync ();
				if (users != null && users.Any (x => x.ObjectId == ObjectId))
					Role = TeacherRole.Moderator;
				else
					Role = TeacherRole.Pending;
			}

			// Raise a PropertyChanged for the visible properties so they show up in the UI
			OnPropertyChanged ("ShalaName");
			OnPropertyChanged ("Email");
			OnPropertyChanged ("Name");
			OnPropertyChanged ("UserName");
			OnPropertyChanged ("Role");
			OnPropertyChanged ("IsAdministrator");
		}
			
		public async Task UpdateRoleAsync(TeacherRole role)
		{				
			var parseRole = await ParseRole.Query.Where (x => x.Name == role.ToString()).FirstOrDefaultAsync ();
			parseRole.Users.Add ((ParseUser)ParseObj);
			await parseRole.SaveAsync ();

			Role = role;
		}

		public async Task<ObservableCollection<StudentViewModel>> GetStudentsAsync ()
		{
			var query = ParseObject.GetQuery ("Student").Where (student => student.Get<string> ("shalaNameLC") == ShalaName.ToLower ());
			IEnumerable<ParseObject> results = await query.FindAsync();

			var list = new ObservableCollection<StudentViewModel> ();
			foreach (var s in results) {

				var student = new Student ();
				await student.InitializeAsync (s);
				var vm = new StudentViewModel (student);
				list.Add (vm);
			}
			return list;
		}

		public async Task<ObservableCollection<ITeacher>> GetTeachersAsync ()
		{
			var query = ParseUser.Query.Where (teacher => teacher.Get<string> ("shalaNameLC") == ShalaName.ToLower ());
			IEnumerable<ParseUser> results = await query.FindAsync();

			var teachers = new ObservableCollection<ITeacher> ();
			foreach (var o in results) {
				var t = DependencyService.Get<ITeacher> (DependencyFetchTarget.NewInstance);
				await t.InitializeAsync (o);
				teachers.Add (t);
			}

			return teachers;
		}

		public Teacher() 
		{
			ParseObj = new ParseUser ();
		}
	}
}
