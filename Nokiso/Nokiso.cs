using System;

using Xamarin.Forms;

namespace Nokiso
{
	public class App : Application
	{
		public static bool IsUserLoggedIn { get; set; }

		public App ()
		{
			// The root page of your application
			if (!IsUserLoggedIn) {
				MainPage = new NavigationPage (new SignInPage ());
			} else {
				MainPage = new NavigationPage (new StorePage ());
			}
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

