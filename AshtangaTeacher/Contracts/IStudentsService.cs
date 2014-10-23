using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AshtangaTeacher
{
	public interface IStudentsService
	{
		Task<IList<Student>> GetAllAsync(string shalaName);

		Task<bool> SaveAsync(Student student);

		Task<Student> AddAsync(Student student);

		Task<bool> DeleteAsync(Student student);

		Task<bool> AddProgressNoteAsync(Student student, ProgressNote note);

		Task<IList<ProgressNote>> GetStudentProgressNotesAsync(Student student);
	}
}