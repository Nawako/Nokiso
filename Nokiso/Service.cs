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
		public const string SERVER_URL = "http://xmazon.appspaces.fr";

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
		public HttpContent Body {
			get;
			set;
		}

		// will only contain Authorization
		public Dictionary<string, string> Headers {
			get;
			set;
		}

		// constructor
		public Service (string operation, string method, string contentType, Dictionary<string, string> body, Dictionary<string, string> headers)
		{
			Operation = operation;
			Method = method;
			ContentType = contentType;
			Body = new FormUrlEncodedContent (body);
			Headers = headers;
		}


		public async Task<JsonValue> CallAsync()
		{
			using (HttpClient request = new HttpClient ())
			{
				request.BaseAddress = new Uri (SERVER_URL);

				request.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue (ContentType));
				foreach (var param in Headers) {
					request.DefaultRequestHeaders.Add (param.Key, param.Value);	
				}

				HttpResponseMessage response;
				switch (Method) {
				case "GET":
					response = await request.GetAsync (Operation);
					break;
				case "POST":
					response = await request.PostAsync (Operation, Body);
					break;
				case "PUT":
					response = await request.PutAsync (Operation, Body);
					break;
				case "DELETE":
					response = await request.DeleteAsync (Operation);
					break;
				default:
					response = null;
					break;
				}
	
				if (response != null) {				
					using (Stream responseStream = await response.Content.ReadAsStreamAsync ()) {
						JsonValue data = await Task.Run (() => JsonObject.Load (responseStream));
						responseStream.Close ();

						return data;
					}
				} else {
					return null;
				}
			}
		}


		/* private string formatUrl(Dictionary<string, string> CallParams)
		{
			if (CallParams == null)
				return "";

			List<string> listParams = new List<string>();

			foreach (var param in CallParams) {
				listParams.Add(param.Key + "=" + param.Value);
			}

			return "?" + string.Join("&", listParams);
		} */
	}
}

