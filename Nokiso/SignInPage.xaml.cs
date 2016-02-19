using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Json;
using System.Threading.Tasks;

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

			Task<JsonValue> taskToken = Service.TokenService.GetUserTokenParam(usernameEntry.Text, passwordEntry.Text);
			JsonValue resultAppToken = await taskToken;
			Console.WriteLine ("Foobar : {0}", resultAppToken);
			Console.WriteLine (TokenManager.UserToken);

			if (TokenManager.UserToken != string.Empty) {
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
	}
}

