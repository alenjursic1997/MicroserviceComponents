using ConfigCore.common.interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ConfigCore.common.converters
{
	/// <summary>
	/// Converter class is used for converting string values to wanted type.
	/// </summary>
	public class Converter : IConverter
	{
		private List<object> _converters;
		private readonly ILogger _logger;

		public Converter(ILogger logger)
		{
			//set logger instance
			_logger = logger;

			_logger.LogInformation($"Creating instances of TypeConverter implementations");

			//getting all implementations of BaseTypeConverter
			var lookup = typeof(BaseTypeConverter<>);
			_converters = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.Where(x => x.IsClass && !x.IsAbstract && IsInheritedFrom(x, lookup))
				.Select(x => Activator.CreateInstance(x))
				.ToList();
			//order converters by their priority number
			_converters.OrderByDescending(e => e.GetType().GetProperty("Priority").GetValue(e)).ToList();
		}

		private bool IsInheritedFrom(Type type, Type Lookup)
		{
			var baseType = type.BaseType;
			if (baseType == null)
				return false;

			if (baseType.IsGenericType
					&& baseType.GetGenericTypeDefinition() == Lookup)
				return true;

			return IsInheritedFrom(baseType, Lookup);
		}

		//convert value from string to type T
		public T ConvertTo<T>(string value)
		{
			//if value is null, return default value for required type
			if (value == null)
				return default(T);

			//firstly check if there is any custom converter that can convert string to type T
			foreach(var converter in _converters)
			{
				try
				{
					if (converter.GetType().GetMethod("Convert").ReturnType == typeof(T))
					{
						var method = converter.GetType().GetMethod("Convert");
						return (T)method.Invoke(converter, new object[] { value });
					}
				}
				catch { }
			}

			//if there is no useful converter, then use Convert.ChangeType method
			try
			{
				if (Nullable.GetUnderlyingType(typeof(T)) != null)
				{
					TypeConverter conv = TypeDescriptor.GetConverter(Nullable.GetUnderlyingType(typeof(T)));
					return (T)conv.ConvertFrom(value);
				}

				FormatValueDependOnType(ref value, typeof(T));
				return (T)Convert.ChangeType(value, typeof(T));
			}
			catch(Exception e) 
			{
				//id value convertion has failed, return default value of type T
				return default(T);
			}
		}

		//replacing dots with commas if type is included it list 'types'
		private void FormatValueDependOnType(ref string value, Type type)
		{
			var types = new List<Type>()
			{
				typeof(Double),
				typeof(double),
				typeof(Decimal),
				typeof(decimal),
				typeof(float)
			};

			if (types.Any(e => e == type))
			{
				value = value.Replace(".", ",");
			}
		}
	}
}
