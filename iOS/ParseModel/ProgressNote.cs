using System;
using AshtangaTeacher.iOS;

[assembly: Xamarin.Forms.Dependency (typeof (ProgressNote))]

namespace AshtangaTeacher.iOS
{
	public class ProgressNote : IProgressNote
	{
		public string ObjectId {
			get;
			set;
		}

		public DateTime InputDate {
			get;
			set;
		}

		public string Text {
			get;
			set;
		}
	}
}

