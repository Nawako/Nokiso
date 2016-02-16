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
		private Editor pseudo;

		private Editor password;

		public StorePage ()
		{
			InitializeComponent ();
			this.Title = "Store page";

			GetStore ();
		}
			
		/* private void UpdateUI(JsonValue data)
		{
			int responseCode = data ["code"];

			if (data != null) {
				if (responseCode != 0) {
					if (responseCode != 401) {
						Console.WriteLine ("Something went wrong with the request");
						this.Result.Text = "Something went wrong with the request";
					} else {
						Console.WriteLine ("Invalid token or refresh token");
						this.Result.Text = "Invalid token or refresh token";
					}
				} else {
					Console.WriteLine ("Result : {0}", data);
					this.Result.Text = "Check in the logs";
				}
			}
		} */

		private async void GetStore()
		{
			Service s = new Service ("/store/list", "app");
			Task<JsonValue> result = s.CallAsync();
	
			JsonValue data = await result;

			if (!data.ContainsKey("erreur")) {
				Console.WriteLine ("Result : {0}", data);
			} else {
				Console.WriteLine ("Something went wrong with the request");
			}
			// UpdateUI (data);
		}

		private void OnClicked (object sender, EventArgs args) {
//			if () {
				this.Navigation.PushAsync(new StorePage());
//			}
		}
			
	}
}

