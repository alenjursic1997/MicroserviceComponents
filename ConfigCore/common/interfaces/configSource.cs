using System;
using System.Threading.Tasks;

/// <summary>
/// This interface contains all methods that MUST be implemented in all config sources!
/// </summary>

namespace ConfigCore.common.interfaces
{
	public interface ConfigSource
	{
		/// <summary>
		/// Get name of configuration source
		/// </summary>
		/// <returns>Name of source</returns>
		string GetName();

		/// <summary>
		/// Get priority of configuration source
		/// </summary>
		/// <returns>Priority of source</returns>
		int GetPriority();

		/// <summary>
		/// Get value from configuration source for given key
		/// </summary>
		/// <param name="key">Key of wanted value</param>
		/// <returns>Value as string</returns>
		string GetValue(string key);

		/// <summary>
		/// Subscribe to value of given key
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="callback"></param>
		void Subscribe<T>(string key, Action<T> callback);

	}
}
