using ConfigCore.common.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigCore.common.converters
{
	public class StringListConverter : BaseTypeConverter<List<string>>
	{
		public override int Priority
		{
			get
			{
				return 1;
			}
		}

		public override List<string> Convert(string value)
		{
			List<string> res = new List<string>();
			foreach (string val in value.Split(','))
			{
				res.Add(val);
			}
			return res;
		}
	}
}
