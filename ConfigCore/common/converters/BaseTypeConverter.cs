using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigCore.common.converters
{
	public abstract class BaseTypeConverter<T>
	{
		public abstract T Convert(string value); 

		public abstract int Priority
		{
			get;
		}
	}
}
