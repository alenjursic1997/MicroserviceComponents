using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigCore.common.interfaces
{
	public interface IConverter
	{
		/// <summary>
		/// Convert given string value to value of wanted type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		T ConvertTo<T>(string value);
	}
}
