using Xamarin.Forms;
using System.Threading.Tasks;

namespace AshtangaTeacher
{
	public class MainTabsViewModel : ViewModelBase
	{
		readonly IParseService parseService;

		public IParseService ParseService { get { return parseService; } }

		public ShalasViewModel ShalasVm { get; private set; }

		public ProfileViewModel ProfileVm { get; private set; }

		public void InitializeTabViewModels ()
		{
			ShalasVm = new ShalasViewModel ();
			ProfileVm = new ProfileViewModel ();
		}

		static MainTabsViewModel instance;
		static readonly object instanceLock = new object();

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
			IsLoading = true;
		}

	}
}

