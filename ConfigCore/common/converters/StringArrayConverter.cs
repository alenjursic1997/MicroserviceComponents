using ConfigCore.common.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigCore.common.converters
{
	public class StringArrayConverter : BaseTypeConverter<string[]>
	{
		public override int Priority
		{
			get
			{
				return 1;
			}
		}

		public override string[] Convert(string value)
		{
			List<string> res = new List<string>();
			foreach(string val in value.Split(','))
			{
				res.Add(val); 
			}
			return res.ToArray();
		}
	}
}
