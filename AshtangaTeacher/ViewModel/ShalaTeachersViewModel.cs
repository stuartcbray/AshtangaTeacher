using System;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;

namespace AshtangaTeacher
{
	public class ShalaTeachersViewModel : ViewModelBase
	{
		readonly ObservableCollection<Teacher> shalaTeachers = new ObservableCollection<Teacher> ();
		public ObservableCollection<Teacher> ShalaTeachers {
			get {
				return shalaTeachers;
			}
		}

		readonly ObservableCollection<Teacher> pendingTeachers = new ObservableCollection<Teacher> ();
		public ObservableCollection<Teacher> PendingTeachers {
			get {
				return pendingTeachers;
			}
		}

		public ShalaTeachersViewModel (List<Teacher> teachers)
		{
			foreach (var t in teachers) {
				if (t.Role != TeacherRole.None)
					ShalaTeachers.Add (t);
				else
					PendingTeachers.Add (t);
			}
		}
	}
}

