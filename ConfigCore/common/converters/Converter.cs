using ConfigCore.common.interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigCore.common.converters
{
	public class Converter : IConverter
	{
		private List<object> _converters;
		private readonly ILogger _logger;

		public Converter(ILogger logger)
		{
			_logger = logger;

			_logger.LogInformation($"Creating instances of TypeConverter implementations");

			//getting all implementations of ITypeConverter

			var lookup = typeof(BaseTypeConverter<>);
			_converters = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.Where(x => x.IsClass && !x.IsAbstract && IsInheritedFrom(x, lookup))
				.Select(x => Activator.CreateInstance(x))
				.ToList();
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

		public T ConvertTo<T>(string value)
		{
			//if value is null, return default value for required type
			if (value == null)
				return default(T);

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
				catch
				{

				}
			}

			//if there is no useful converter, then use Convert.ChangeType method
			try
			{
				FormatValueDependOnType(ref value, typeof(T));
				return (T)Convert.ChangeType(value, typeof(T));
			}
			catch
			{
				return default(T);
			}
		}

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
