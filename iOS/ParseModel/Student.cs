using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Xamarin.Forms;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using AshtangaTeacher.iOS;
using System.Threading.Tasks;
using Parse;

[assembly: Xamarin.Forms.Dependency (typeof (Student))]

namespace AshtangaTeacher.iOS
{
	public class Student : ViewModelBasee, IStudent
	{
		bool isDirty, thumbIsDirty, notesInitialized;

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
				var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();
				image = cameraService.GetImagePath (studentId);
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


		public async Task<bool> AddProgressNoteAsync(IProgressNote note)
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery("Student");
			ParseObject studentObj = await query.GetAsync(ObjectId);

			var noteObj = new ParseObject("ProgressNote")
			{
				{ "content", note.Text }
			};

			// Add a relation between the Student and ProgressNote
			noteObj["parent"] = studentObj;
			noteObj.ACL = studentObj.ACL;

			// This will save both noteObj and studentObj
			await noteObj.SaveAsync();

			ProgressNotes.Add (note);

			return true; 
		}

		public async Task GetProgressNotesAsync ()
		{
			if (!notesInitialized) {
				ProgressNotes.Clear ();

				ParseQuery<ParseObject> query = ParseObject.GetQuery ("Student");
				ParseObject studentObj = await query.GetAsync (ObjectId);

				query = from note in ParseObject.GetQuery ("ProgressNote")
				       where note ["parent"] == studentObj
				       select note;

				var notes = await query.FindAsync ();

				foreach (var n in notes) {
					var note = DependencyService.Get<IProgressNote> (DependencyFetchTarget.NewInstance);
					note.ObjectId = n.ObjectId;
					note.InputDate = n.CreatedAt ?? DateTime.Now;
					note.Text = n.Get<string> ("content");
					ProgressNotes.Add (note);
				}
				notesInitialized = true;
			}
		}
			
		public Student ()
		{
			ProgressNotes = new ObservableCollection<IProgressNote> ();
		}

	}
}
