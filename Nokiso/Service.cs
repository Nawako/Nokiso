using System;
using System.Text;
using System.IO;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Nokiso
{
	public class Service
	{
		private const string SERVER_URL = "http://xmazon.appspaces.fr";
		private static int callsNumber = 0;
		private TokenManager TkMgr;
		private bool firstCall = true;

		// operation that'll be executed on the server
		public string Operation {
			get;
			set;
		}

		// "GET", "POST", etc.
		public string Method {
			get;
			set;
		}

		// will be "application/x-www-form-urlencoded" most of the time
		public string ContentType {
			get;
			set;
		}

		// parameters in the body of the request
		public Dictionary<string, string> Body {
			get;
			set;
		}

		// will only contain Authorization
		public Dictionary<string, string> Headers {
			get;
			set;
		}
			

		public Service()
		{
			this.Method = "GET";
			this.ContentType = "application/x-www-form-urlencoded";
			this.Headers = new Dictionary<string, string> ();
			this.Body = new Dictionary<string, string> ();
		}

		// for operation with get method
		public Service (string operation) : this()
		{
			this.Operation = operation;
		}
			
		// constructor used when initializing a TokenManager instance
		public Service (string operation, string method, Dictionary<string, string> body) : this()
		{
			this.Operation = operation;
			this.Method = method;
			this.Body = body;
		}

		// for operation with get method, body for specific use of the operation
		public Service (string operation, Dictionary<string, string> body) : this()
		{
			this.Operation = operation;
			this.Body = body;
		}

		// custom call
		public Service (string operation, string method, string contentType, Dictionary<string, string> headers, Dictionary<string, string> body)
		{
			this.Operation = operation;
			this.Method = method;
			this.ContentType = contentType;
			this.Body = body;
			this.Headers = headers;
		}
			

		public async Task<JsonValue> CallAsync()
		{
			using (HttpClient request = new HttpClient ())
			{
				request.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue (this.ContentType));

				// get a token only if the operation is /oauth/token
				// otherwise we'd get an infinite loop
				if (firstCall && this.Operation != "/oauth/token") {
					TkMgr = new TokenManager();
					Task<JsonValue> resultGetAppToken = TkMgr.GetAppToken ();
					JsonValue getTokenData = await resultGetAppToken;
					string accessToken = getTokenData ["access_token"];

					this.Headers.Add("Authorization", "Bearer " + accessToken);
					this.firstCall = false;
				}
					
				if (this.Headers.Count > 0) {
					foreach (var param in this.Headers) {
						request.DefaultRequestHeaders.Add (param.Key, param.Value);	
					}
				}

				request.BaseAddress = new Uri (SERVER_URL);
				HttpContent body = new FormUrlEncodedContent (this.Body);

				HttpResponseMessage response = null;
				if (this.Method != null && this.Operation != null) {
					switch (this.Method) {
					case "GET":
						response = await request.GetAsync (this.Operation);
						break;
					case "POST":
						response = await request.PostAsync (this.Operation, body);
						break;
					case "PUT":
						response = await request.PutAsync (this.Operation, body);
						break;
					case "DELETE":
						response = await request.DeleteAsync (this.Operation);
						break;
					default:
						response = null;
						break;
					}
				}
	
				if (response != null) {
					using (Stream responseStream = await response.Content.ReadAsStreamAsync ()) {
						JsonValue data = await Task.Run (() => JsonObject.Load (responseStream));
						responseStream.Close ();

						// if the operation is to get or refresh a token, we can return data
						// no "code" key in the response of "/oauth/token" operations
						if (response.IsSuccessStatusCode && this.Operation == "/oauth/token") {
							return data;
						}
						
						int responseCode = data["code"];
						if (responseCode != 0) {
							TkMgr = new TokenManager();
							Task<JsonValue> resultRefreshAppToken = TkMgr.RefreshAppToken ();
							JsonValue refreshData = await resultRefreshAppToken;

							if (refreshData != null) {
								if (!refreshData.ContainsKey("access_token")) {
									return null;
								} else {
									string newToken = refreshData ["access_token"];
									this.Headers ["Authorization"] = "Bearer " + newToken;
									await this.CallAsync ();
								}
							} else {
								return null;
							}
						}

						return data;
					}
				}

				return null;
			}
		}

		class TokenManager
		{
			private Service Service;
			private const string Method = "POST";
			private const string Operation = "/oauth/token";
			private const string ContentType = "application/x-www-form-urlencoded";
			private string refreshAppToken;
			private string refreshClientToken;

			// get is public, in case we need to get the grant_type
			public Dictionary<string,string> Body {
				get;
				private set;
			}

			public TokenManager()
			{
				this.Body = new Dictionary<string, string>();

				this.Body.Add("grant_type", "client_credentials");
				this.Body.Add("client_id", "8a1d8939-7ded-4e0c-9cb1-a27748edad62");
				this.Body.Add("client_secret", "cdf2662153b94b1cef93a7513276256908fe8992");
				this.Body.Add("refresh_token", "");
				this.Body.Add("username", "");
				this.Body.Add("password", "");

				this.Service = new Service(Operation, Method, Body);
			}

			public async Task<JsonValue> GetAppToken()
			{
				this.Body ["grant_type"] = "client_credentials";
				Task<JsonValue> result = this.Service.CallAsync ();
				JsonValue data = await result;

				// CallAsync already checked for successful answer
				this.refreshAppToken = data ["refresh_token"];
				return data;
			}

			public async Task<JsonValue> RefreshAppToken()
			{
				this.Body ["grant_type"] = "refresh_token";
				this.Body ["refresh_token"] = refreshAppToken;
				Task<JsonValue> result = this.Service.CallAsync ();
				JsonValue data = await result;

				this.refreshAppToken = data ["refresh_token"];
				return data;
			}

			// grant_type : password
			public void GetClientToken()
			{
				
			}

			// grant_type : refresh_token et mettre le refresh_token correspondant
			public void RefreshClientToken()
			{
				
			}
		}
	}
}
