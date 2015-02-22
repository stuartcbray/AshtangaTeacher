using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace AshtangaTeacher
{
	// Inspired from MVVM Light INavigationService
	public class NavigationService
	{
		Dictionary<string, Type> pagesByKey;
		NavigationPage rootPage;

		public string CurrentPageKey
		{
			get
			{
				lock (pagesByKey)
				{
					if (rootPage.CurrentPage == null)
					{
						return null;
					}

					var pageType = rootPage.CurrentPage.GetType();

					return pagesByKey.ContainsValue(pageType)
						? pagesByKey.First(p => p.Value == pageType).Key
							: null;
				}
			}
		}

		public void GoBack()
		{
			rootPage.PopAsync();
		}

		public void PopToRoot ()
		{
			rootPage.PopToRootAsync ();
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
					rootPage.PushAsync(page);
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

		public void Initialize (NavigationPage root, Dictionary<string, Type> pages)
		{
			pagesByKey = pages;
			rootPage = root;
		}
	}
}