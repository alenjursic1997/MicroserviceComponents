using ConfigCore.common.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigCore.config
{
	public interface IConfig
	{
		void Subscribe<T>(string key, Action<T> callback);

		List<ConfigSource> GetConfigSources();

		T Get<T>(string key);
		
	}
}
