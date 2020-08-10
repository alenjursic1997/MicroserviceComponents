using Consul;
using SemVer;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscoveryCore.common.models
{
	public class DiscoveredService
	{
		public string Id { get; set; }
		public string DirectURL { get; set; }
		public Version Version { get; set; }
		public string Protocol { get; set; }

		public DiscoveredService(ServiceEntry entry) 
		{
			if (entry?.Service == null)
				return;

			Id = entry.Service.ID;
			Protocol = (entry.Service.Tags.Where(e => e == "https").Any()) ? "https" : "http"; 

			var tag = entry.Service.Tags.Where(e => e.ToLower().Contains("version")).First();
			tag = tag.Split('=')[1] ?? "";

			try
			{
				Version = new Version(tag);
			}
			catch
			{
				Version = null;
			}
			if (Version == null) return;

			var addr = entry.Service.Address ?? $"{Protocol}://{entry.Node.Address}";
			DirectURL = $"{addr}:{entry.Service.Port}";
		} 
	}
}
