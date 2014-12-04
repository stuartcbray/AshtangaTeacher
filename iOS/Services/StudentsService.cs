using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Parse;
using Xamarin.Forms;
using AshtangaTeacher.iOS;

[assembly: Xamarin.Forms.Dependency (typeof (StudentsService))]

namespace AshtangaTeacher.iOS
{
	public class StudentsService : IStudentsService
	{
		public async Task<ObservableCollection<StudentViewModel>> GetAllAsync(string shalaName)
		{
			var query = ParseObject.GetQuery ("Student").Where (student => student.Get<string> ("shalaNameLC") == shalaName.ToLower ());
			IEnumerable<ParseObject> results = await query.FindAsync();

			// Consider returning ObservableCollection instead
			var list = new ObservableCollection<StudentViewModel> ();
			foreach (var s in results) {
			
				var student = new Student ();
				await student.InitializeAsync (s);
				var vm = new StudentViewModel (student);

				// Try the local cache first
				var cameraService = DependencyService.Get<ICameraService> ();
				var imgPath = cameraService.GetImagePath (vm.Model.UID);

				bool fetchImage = true;
				if (File.Exists (imgPath)) {
					var dt = File.GetLastWriteTimeUtc (imgPath);
					if (s.UpdatedAt != null && s.UpdatedAt <= dt) {
						vm.Model.Image = ImageSource.FromFile (imgPath);
						fetchImage = false;
					} 
				}

				if (fetchImage) {
					// Load from Parse
					var parseImg = s.Get<ParseFile>("image");
					byte[] imgData = await new HttpClient ().GetByteArrayAsync (parseImg.Url);
					vm.Model.Image = ImageSource.FromStream(() => new MemoryStream(imgData));

					var deviceService = DependencyService.Get<IDeviceService> ();
					deviceService.SaveToFile (imgData, imgPath);
				}

				vm.Model.IsDirty = false;
				vm.Model.ThumbIsDirty = false;
				list.Add (vm);
			}
			return list;
		}
	}
}
