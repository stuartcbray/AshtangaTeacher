using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AshtangaTeacher
{
	public enum TeacherRole {
		None,
		Administrator,
		Moderator,
		Pending
	};

	public interface ITeacher : IUser
	{
		TeacherRole Role { get; set; }

		string RoleDisplay { get; }

		string UserName { get; set; }

		bool IsAvailableForSub { get; set; }

		Task<ObservableCollection<ShalaViewModel>> GetShalasAsync ();
	}
}

