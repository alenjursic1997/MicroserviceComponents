using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigCore.common.interfaces
{
	public interface IConverter
	{
		T ConvertTo<T>(string value);
	}
}
