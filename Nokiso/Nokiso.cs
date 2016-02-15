using System;

using Xamarin.Forms;

namespace Nokiso
{
	public class App : Application
	{
		public App ()
		{
			// The root page of your application
			MainPage = new NavigationPage(new StorePage());
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
			User.Username = "Foo";
			User.Password = "Bar";

			TokenManager.AppToken = "";
			TokenManager.UserToken = "";
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

