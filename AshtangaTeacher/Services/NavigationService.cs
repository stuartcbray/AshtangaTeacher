using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace AshtangaTeacher
{
	// Inspired from MVVM Light INavigationService
	public class NavigationService : INavigator
	{
		Dictionary<string, Type> pagesByKey;
		NavigationPage navigation;

		static NavigationService instance;
		static object instanceLock = new object();

		public static NavigationService Instance {
			get {
				if (instance == null) {
					lock (instanceLock) {
						if (instance == null)
							instance = new NavigationService ();
					}
				}
				return instance;
			}
		}

		NavigationService ()
		{
		}

		public string CurrentPageKey
		{
			get
			{
				lock (pagesByKey)
				{
					if (navigation.CurrentPage == null)
					{
						return null;
					}

					var pageType = navigation.CurrentPage.GetType();

					return pagesByKey.ContainsValue(pageType)
						? pagesByKey.First(p => p.Value == pageType).Key
							: null;
				}
			}
		}

		public void GoBack()
		{
			navigation.PopAsync();
		}

		public void PopToRoot ()
		{
			navigation.PopToRootAsync ();
		}

		public void NavigateTo(string pageKey)
		{
			NavigateTo(pageKey, null);
		}

		public void NavigateTo(string pageKey, object parameter)
		{
			lock (pagesByKey)
			{
				if (pagesByKey.ContainsKey(pageKey))
				{
					var type = pagesByKey[pageKey];
					ConstructorInfo constructor = null;
					object[] parameters = null;

					if (parameter == null)
					{
						constructor = type.GetTypeInfo()
							.DeclaredConstructors
							.FirstOrDefault(c => !c.GetParameters().Any());

						parameters = new object[]
						{
						};
					}
					else
					{
						constructor = type.GetTypeInfo()
							.DeclaredConstructors
							.FirstOrDefault(
								c =>
								{
									var p = c.GetParameters();
									return p.Count() == 1
										&& p[0].ParameterType == parameter.GetType();
								});

						parameters = new[]
						{
							parameter
						};
					}

					if (constructor == null)
					{
						throw new InvalidOperationException(
							"No suitable constructor found for page " + pageKey);
					}

					var page = constructor.Invoke(parameters) as Page;
					navigation.PushAsync(page);
				}
				else
				{
					throw new ArgumentException(
						string.Format(
							"No such page: {0}. Did you forget to call NavigationService.Configure?",
							pageKey),
						"pageKey");
				}
			}
		}

		public void Initialize (Dictionary<string, Type> pages)
		{
			pagesByKey = pages;
		}

		public void SetRootNavigation(NavigationPage navigation)
		{
			this.navigation = navigation;
		}
	}
}