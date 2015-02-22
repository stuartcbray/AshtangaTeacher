using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AshtangaTeacher
{
	public interface IShala : IParseObject
	{
		string Name { get; set; }

		string Address { get; set; }

		string Rates { get; set; }

		string Schedule { get; set; }

		string Contact { get; set; }

		Task<ObservableCollection<ITeacher>> GetTeachersAsync ();

		Task<ObservableCollection<StudentViewModel>> GetStudentsAsync ();
	}
}

