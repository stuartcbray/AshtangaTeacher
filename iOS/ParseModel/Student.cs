using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using AshtangaTeacher.iOS;
using System.Threading.Tasks;
using Parse;

[assembly: Xamarin.Forms.Dependency (typeof (Student))]

namespace AshtangaTeacher.iOS
{
	public class Student : User, IStudent
	{
		const string Field_Email = "email";
		const string Field_ExpiryDate = "expiryDate";

		bool notesInitialized;
		DateTime expiryDate;

		public ObservableCollection<IProgressNote> ProgressNotes { get; set; }

		public ObservableCollection<DateTime> AttendanceRecord { get; set; }

		public string Email {
			get {
				return ParseObj.ContainsKey (Field_Email) ? ParseObj.Get<string> (Field_Email) : "";
			}
			set {
				if (Email != value) {
					ParseObj [Field_Email] = value;
					IsDirty = true;
					OnPropertyChanged ();
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

		public async Task DeleteAsync ()
		{
			await ParseObj.DeleteAsync ();
		}
					
		public Student ()
		{
			ProgressNotes = new ObservableCollection<IProgressNote> ();
			ParseObj = new ParseObject ("Student");
			ShalaName = App.Profile.Model.ShalaName;
			ExpiryDate = DateTime.Now;
			UID = Guid.NewGuid ().ToString ();

			// Students are only visible to Approved Teachers (Moderators)
			var acl = new ParseACL();
			acl.SetRoleWriteAccess("Moderator", true);
			acl.SetRoleReadAccess("Moderator", true);
			ParseObj.ACL = acl;
		}

	}
}
