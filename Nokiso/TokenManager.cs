using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Json;
using System.Threading.Tasks;

namespace Nokiso
{
	public class TokenManager
	{
		private const string Method = "POST";
		private const string Operation = "/oauth/token";
		private const string ContentType = "application/x-www-form-urlencoded";
		private KeyValuePair<string, string> refreshToken = new KeyValuePair<string, string> ("refresh_token", "");

		private readonly KeyValuePair<string, string> clientId = new KeyValuePair<string, string>("client_id", "8a1d8939-7ded-4e0c-9cb1-a27748edad62");
		private readonly KeyValuePair<string, string> clientSecret = new KeyValuePair<string, string>("client_secret", "cdf2662153b94b1cef93a7513276256908fe8992");

	
		public async void GetAppToken() {
			HttpContent Body = new FormUrlEncodedContent(new[]
				{
					clientId,
					clientSecret,
					new KeyValuePair<string, string>("grant_type", "client_credentials")
				});
					
			Console.WriteLine ("In GetAppToken");

			// Task<JsonValue> result = GetData (Body);
			// JsonValue data = await result;

			// Console.WriteLine ("Data : {0}", data);
			// int responseCode = data ["code"];
			// JsonValue data = GetData (Body);

			// return (string) data["access_token"]; 
		}

		public string RefreshAppToken()
		{
			HttpContent Body = new FormUrlEncodedContent(new[]
				{
					clientId,
					clientSecret,
					new KeyValuePair<string, string>("grant_type", "refresh_token"), 
					refreshToken
				});

			// JsonValue data = GetData (Body).Result;
			//return (string) data["access_token"]; 
			return "";
		}

		public string GetClientToken()
		{
			HttpContent Body = new FormUrlEncodedContent(new[]
				{
					clientId,
					clientSecret,
					new KeyValuePair<string, string>("grant_type", "password"), 
					new KeyValuePair<string, string>("username", ""),
					new KeyValuePair<string, string>("password", ""),
				});
			
			// JsonValue data = GetData (Body).Result;
			// return (string) data["access_token"]; 
			return "";
		}

		public string RefreshClientToken()
		{
			HttpContent Body = new FormUrlEncodedContent(new[]
				{
					clientId,
					clientSecret,
					new KeyValuePair<string, string>("grant_type", "refresh_token"), 
					refreshToken,
					new KeyValuePair<string, string>("username", ""),
					new KeyValuePair<string, string>("password", ""),
				});

			// JsonValue data = GetData (Body).Result;
			// return (string) data["access_token"]; 
			return "";
		}

		/* private async Task<JsonValue> GetData(HttpContent Body)
		{
			Service s = new Service (Operation, Method, ContentType, Body, "");

			Task<JsonValue> result = s.CallAsync();
			JsonValue data = await result;

			return data;
		} */

	}
}

