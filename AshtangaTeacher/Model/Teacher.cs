using GalaSoft.MvvmLight;

namespace AshtangaTeacher
{
	public class Teacher : ObservableObject
	{
		string shalaName;
		string name;
		string email;
		string password;

		public string ObjectId {
			get;
			set;
		}

		public string Image {
			get;
			set;
		}

		public string Name {
			get {
				return name;
			}
			set {
				Set (() => Name, ref name, value);
			}
		}

		public string Email {
			get {
				return email;
			}
			set {
				Set (() => Email, ref email, value);
			}
		}

		public string Password {
			get {
				return password;
			}
			set {
				Set (() => Password, ref password, value);
			}
		}

		public string ShalaName {
			get {
				return shalaName;
			}
			set {
				Set (() => ShalaName, ref shalaName, value);
			}
		}
	}
}
