using System;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public class ShalaTeachersViewModel : ViewModelBase
	{
		bool isLoading;

		readonly IParseService parseService;
		readonly INavigator navigationService;

		public bool IsLoading {
			get {
				return isLoading;
			}
			set {
				if (Set (() => IsLoading, ref isLoading, value)) {
					RaisePropertyChanged ("IsReady");
				}
			}
		}

		public bool IsReady {
			get {
				return !isLoading;
			}
		}

		ObservableCollection<Teacher> shalaTeachers = new ObservableCollection<Teacher> ();
		public ObservableCollection<Teacher>  ShalaTeachers {
			get {
				return shalaTeachers;
			}
			set {
				Set (() => ShalaTeachers, ref shalaTeachers, value);
			}
		}

		ObservableCollection<Teacher> pendingTeachers = new ObservableCollection<Teacher> ();
		public ObservableCollection<Teacher>  PendingTeachers {
			get {
				return pendingTeachers;
			}
			set {
				Set (() => PendingTeachers, ref pendingTeachers, value);
			}
		}

		public async Task Init()
		{
			IsLoading = true;
			ShalaTeachers = await parseService.GetTeachers ();
			PendingTeachers = await parseService.GetPendingTeachers ();
			IsLoading = false;
		}

		public ShalaTeachersViewModel (
			INavigator navigationService,
			IParseService parseService
		)
		{
			this.parseService = parseService;
			this.navigationService = navigationService;
		}
	}
}

