using GalaSoft.MvvmLight;
using Xamarin.Forms;
using System;
using Microsoft.Practices.ServiceLocation;

namespace AshtangaTeacher
{
	public enum TeacherRole {
		Administrator,
		Moderator,
		None
	};

	public class Teacher : ObservableObject
	{
		string shalaName;
		string name;
		string userName;
		string email;
		string password;

		bool thumbIsDirty, isDirty;
		TeacherRole role;

		ImageSource image;

		public string TeacherId { get; set; }

		public string ObjectId { get; set; }

		public TeacherRole Role {
			get {
				return role;
			}
			set {
				Set (() => Role, ref role, value);
			}
		}

		public bool IsDirty {
			get {
				return isDirty;
			}
			set {
				Set (() => IsDirty, ref isDirty, value);
			}
		}

		public bool ThumbIsDirty {
			get {
				return thumbIsDirty;
			}
			set {
				Set (() => ThumbIsDirty, ref thumbIsDirty, value);
			}
		}

		public ImageSource Image {
			get {
				return image;
			}
			set {
				image = value;
				ThumbIsDirty = true;
				IsDirty = true;
				RaisePropertyChanged ("Image");
			}
		}

		public string Name {
			get {
				return name;
			}
			set {
				if (Set (() => Name, ref name, value)) {
					IsDirty = true;
				}
			}
		}

		public string UserName {
			get {
				return userName;
			}
			set {
				if (Set (() => UserName, ref userName, value)) {
					IsDirty = true;
				}
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
				if (Set (() => ShalaName, ref shalaName, value)) {
					IsDirty = true;
				}
			}
		}

		public Teacher() 
		{
			role = TeacherRole.None;
		}
	}
}
