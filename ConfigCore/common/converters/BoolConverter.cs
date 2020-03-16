using ConfigCore.common.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigCore.common.converters
{
	public class BoolConverter : BaseTypeConverter<bool>
	{
		public override int Priority
		{
			get
			{
				return 1;
			}
		}

		public override bool Convert(string value)
		{
			if (bool.TryParse(value, out bool result))
				return result;

			switch (value.ToLower())
			{
				case "1": return true;
				case "0": return false;
				case "yes": return true;
				case "no": return false;
				case "y": return true;
				case "n": return false;
				default: return false;
			}
		}
	}
}
