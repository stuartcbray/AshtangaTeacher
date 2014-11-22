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
		bool isDirty, thumbIsDirty;

		string name;
		string email;
		ImageSource image;
		DateTime expiryDate;
		ICameraService cameraService;

		readonly string studentId;

		public string StudentId { get { return studentId; } }

		public ObservableCollection<IProgressNote> ProgressNotes { get; set; }

		public ObservableCollection<DateTime> AttendanceRecord { get; set; }

		public bool IsDirty {
			get {
				return isDirty;
			}
			set {
				Set (() => IsDirty, ref isDirty, value);
			}
		}

		public bool ThumbIsDirty {
			get {
				return thumbIsDirty;
			}
			set {
				Set (() => ThumbIsDirty, ref thumbIsDirty, value);
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
				return image;
			}
			set {
				image = value;
				ThumbIsDirty = true;
				IsDirty = true;
				RaisePropertyChanged ("Image");
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
			ProgressNotes = new ObservableCollection<IProgressNote> ();
			image = cameraService.GetImagePath (studentId);
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
