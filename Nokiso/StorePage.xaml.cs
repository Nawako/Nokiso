using System;
using System.IO;
using System.Net;
using System.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

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
			FetchAsync ("http://appspaces.fr/esgi/jsontest.php");
		}

		private void Display(JsonValue value)
		{
			var date = value ["result"] ["date"];
			Console.WriteLine ("{0}", date);
			this.LabelDate.Text = date;
		}

		private async Task<JsonValue> FetchAsync (string url)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri(url));
			request.ContentType = "application/json";
			request.Method = "GET";

			using (WebResponse response = await request.GetResponseAsync ()) 
			{
				using (Stream stream = response.GetResponseStream ()) 
				{
					JsonValue json = await Task.Run (() => JsonObject.Load (stream));
					Display (json);
					return null;
				}
			}
		}

		private void OnClicked (object sender, EventArgs args) {
//			if () {
				this.Navigation.PushAsync(new StorePage());
//			}
		}
			
	}
}

