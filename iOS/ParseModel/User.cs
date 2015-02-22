using System;
using Xamarin.Forms;

namespace AshtangaTeacher.iOS
{
	public abstract class User : ParseObjectBase, IUser
	{
		protected const string FieldName = "name";
		protected const string FieldSex = "sex";
		protected const string FieldDoB = "dob";

		public abstract string Email { get; set; }

		public string Name {
			get {
				return ParseObj.ContainsKey (FieldName) ? ParseObj.Get<string> (FieldName) : "";
			}
			set {
				if (Name != value) {
					ParseObj [FieldName] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public string Sex {
			get {
				return ParseObj.ContainsKey (FieldSex) ? ParseObj.Get<string> (FieldSex) : "";
			}
			set {
				if (Sex != value) {
					ParseObj [FieldSex] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public DateTime DOB {
			get {
				return ParseObj.ContainsKey (FieldDoB) ? ParseObj.Get<DateTime> (FieldDoB) : DateTime.Now;
			}
			set {
				if (DOB != value) {
					ParseObj [FieldDoB] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}
	}
}

