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

			GetData ();
		}

		private async void GetData()
		{
			// string Method = "POST";
			string Method = "GET";
			string Operation = "/store/list";
			// string Operation = "/oauth/token";
			string ContentType = "application/x-www-form-urlencoded";

			HttpContent Body = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("grant_type", "client_credentials"), 
				new KeyValuePair<string, string>("client_id", "8a1d8939-7ded-4e0c-9cb1-a27748edad62"), 
				new KeyValuePair<string, string>("client_secret", "cdf2662153b94b1cef93a7513276256908fe8992"), 
				new KeyValuePair<string, string>("refresh_token", ""),
				new KeyValuePair<string, string>("username", ""),
				new KeyValuePair<string, string>("password", ""),
				new KeyValuePair<string, string>("code", ""),
				new KeyValuePair<string, string>("redirect_ui", ""),
			});
			
			string AccessToken = "INSERT_VALID_TOKEN";
		
			Service s = new Service (Operation, Method, ContentType, Body, AccessToken);
			Task<JsonValue> result = s.CallAsync();

			JsonValue data = await result;
			int responseCode = data ["code"];

			if (data != null) {
				if (responseCode != 200 && responseCode != 0) {
					if (responseCode != 401) {
						Console.WriteLine ("Something went wrong with the request...");
						this.Result.Text = "Something went wrong with the request...";
					} else {
						Console.WriteLine ("Refresh token");
						this.Result.Text = "Refresh token";
					}
				} else {
					Console.WriteLine ("Result : {0}", data);
					this.Result.Text = "Check in the logs";
				}
			}
		}
	}
}

