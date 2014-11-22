using System;

namespace AshtangaTeacher
{
	public interface IProgressNote
	{
		string ObjectId { get; set; }

		DateTime InputDate { get; set; }

		string Text { get; set; }
	}
}

