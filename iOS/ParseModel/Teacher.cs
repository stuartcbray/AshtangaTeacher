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

		public TeacherRole Role {
			get {
				return ParseObj.ContainsKey (Field_Role) ? 
					(TeacherRole)ParseObj.Get<long> (Field_Role) : TeacherRole.None;
			}
			set {
				ParseObj [Field_Role] = (long) value;
				OnPropertyChanged ();
			}
		}

		public async Task<bool> ShalaExistsAsync (string name)
		{
			var query = ParseUser.Query.WhereEqualTo("shalaNameLC", name.ToLower());
			IEnumerable<ParseObject> results = await query.FindAsync ();
			return results.Any ();
		}

		public override async Task InitializeAsync (object userObj)
		{
			await base.InitializeAsync (userObj);

			// Raise a PropertyChanged for the visible properties so they show up in the UI
			OnPropertyChanged ("ShalaName");
			OnPropertyChanged ("Email");
			OnPropertyChanged ("Name");
			OnPropertyChanged ("UserName");
			OnPropertyChanged ("Role");
		}
			
		public async Task UpdateRoleAsync(TeacherRole role)
		{
			ParseObj ["role"] = (long)role;
			await SaveAsync ();
				
			var parseRole = await ParseRole.Query.Where (x => x.Name == role.ToString()).FirstOrDefaultAsync ();
			parseRole.Users.Add ((ParseUser)ParseObj);
			await parseRole.SaveAsync ();
		}

		public async Task UpdatePropertyAsync<T> (string name, T value)
		{
			ParseObj [name] = value;
			await SaveAsync ();
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

		public Teacher() 
		{
			ParseObj = new ParseUser ();
		}
	}
}
