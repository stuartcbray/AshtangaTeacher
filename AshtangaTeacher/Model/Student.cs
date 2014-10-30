using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Xamarin.Forms;

namespace AshtangaTeacher
{
	public class Student : ObservableObject
	{
		bool isDirty;

		string name;
		string email;
		DateTime expiryDate;
		ImageSource imageSource;

		readonly string studentId;
		public string StudentId { get { return studentId; } }

		public ObservableCollection<ProgressNote> ProgressNotes { get; set; }

		public ObservableCollection<DateTime> AttendanceRecord { get; set; }

		public bool IsDirty {
			get {
				return isDirty;
			}
			set {
				Set (() => IsDirty, ref isDirty, value);
			}
		}

		public string ShalaName { 
			get; 
			set; 
		}

		public string ObjectId {
			get;
			set;
		}

		public ImageSource Image {
			get {
				return imageSource;
			}
			set {
				Set (() => Image, ref imageSource, value);
			}
		}

		public string Name {
			get {
				return name;
			}
			set {
				if (Set (() => Name, ref name, value)) {
					IsDirty = true;
				}
			}
		}

		public string Email {
			get {
				return email;
			}
			set {
				if (Set (() => Email, ref email, value)) {
					IsDirty = true;
				}
			}
		}

		public DateTime ExpiryDate {
			get {
				return expiryDate;
			}
			set {
				if (Set (() => ExpiryDate, ref expiryDate, value)) {
					IsDirty = true;
				}
			}
		}

		public Student ()
		{
			ProgressNotes = new ObservableCollection<ProgressNote> ();
			studentId = Guid.NewGuid ().ToString ();
		}
	}
}
