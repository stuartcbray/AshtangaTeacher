using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;
using Facebook;
using System.Collections.ObjectModel;
using System.Collections;

namespace AshtangaTeacher.iOS
{
	public class StudentsService : IStudentsService
	{
		public async Task<ObservableCollection<StudentViewModel>> GetAllAsync(string shalaName)
		{
			var query = ParseObject.GetQuery ("Student").Where (student => student.Get<string> ("shala") == shalaName);
			IEnumerable<ParseObject> results = await query.FindAsync();

			// Consider returning ObservableCollection instead
			var list = new ObservableCollection<StudentViewModel> ();
			foreach (var s in results) {
				var student = new StudentViewModel (this,  new Student { 
					ShalaName = shalaName,
					Name = s.Get<string> ("name"),
					Email = s.Get<string> ("email"),
					ObjectId = s.ObjectId,
					ExpiryDate = new DateTime (s.Get<long> ("expirydate"))
				});

				student.Model.IsDirty = false;
				list.Add (student);
			}
			return list;
		}
			
		public async Task<IList<ProgressNote>> GetStudentProgressNotesAsync(Student student)
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery("Student");
			ParseObject studentObj = await query.GetAsync(student.ObjectId);

			query = from note in ParseObject.GetQuery("ProgressNote")
					where note["parent"] == studentObj
				select note;

			var notes = await query.FindAsync();

			var list = new List<ProgressNote> ();
			foreach (var n in notes)
				list.Add (new ProgressNote { 
					ObjectId = n.ObjectId,
					InputDate = n.CreatedAt ?? DateTime.Now,
					Text = n.Get<string>("content")
				});

			return list;
		}

		public async Task<bool> SaveAsync(Student student)
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery("Student");
			ParseObject studentObj = await query.GetAsync(student.ObjectId);

			if (studentObj != null) {
				studentObj ["name"] = student.Name;
				studentObj ["email"] = student.Email;
				studentObj ["shala"] = student.ShalaName;
				studentObj ["expirydate"] = student.ExpiryDate.Ticks;
				await studentObj.SaveAsync();
				student.IsDirty = false;
				return true;
			}

			return false;
		}

		public async Task<bool> AddProgressNoteAsync(Student student, ProgressNote note)
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery("Student");
			ParseObject studentObj = await query.GetAsync(student.ObjectId);

			var noteObj = new ParseObject("ProgressNote")
			{
				{ "content", note.Text }
			};

			// Add a relation between the Student and ProgressNote
			noteObj["parent"] = studentObj;

			// This will save both noteObj and studentObj
			await noteObj.SaveAsync();

			return true; 
		}

		public async Task<Student> AddAsync(Student student)
		{
			var studentObj = new ParseObject("Student");
			studentObj ["name"] = student.Name;
			studentObj ["email"] = student.Email;
			studentObj ["shala"] = student.ShalaName;
			studentObj ["expirydate"] = student.ExpiryDate.Ticks;

			await studentObj.SaveAsync();

			// After the SaveAsync we have an ObjectId
			student.ObjectId = studentObj.ObjectId;
			student.IsDirty = false;
			return student;
		}

		public async Task<bool> DeleteAsync(Student student)
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery("Student");
			ParseObject studentObj = await query.GetAsync(student.ObjectId);

			if (studentObj != null) {
				await studentObj.DeleteAsync ();
				return true;
			}

			return false;
		}
	}
}
