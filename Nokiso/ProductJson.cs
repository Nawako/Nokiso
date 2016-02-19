using System;
using System.Collections.Generic;

namespace Nokiso
{
	public class ProductJson
	{
		public class Result
		{
			public bool available { get; set; }
			public string name { get; set; }
			public double price { get; set; }
			public string uid { get; set; }
		}

		public int code { get; set; }
		public List<Result> result { get; set; }
	}
}

