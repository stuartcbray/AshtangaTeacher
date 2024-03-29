﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AshtangaTeacher
{
	public class DialogService
	{
		static DialogService instance;
		static object instanceLock = new object();

		public static DialogService Instance {
			get {
				if (instance == null) {
					lock (instanceLock) {
						if (instance == null)
							instance = new DialogService ();
					}
				}
				return instance;
			}
		}

		DialogService ()
		{
		}

		Page _dialogPage;

		public void Initialize(Page dialogPage)
		{
			_dialogPage = dialogPage;
		}

		public async Task ShowError(string message, 
			string title, 
			string buttonText, 
			Action afterHideCallback)
		{
			await _dialogPage.DisplayAlert(
				title,
				message,
				buttonText);

			if (afterHideCallback != null)
			{
				afterHideCallback();
			}
		}

		public async Task ShowError(
			Exception error, 
			string title, 
			string buttonText, 
			Action afterHideCallback)
		{
			await _dialogPage.DisplayAlert(
				title,
				error.Message,
				buttonText);

			if (afterHideCallback != null)
			{
				afterHideCallback();
			}
		}

		public async Task ShowMessage(
			string message, 
			string title)
		{
			await _dialogPage.DisplayAlert(
				title,
				message,
				"OK");
		}

		public async Task ShowMessage(
			string message, 
			string title, 
			string buttonText, 
			Action afterHideCallback)
		{
			await _dialogPage.DisplayAlert(
				title,
				message,
				buttonText);

			if (afterHideCallback != null)
			{
				afterHideCallback();
			}
		}

		public async Task<bool> ShowMessage(
			string message,
			string title,
			string buttonConfirmText,
			string buttonCancelText,
			Action<bool> afterHideCallback)
		{
			var result = await _dialogPage.DisplayAlert(
				title,
				message,
				buttonConfirmText,
				buttonCancelText);

			if (afterHideCallback != null)
			{
				afterHideCallback(result);
			}
			return result;
		}

		public async Task ShowMessageBox(
			string message, 
			string title)
		{
			await _dialogPage.DisplayAlert(
				title,
				message,
				"OK");
		}
	}
}
