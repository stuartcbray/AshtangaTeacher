using System;

namespace AshtangaTeacher
{
	public class ProfileViewModel
	{
		readonly IParseService parseService;
		readonly INavigator navigationService;

		public ProfileViewModel (
			INavigator navigationService,
			IParseService parseService
		)
		{
			this.parseService = parseService;
			this.navigationService = navigationService;
		}
	}
}

