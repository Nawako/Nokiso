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
			// TMP : get username / password from a login form
			User.Username = "Foo";
			User.Password = "Bar";
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

