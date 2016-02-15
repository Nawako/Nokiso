using System;

namespace Nokiso
{
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

		public static bool AppHasToken() { return AppToken == null || AppToken == String.Empty; }

		public static bool UserHasToken() { return UserToken == null || UserToken == String.Empty; }
	}
}

