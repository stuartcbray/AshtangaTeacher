using System;
using GalaSoft.MvvmLight;

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

