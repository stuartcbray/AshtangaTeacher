using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace AshtangaTeacher
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		protected readonly IDevice DeviceService;

		public virtual event PropertyChangedEventHandler PropertyChanged;

		public NavigationService Navigator { get; protected set; }

		public Double ScreenWidth {
			get {
				return DeviceService.Display.WidthRequestInInches (DeviceService.ScreenWidthInches ());
			}
		}

		public Double ScreenWidthOneThird {
			get {
				return DeviceService.Display.WidthRequestInInches (DeviceService.ScreenWidthInches ()) / 3;
			}
		}

		public Double ScreenWidthOneHalf {
			get {
				return DeviceService.Display.WidthRequestInInches (DeviceService.ScreenWidthInches ()) / 2;
			}
		}

		public Double ScreenHeight {
			get {
				return DeviceService.Display.HeightRequestInInches (DeviceService.ScreenHeightInches ());
			}
		}

		string errorMessage;
		public string ErrorMessage {
			get {
				return errorMessage;
			}
			set {
				Set (() => ErrorMessage, ref errorMessage, value);
			}
		}

		public event Action IsLoadingChanged = delegate {};

		bool isLoading;
		public bool IsLoading {
			get {
				return isLoading;
			}
			set {
				if (Set (() => IsLoading, ref isLoading, value)) {
					OnPropertyChanged ("IsReady");
					IsLoadingChanged ();
				}
			}
		}

		public bool IsReady {
			get {
				return !isLoading;
			}
		}

		public ViewModelBase() 
		{
			DeviceService = Resolver.Resolve<IDevice>();
		}

		protected void OnPropertyChanged([CallerMemberName] string property = null)
		{
			var handler = PropertyChanged;
			if (handler != null) {
				handler (this, new PropertyChangedEventArgs (property));
			}
		}

		protected bool Set<T> (string propertyName, ref T field, T newValue)
		{
			if (EqualityComparer<T>.Default.Equals (field, newValue)) {
				return false;
			}
			field = newValue;
			OnPropertyChanged(propertyName);
			return true;
		}

		protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
		{
			if (EqualityComparer<T>.Default.Equals (field, newValue)) {
				return false;
			}
			field = newValue;
			OnPropertyChanged(GetPropertyName(propertyExpression));
			return true;
		}

		protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
		{
			if (propertyExpression == null) {
				throw new ArgumentNullException("propertyExpression");
			}

			var body = propertyExpression.Body as MemberExpression;

			if (body == null) {
				throw new ArgumentException("Invalid argument", "propertyExpression");
			}

			var property = body.Member as PropertyInfo;

			if (property == null) {
				throw new ArgumentException("Argument is not a property", "propertyExpression");
			}

			return property.Name;
		}
	}


}

