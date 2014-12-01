using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AshtangaTeacher
{
	public interface IStudentsService
	{
		Task<ObservableCollection<StudentViewModel>> GetAllAsync(string shalaName);

		Task<bool> SaveAsync(IStudent student);

		Task<IStudent> AddAsync(IStudent student);

		Task<bool> DeleteAsync(IStudent student);

		Task<bool> AddProgressNoteAsync(IStudent student, IProgressNote note);

		Task<IList<IProgressNote>> GetStudentProgressNotesAsync(IStudent student);
	}
}