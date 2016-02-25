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
				//Console.WriteLine ("Result : {0}", data);
				await DisplayAlert ("New user", "Your account has been successfully activated", "OK");
				Registered ();
				//DisplayAlert ("New user", "Your account has been successfully activated", "OK");
				//Navigation.PopAsync ();
			} else {
				if (data.ToString().Contains("User"))
					DisplayAlert ("Oops", "User already exist", "OK");
				else 
					DisplayAlert ("Oops", "Something went wrong, please try again later", "OK");
				
				//Console.WriteLine ("Something went wrong with the request");
			}
		}

		private async void Registered () {
			Navigation.PopAsync ();
		}

		bool AreCredentialsCorrect ()
		{
			bool credits = false;
			if (mailEntry.Text.Length > 4 &&
				passwordEntry.Text.Length >= 6
				&& isLetters(firstNameEntry.Text)
				&& isLetters(lastNameEntry.Text))
				credits = true;
			else
				DisplayAlert ("Login / Password invalid", "Username should be superior to 4 characters, password should be superior to 6 characters, firstname and lastname should have only letters", "OK");

			return credits;
		}

		// Vérifie que c'est que des lettres
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

