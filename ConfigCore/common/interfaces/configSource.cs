using System;
using System.Threading.Tasks;

/// <summary>
/// This interface contains all methods that MUST be implemented in all config sources!
/// </summary>

namespace ConfigCore.common.interfaces
{
	public interface ConfigSource
	{
		string GetName();

		int GetPriority();

		string GetValue(string key);

		void Subscribe<T>(string key, Action<T> callback);

	}
}
