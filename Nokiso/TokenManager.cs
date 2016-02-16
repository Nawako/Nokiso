using System;

namespace Nokiso
{
	// class reachable everywhere to get if the app has a token
	public static class TokenManager
	{
		public static string AppToken {
			get;
			set;
		}

		public static string UserToken {
			get;
			set;
		}

		public static bool AppHasToken() {
			if (AppToken == null || AppToken == String.Empty) {
				return false;
			} else {
				return true;
			}
			
		}

		public static bool UserHasToken() {
			if (UserToken == null || UserToken == String.Empty) {
				return true;
			} else {
				return false;
			}
		
		}
	}
}

