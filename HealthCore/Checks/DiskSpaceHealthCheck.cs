using HealthCore.Models;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HealthCore.Checks
{
	public class DiskSpaceHealthCheck : HealthCheck
	{
		private long _defaultTreshold;
		private HealthCheckResponse response = new HealthCheckResponse();
		private Dictionary<string, object> _data = new Dictionary<string, object>();

		private const string REQUIRED_SPACE = "required-space-kb";
		private const string AVAILABLE_SPACE = "available-space-kb";


		public DiskSpaceHealthCheck(long treshold = 100000000, SpaceUnit unit = SpaceUnit.Byte)
		{
			_defaultTreshold = treshold * (long)unit;

			_data.Add(REQUIRED_SPACE, _defaultTreshold);
		}

		public override HealthCheckResponse CheckResponse()
		{
			try
			{
				DriveInfo driveInfo = new DriveInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
				long usableSpace = driveInfo.AvailableFreeSpace;

				if (!_data.ContainsKey(AVAILABLE_SPACE))
					_data.Add(AVAILABLE_SPACE, usableSpace);
				else
					_data[AVAILABLE_SPACE] = usableSpace;

				response.Data = _data;
				

				if (usableSpace >= _defaultTreshold)
				{
					response.Up();
					return response;
				}
				else
				{
					response.Down();
					return response;
				}
			}
			catch
			{
				response.Down();
				return response;
			}
		}

	}

	public enum SpaceUnit : long
	{
		Byte = 1,
		Kilobyte = 1000,
		Megabyte = 1000000,
		Gigabyte = 1000000000,
		Terabyte = 1000000000000
	}
}
