using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

namespace AshtangaTeacher
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public virtual event PropertyChangedEventHandler PropertyChanged;

		public NavigationService Navigator { get; protected set; }

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

