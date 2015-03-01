using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Collections.Generic;
using AshtangaTeacher.iOS;
using System.Threading.Tasks;
using Parse;

[assembly: Xamarin.Forms.Dependency (typeof (Student))]

namespace AshtangaTeacher.iOS
{
	public class Student : User, IStudent
	{
		const string FieldEmail = "email";
		const string FieldExpiryDate = "expiryDate";

		bool notesInitialized;

		public ObservableCollection<IProgressNote> ProgressNotes { get; set; }

		public ObservableCollection<DateTime> AttendanceRecord { get; set; }

		public override string Email {
			get {
				return ParseObj.ContainsKey (FieldEmail) ? ParseObj.Get<string> (FieldEmail) : "";
			}
			set {
				if (Email != value) {
					ParseObj [FieldEmail] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}

		public DateTime ExpiryDate {
			get {
				return ParseObj.ContainsKey (FieldExpiryDate) ? ParseObj.Get<DateTime> (FieldExpiryDate) : DateTime.Now;
			}
			set {
				if (ExpiryDate != value) {
					ParseObj [FieldExpiryDate] = value;
					IsDirty = true;
					OnPropertyChanged ();
				}
			}
		}
			
		public async Task<bool> AddProgressNoteAsync(IProgressNote note)
		{
			var noteObj = new ParseObject("ProgressNote")
			{
				{ "content", note.Text }
			};

			// Add a relation between the Student and ProgressNote
			noteObj["parent"] = ParseObj;
			noteObj.ACL = ParseObj.ACL;

			// This will save both noteObj and studentObj
			await noteObj.SaveAsync();

			ProgressNotes.Add (note);

			return true; 
		}

		public async Task GetProgressNotesAsync ()
		{
			if (!notesInitialized) {
				ProgressNotes.Clear ();

				var query = from note in ParseObject.GetQuery ("ProgressNote")
						where note ["parent"] == ParseObj
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
			ParseObj = new ParseObject ("Student");
			ExpiryDate = DateTime.Now;

			// Students are only visible to Approved Teachers (Moderators)
			var acl = new ParseACL();
			acl.SetRoleWriteAccess("Moderator", true);
			acl.SetRoleReadAccess("Moderator", true);
			ParseObj.ACL = acl;
		}

	}
}
