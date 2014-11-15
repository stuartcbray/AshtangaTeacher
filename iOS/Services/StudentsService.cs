using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;
using Facebook;
using System.Collections.ObjectModel;
using System.Collections;
using Microsoft.Practices.ServiceLocation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Runtime.InteropServices;
using Xamarin.Forms.Platform.iOS;
using System.Net.Http;
using Xamarin.Forms;
using System.IO;

namespace AshtangaTeacher.iOS
{
	public class StudentsService : IStudentsService
	{
		public async Task<ObservableCollection<StudentViewModel>> GetAllAsync(string shalaName)
		{
			var query = ParseObject.GetQuery ("Student").Where (student => student.Get<string> ("shalaName") == shalaName);
			IEnumerable<ParseObject> results = await query.FindAsync();

			// Consider returning ObservableCollection instead
			var list = new ObservableCollection<StudentViewModel> ();
			foreach (var s in results) {

				var sid = s.Get<string> ("studentId");
				var student = new StudentViewModel (this,  new Student(sid) { 
					ShalaName = shalaName,
					Name = s.Get<string> ("name"),
					Email = s.Get<string> ("email"),
					ObjectId = s.ObjectId,
					ExpiryDate = new DateTime (s.Get<long> ("expiryDate"))
				});

				// Try the local cache first
				var cameraService = ServiceLocator.Current.GetInstance<ICameraService> ();
				var imgPath = cameraService.GetImagePath (sid);

				bool fetchImage = true;
				if (File.Exists (imgPath)) {
					var dt = File.GetLastWriteTimeUtc (imgPath);
					if (s.UpdatedAt != null && s.UpdatedAt <= dt) {
						student.Model.Image = ImageSource.FromFile (imgPath);
						fetchImage = false;
					} 
				}

				if (fetchImage) {
					// Load from Parse
					var parseImg = s.Get<ParseFile>("image");
					byte[] imgData = await new HttpClient ().GetByteArrayAsync (parseImg.Url);
					student.Model.Image = ImageSource.FromStream(() => new MemoryStream(imgData));

					// Now save local copy
					var data = NSData.FromArray (imgData);
					SaveThumbToDisk (data, imgPath);
				}

				student.Model.IsDirty = false;
				student.Model.ThumbIsDirty = false;
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
				studentObj ["shalaName"] = student.ShalaName;
				studentObj ["studentId"] = student.StudentId;
				studentObj ["expiryDate"] = student.ExpiryDate.Ticks;
				await studentObj.SaveAsync();

				if (student.ThumbIsDirty) {
					await SaveThumb (student, studentObj);
				}

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
			studentObj ["shalaName"] = student.ShalaName;
			studentObj ["studentId"] = student.StudentId;
			studentObj ["expiryDate"] = student.ExpiryDate.Ticks;
			await studentObj.SaveAsync();
			await SaveThumb (student, studentObj);
			
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

		void SaveThumbToDisk(NSData data, string fileName)
		{
			var docFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string pngFile = System.IO.Path.Combine (docFolder, fileName);

			NSError err;
			if (data.Save (pngFile, false, out err)) {
				Console.WriteLine ("Saved file " + pngFile);
			} else {
				Console.WriteLine ("NOT saved as " + pngFile + " because" + err.LocalizedDescription);
			}
		}

		async Task SaveThumb(Student student, ParseObject studentObj)
		{
			var renderer = new StreamImagesourceHandler ();
			var image = await renderer.LoadImageAsync (student.Image);
			using (NSData pngData = image.AsPNG()) {
			
				Byte[] data = new Byte[pngData.Length];
				Marshal.Copy(pngData.Bytes, data, 0, Convert.ToInt32(pngData.Length));

				ParseFile parseImg = new ParseFile(student.StudentId + ".PNG", data);
				await parseImg.SaveAsync ();

				studentObj["image"] = parseImg;
				await studentObj.SaveAsync ();

				SaveThumbToDisk (pngData, student.StudentId + ".PNG");
			}
			student.ThumbIsDirty = false;
		}
	}
}
