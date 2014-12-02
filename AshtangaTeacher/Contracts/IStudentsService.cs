using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AshtangaTeacher
{
	public interface IStudentsService
	{
		Task<ObservableCollection<StudentViewModel>> GetAllAsync(string shalaName);
	}
}