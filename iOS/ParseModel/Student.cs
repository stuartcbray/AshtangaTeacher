using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Xamarin.Forms;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace AshtangaTeacher.iOS
{
	public class Student : ViewModelBasee, IStudent
	{
		bool isDirty, thumbIsDirty;

		string name;
		string email;
		string studentId;

		ImageSource image;
		DateTime expiryDate;

		public string StudentId { 
			get { 
				return studentId; 
			} 
			set {
				studentId = value;
			}
		}

		public ObservableCollection<IProgressNote> ProgressNotes { get; set; }

		public ObservableCollection<DateTime> AttendanceRecord { get; set; }

		public bool IsDirty {
			get {
				return isDirty;
			}
			set {
				isDirty = value;
				OnPropertyChanged ();
			}
		}

		public bool ThumbIsDirty {
			get {
				return thumbIsDirty;
			}
			set {
				thumbIsDirty = value;
				OnPropertyChanged ();
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
				OnPropertyChanged ("Image");
			}
		}

		public string Name {
			get {
				return name;
			}
			set {
				if (Set ("Name", ref name, value)) {
					IsDirty = true;
				}
			}
		}

		public string Email {
			get {
				return email;
			}
			set {
				if (Set ("Email", ref email, value)) {
					IsDirty = true;
				}
			}
		}

		public DateTime ExpiryDate {
			get {
				return expiryDate;
			}
			set {
				if (Set ("ExpiryDate", ref expiryDate, value)) {
					IsDirty = true;
				}
			}
		}
			
		public Student ()
		{
			ProgressNotes = new ObservableCollection<IProgressNote> ();
			var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();
			image = cameraService.GetImagePath (studentId);
		}

	}
}
