using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Nokiso
{
	public partial class SignInPage : ContentPage
	{
		public SignInPage ()
		{
			this.Title = "Authentification";
			InitializeComponent ();
		}

		private async void OnSignInButtonClicked (object sender, EventArgs args) {
			User.Username = usernameEntry.Text;
			User.Password = passwordEntry.Text;

			var isValid = AreCredentialsCorrect ();
			if (isValid) {
				App.IsUserLoggedIn = true;
				Navigation.InsertPageBefore (new StorePage (), this);
				await Navigation.PopAsync ();
			} else {
				messageLabel.Text = "Login failed";
				passwordEntry.Text = string.Empty;
			}
		}

		async void OnSignUpButtonClicked (object sender, EventArgs args) {
			await Navigation.PushAsync (new SignUpPage ());
		}

		bool AreCredentialsCorrect ()
		{
			return User.Username == "Yannick" && User.Password == "Jussy971";
		}
	}
}

