using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace AshtangaTeacher
{
	public abstract class ViewModelBasee : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = delegate {};

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
	}


}

