using System;
using System.Collections.Generic;

namespace Nokiso
{
	public class Result
	{
		public string name { get; set; }
		public string uid { get; set; }
	}

	public class CategoryJson
	{
		public int code { get; set; }
		public List<Result> result { get; set; }
	}
}

