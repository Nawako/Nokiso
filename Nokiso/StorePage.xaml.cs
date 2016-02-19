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
				string uid = data["result"][0]["uid"];

				s.Operation = "/category/list";
				s.Body.Add ("store_uid", uid);
				Task<JsonValue> result_category = s.CallAsync ();
				JsonValue data_category = await result_category;

				Console.WriteLine ("Result category : {0}", data_category);

			} else {
				Console.WriteLine ("Something went wrong with the request");
			}


			// UpdateUI (data);
		}
	}
}

