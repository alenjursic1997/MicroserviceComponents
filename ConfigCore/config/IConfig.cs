using ConfigCore.common.interfaces;
using System;
using System.Collections.Generic;

namespace ConfigCore.config
{
	public interface IConfig
	{
		/// <summary>
		/// Subscribe to value changes of given key
		/// </summary>
		/// <param name="callback"></param>
		void Subscribe<T>(string key, Action<T> callback);

		/// <summary>
		/// Get instances of all registred configuration sources
		/// </summary>
		/// <returns>List of configuration sources</returns>
		List<ConfigSource> GetConfigSources();

		/// <summary>
		/// Get value for given key
		/// </summary>
		/// <returns>Value of type T</returns>
		T Get<T>(string key);
		
	}
}
