 using System;
using AshtangaTeacher.iOS;
using Parse;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency (typeof (Teacher))]

namespace AshtangaTeacher.iOS
{
	public class Teacher : User, ITeacher
	{
		const string FieldRole = "role";

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

		public override string Email {
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
				OnPropertyChanged("RoleDisplay");
			}
		}
			
		public async Task<ObservableCollection<ShalaViewModel>> GetShalasAsync ()
		{
			var shalasRelation = ParseObj.GetRelation<ParseObject>("shalas");
			IEnumerable<ParseObject> shalas = await shalasRelation.Query.FindAsync();
			var shalasCollection = new ObservableCollection<ShalaViewModel>();
			foreach (var parseObj in shalas) {
				var shala = new Shala();
				await shala.InitializeAsync (parseObj);
				shalasCollection.Add(new ShalaViewModel(shala, null));
			}
			return shalasCollection;
		}

		public Teacher() 
		{
			ParseObj = new ParseUser ();
		}
	}
}
