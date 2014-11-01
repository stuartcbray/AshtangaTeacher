using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Xamarin.Forms;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace AshtangaTeacher
{
	public class Student : ObservableObject
	{
		bool isDirty;

		string name;
		string email;
		string imageUrl;
		DateTime expiryDate;
		ICameraService cameraService;

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

		public string ImageUrl {
			get {
				return imageUrl;
			}
			set {
				imageUrl = value;
				RaisePropertyChanged ("ImageUrl");
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

		void Init()
		{
			cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();
			ProgressNotes = new ObservableCollection<ProgressNote> ();
			imageUrl = cameraService.GetImagePath (studentId);
		}

		public Student ()
		{
			studentId = Guid.NewGuid ().ToString ();
			Init ();
		}

		public Student (string studentId)
		{
			this.studentId = studentId;
			Init ();
		}
	}
}
