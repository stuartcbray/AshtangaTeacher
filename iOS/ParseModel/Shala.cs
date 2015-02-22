using System;
using Parse;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using System.IO;
using System.Net.Http;

namespace AshtangaTeacher.iOS
{
	public class Shala : ParseObjectBase, IShala
	{
		protected const string FieldName = "name";
		protected const string FieldNameLC = "nameLowerCase";
		protected const string FieldAddress = "address";
		protected const string FieldRates = "rates";
		protected const string FieldSchedule = "schedule";
		protected const string FieldContact = "contact";

		public string Rates {
			get {
				return ParseObj.ContainsKey (FieldRates) ? ParseObj.Get<string> (FieldRates) : "";
			}
			set {
				if (Rates != value) {
					ParseObj [FieldRates] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public string Schedule {
			get {
				return ParseObj.ContainsKey (FieldSchedule) ? ParseObj.Get<string> (FieldSchedule) : "";
			}
			set {
				if (Address != value) {
					ParseObj [FieldSchedule] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public string Contact {
			get {
				return ParseObj.ContainsKey (FieldContact) ? ParseObj.Get<string> (FieldContact) : "";
			}
			set {
				if (Contact != value) {
					ParseObj [FieldContact] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public string Address {
			get {
				return ParseObj.ContainsKey (FieldAddress) ? ParseObj.Get<string> (FieldAddress) : "";
			}
			set {
				if (Address != value) {
					ParseObj [FieldAddress] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}


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
			
		public async Task<ObservableCollection<ITeacher>> GetTeachersAsync ()
		{
			return null;
		}

		public async Task<ObservableCollection<StudentViewModel>> GetStudentsAsync ()
		{
			return null;
		}

		public Shala ()
		{
			ParseObj = new ParseObject("Shala");
		}
	}
}

