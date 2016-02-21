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

		// "app" or "user", the token changes accordingly
		public string Context {
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
			Method = "GET"; 
			ContentType = "application/x-www-form-urlencoded"; 
			Headers = new Dictionary<string, string> (); 
			Body = new Dictionary<string, string> (); 
			Context = "app";
		}

		// for operation with get method
		public Service (string operation, string context) : this()
		{
			Operation = operation;
			Context = context;
		}
			
		// constructor used when initializing a TokenService instance
		public Service (string operation, string method, Dictionary<string, string> body) : this()
		{
			Operation = operation;
			Method = method;
			Body = body;
		}

		// for operation with get method, body for specific use of the operation
		public Service (string operation, Dictionary<string, string> body, string context) : this()
		{
			Operation = operation;
			Context = context;
			Body = body;
		}

		// custom call
		public Service (string operation, string method, string contentType, Dictionary<string, string> headers, Dictionary<string, string> body, string context) : this()
		{
			Operation = operation;
			Method = method;
			ContentType = contentType;
			Body = body;
			Headers = headers;
			Context = context;
		}


		public async Task<JsonValue> CallAsync()
		{
			JsonObject error = new JsonObject ();

			using (HttpClient Request = new HttpClient ())
			{
				Request.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue (ContentType));

				// get a token only if the operation isn't /oauth/token
				// don't need a token in the headers for these operations
				if (Operation != "/oauth/token") {
					if (Context == "app") {
						if (!TokenManager.AppHasToken()) {
							await TokenService.GetAppToken ();
						}
						Headers["Authorization"] = "Bearer " + TokenManager.AppToken;
					} else {
						if (!TokenManager.UserHasToken()) {
							await TokenService.GetUserToken ();
						}
						Headers["Authorization"] = "Bearer " + TokenManager.UserToken;
					}
				}

				if (Headers.Count > 0) {
					foreach (var HeadersParam in this.Headers) {
						Request.DefaultRequestHeaders.Add (HeadersParam.Key, HeadersParam.Value);	
					}
				}

				Request.BaseAddress = new Uri (SERVER_URL);
				HttpContent RequestBody = new FormUrlEncodedContent (Body);

				HttpResponseMessage Response = null;
				if (Method != null && Operation != null) {
					switch (Method) {
					case "GET":
							Response = await Request.GetAsync (Operation + formatUrl (Body));
						break;
					case "POST":
						Response = await Request.PostAsync (Operation, RequestBody);
						break;
					case "PUT":
						Response = await Request.PutAsync (Operation, RequestBody);
						break;
					case "DELETE":
						Response = await Request.DeleteAsync (Operation);
						break;
					default:
						Response = null;
						break;
					}
				}
	
				// get ready for new request
				// Body.Clear ();
				// Console.WriteLine("Operation : {0}", Operation);
				// Console.WriteLine("Response : {0}", Response);

				if (Response != null) {
					using (Stream ResponseStream = await Response.Content.ReadAsStreamAsync ()) {
						JsonValue ResponseData = await Task.Run (() => JsonObject.Load (ResponseStream));
						ResponseStream.Close ();

						// if the operation is to get or refresh a token, we can return data
						// no "code" key in the response of "/oauth/token" operations
						if (Operation == "/oauth/token") {
							if (Response.IsSuccessStatusCode) {
								return ResponseData;
							} else {
								error.Add ("erreur", "Something went wrong while retrieving a token.");
								return error;
							}
						}

						int ResponseCode;
						if (ResponseData.ContainsKey ("code")) {
							ResponseCode = ResponseData ["code"];
						} else {
							error.Add ("erreur", "Something went wrong while parsing the response data.");
							return error;
						}

						if (ResponseCode != 0) {
							JsonValue RefreshedTokenData;
							if (Context == "app") {
								Task<JsonValue> ResultRefreshedToken = TokenService.RefreshAppToken ();
								RefreshedTokenData = await ResultRefreshedToken;
							} else {
								Task<JsonValue> ResultRefreshedToken = TokenService.RefreshUserToken ();
								RefreshedTokenData = await ResultRefreshedToken;
							}

							if (RefreshedTokenData != null) {
								if (!RefreshedTokenData.ContainsKey("access_token")) {
									error.Add ("erreur", "Something went wrong while parsing the result of the refresh token.");
									return error;
								} else {
									Headers ["Authorization"] = "Bearer " + RefreshedTokenData ["access_token"];

									// re-execute the call but this time the Header has a valid token
									Task<JsonValue> ResultNewCallAsync = CallAsync ();
									JsonValue NewCallAsyncData = await ResultNewCallAsync;

									return NewCallAsyncData;
								}
							} else {
								error.Add ("erreur", "Something went wrong while refreshing the token.");
								return error;
							}
						}

						return ResponseData;

					} // </Using stream response>
				} // </Response != null>

				error.Add ("erreur", "The request did not send back any response");
				return error;
			} // </Using HttpClient>
		}

		private string formatUrl(Dictionary<string, string> CallParams)
		{
			if (CallParams.Count == 0)
				return "";
			
			List<string> listParams = new List<string>();
			foreach (var param in CallParams) {
				listParams.Add(param.Key + "=" + param.Value);
			}

			return "?" + string.Join("&", listParams);
		}


		// set up the requests to get / refresh tokens
		public class TokenService
		{
			private static readonly string Method = "POST";
			private static readonly string Operation = "/oauth/token";
			private static string RefreshAppTokenP;
			private static string RefreshUserTokenP;

			private static Dictionary<string, string> Body = new Dictionary<string, string>();
			private static Service CallToken = new Service(Operation, Method, Body);
		

			public static async Task<JsonValue> GetAppToken()
			{
				CallToken.Body ["grant_type"] = "client_credentials";
				CallToken.Body ["client_id"] = "8a1d8939-7ded-4e0c-9cb1-a27748edad62";
				CallToken.Body ["client_secret"] = "cdf2662153b94b1cef93a7513276256908fe8992";

				Task<JsonValue> Result = CallToken.CallAsync ();
				JsonValue Data = await Result;

				if (Data.ContainsKey("refresh_token")) {
					RefreshAppTokenP = Data ["refresh_token"];	
				}

				TokenManager.AppToken = Data ["access_token"];
				return Data;
			}

			public static async Task<JsonValue> RefreshAppToken()
			{
				CallToken.Body ["grant_type"] = "refresh_token";
				CallToken.Body ["refresh_token"] = RefreshAppTokenP;

				Task<JsonValue> Result = CallToken.CallAsync ();
				JsonValue Data = await Result;

				if (Data.ContainsKey("refresh_token")) {
					RefreshAppTokenP = Data ["refresh_token"];
				}
					
				TokenManager.AppToken = Data ["access_token"];
				return Data;
			}

			public static async Task<JsonValue> GetUserToken()
			{
				CallToken.Body ["grant_type"] = "password";
				CallToken.Body ["client_id"] = "8a1d8939-7ded-4e0c-9cb1-a27748edad62";
				CallToken.Body ["client_secret"] = "cdf2662153b94b1cef93a7513276256908fe8992";
				CallToken.Body ["username"] = User.Username; 
				CallToken.Body ["password"] = User.Password;
				
				Task<JsonValue> Result = CallToken.CallAsync ();
				JsonValue Data = await Result;

				if (Data.ContainsKey("refresh_token")) {
					RefreshUserTokenP = Data ["refresh_token"];
				}

				TokenManager.UserToken = Data ["access_token"];
				return Data;
			}

			public static async Task<JsonValue> RefreshUserToken()
			{
				CallToken.Body ["grant_type"] = "refresh_token";
				CallToken.Body ["refresh_token"] = RefreshUserTokenP;
				
				Task<JsonValue> Result = CallToken.CallAsync ();
				JsonValue Data = await Result;

				if (Data.ContainsKey("refresh_token")) {
					RefreshUserTokenP = Data ["refresh_token"];
				}

				TokenManager.UserToken = Data ["access_token"];
				return Data;
			}
		} // </class TokenService>
	} // </class Service>
} // </Namespace>
