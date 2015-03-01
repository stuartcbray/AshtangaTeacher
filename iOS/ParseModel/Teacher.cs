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
		const string FieldCredentials = "credentials";
		const string FieldBio = "bio";
		const string FieldIsAvailableForSub = "sub";
		const string FieldIsAvailableMessage = "isAvailableMessage";

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

		public string Credentials {
			get {
				return ParseObj.ContainsKey (FieldCredentials) ? ParseObj.Get<string> (FieldCredentials) : "";
			}
			set {
				if (Credentials != value) {
					ParseObj [FieldCredentials] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public string Bio {
			get {
				return ParseObj.ContainsKey (FieldBio) ? ParseObj.Get<string> (FieldBio) : "";
			}
			set {
				if (Bio != value) {
					ParseObj [FieldBio] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public string IsAvailableToggleMessage
		{
			get {
				return IsAvailableForSub ? "Available for Sub" : "Not Availble for Sub";
			}
		}

		public bool IsAvailableForSub {
			get {
				return ParseObj.ContainsKey (FieldIsAvailableForSub) && ParseObj.Get<bool> (FieldIsAvailableForSub);
			}
			set {
				if (IsAvailableForSub != value) {
					ParseObj [FieldIsAvailableForSub] = value;
					IsDirty = true;
					OnPropertyChanged ();
					OnPropertyChanged ("IsAvailableToggleMessage");
				}
			}
		}

		public string IsAvailableMessage {
			get {
				return ParseObj.ContainsKey (FieldIsAvailableMessage) ? ParseObj.Get<string> (FieldIsAvailableMessage) : "";
			}
			set {
				if (IsAvailableMessage != value) {
					ParseObj [FieldIsAvailableMessage] = value;
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
