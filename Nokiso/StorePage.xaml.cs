using System;
using System.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using Xamarin.Forms;


namespace Nokiso
{
	public partial class StorePage : ContentPage
	{
		public StorePage ()
		{
			InitializeComponent ();
			this.Title = "Store page";

			GetStore ();
		}
			
		private void UpdateUI(JsonValue data)
		{
			int responseCode = data ["code"];

			if (data != null) {
				if (responseCode != 200 && responseCode != 0) {
					if (responseCode != 401) {
						Console.WriteLine ("Something went wrong with the request...");
						this.Result.Text = "Something went wrong with the request...";
					} else {
						Console.WriteLine ("Invalid token or refresh token");
						this.Result.Text = "Invalid token or refresh token";
					}
				} else {
					Console.WriteLine ("Result : {0}", data);
					this.Result.Text = "Check in the logs";
				}
			}
		}

		private async void GetStore()
		{
			// string Method = "GET";
			// string Operation = "/store/list";
			string Method = "POST";
			string Operation = "/oauth/token";
			string ContentType = "application/x-www-form-urlencoded";

			Dictionary<string, string> Body = new Dictionary<string, string> ();
			Body.Add ("grant_type", "client_credentials");
			Body.Add("client_id", "8a1d8939-7ded-4e0c-9cb1-a27748edad62");
			Body.Add("client_secret", "cdf2662153b94b1cef93a7513276256908fe8992");

			Dictionary<string, string> Headers = new Dictionary<string, string> ();
			Headers.Add ("Authorization", "Bearer faa3e81a3d784725ff8a8893e600200317fd14ae");
		
			Service s = new Service (Operation, Method, ContentType, Body, Headers);
			Task<JsonValue> result = s.CallAsync();

			JsonValue data = await result;
			Console.WriteLine ("Result : {0}", data);
			// UpdateUI (data);
		}
	}
}

