using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Nokiso
{
	public partial class SignUpPage : ContentPage
	{
		public SignUpPage ()
		{
			this.Title = "Sign Up";
			InitializeComponent ();
		}

		private async void OnRegisterButtonClicked (object sender, EventArgs args) {

			if (AreCredentialsCorrect() && IsValidEmail(mailEntry.Text)) {
				PostUser ();
			} else {
				passwordEntry.Text = string.Empty;
			}
		}

		private async void PostUser ()
		{
			Dictionary<string,string> body = new Dictionary<string, string> ();
			body.Add ("email", mailEntry.Text);
			body.Add ("firstname", firstNameEntry.Text);
			body.Add ("lastname", lastNameEntry.Text);
			body.Add ("password", passwordEntry.Text);
			
			Service s = new Service ("/auth/subscribe", "POST", body, "app");
			Task<JsonValue> result = s.CallAsync();

			JsonValue data = await result;

			if (!data.ContainsKey("erreur")) {
				await DisplayAlert ("New user", "Your account have successfully been activated", "OK");
				Registered ();
			} else {
				if (data.ToString ().Contains ("User")) {
					DisplayAlert ("Oops", "User already exist", "OK");
				} else if (data.ContainsKey ("erreur")) {
					DisplayAlert ("Erreur", data ["erreur"], "OK");
				} else { 
					DisplayAlert ("Oops", "Something went wrong, please try again later", "OK");
				}
			}
		}

		private async void Registered () {
			Navigation.PopAsync ();
		}

		bool AreCredentialsCorrect ()
		{
			bool credits = false;


			if (mailEntry.Text != null && mailEntry.Text != String.Empty &&
				passwordEntry.Text != null && passwordEntry.Text != String.Empty &&
				firstNameEntry.Text != null && firstNameEntry.Text != String.Empty &&
				lastNameEntry.Text != null && lastNameEntry.Text != String.Empty) {

				if (mailEntry.Text.Length > 4 &&
				    passwordEntry.Text.Length >= 6 &&
				    isLetters (firstNameEntry.Text) &&
				    isLetters (lastNameEntry.Text)) {
					credits = true;
				} else {
					DisplayAlert ("Login / Password invalid", "Username should be superior to 4 characters, password should be superior to 6 characters, firstname and lastname should have only letters.", "OK");
			
				}
			} else {
				DisplayAlert ("One of the fields are empty", "Please fill up all fields.", "OK");
			}

			return credits;
		}

		// check string is only composed of letters
		bool isLetters (string parse) {
			return Regex.IsMatch(parse, @"^[a-zA-Z]+$");
		}

		bool IsValidEmail(string email)
		{
			try {
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch {
				return false;
			}
		}
	}
}

