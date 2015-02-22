using System;

namespace AshtangaTeacher
{
	public interface IUser : IParseObject
	{
		string Name { get; set; }

		string Email { get; set; }

		string Sex { get; set; }

		DateTime DOB { get; set; }
	}
}

