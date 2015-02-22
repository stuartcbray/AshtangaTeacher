using Xamarin.Forms;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public class MainTabsViewModel : ViewModelBase
	{
		bool isLoading;

		readonly IParseService parseService;

		public IParseService ParseService { get { return parseService; } }

		public ShalasViewModel ShalasVm { get; private set; }

		public ProfileViewModel ProfileVm { get; private set; }

		public bool IsLoading {
			get {
				return isLoading;
			}
			set {
				if (Set (() => IsLoading, ref isLoading, value)) {
					OnPropertyChanged ("IsReady");
				}
			}
		}

		public bool IsReady {
			get {
				return !isLoading;
			}
		}

		public void InitializeTabViewModels ()
		{
			ShalasVm = new ShalasViewModel ();
			ProfileVm = new ProfileViewModel ();
		}

		static MainTabsViewModel instance;
		static object instanceLock = new object();

		public static MainTabsViewModel Instance {
			get {
				if (instance == null) {
					lock (instanceLock) {
						if (instance == null)
							instance = new MainTabsViewModel ();
					}
				}
				return instance;
			}
		}

		MainTabsViewModel ()
		{
			parseService = DependencyService.Get<IParseService>();
			isLoading = true;
		}

	}
}

