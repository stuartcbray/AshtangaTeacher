using GalaSoft.MvvmLight;

namespace AshtangaTeacher
{
	public class Teacher : ObservableObject
	{
		string shalaName;
		string name;
		string userName;
		string email;
		string password;
		string imageUrl;

		public string ObjectId {
			get;
			set;
		}

		public string ImageUrl {
			get {
				return imageUrl;
			}
			set {
				Set (() => ImageUrl, ref imageUrl, value);
			}
		}

		public string Name {
			get {
				return name;
			}
			set {
				Set (() => Name, ref name, value);
			}
		}

		public string UserName {
			get {
				return userName;
			}
			set {
				Set (() => UserName, ref userName, value);
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
