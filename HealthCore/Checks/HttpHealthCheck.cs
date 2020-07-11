using HealthCore.Attributes;
using HealthCore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace HealthCore.Checks
{
	[Liveness]
	public class HttpHealthCheck : HealthCheck
	{
		private List<string> _urls;
		private HealthCheckResponse response = new HealthCheckResponse();
		private Dictionary<string, State> _data = new Dictionary<string, State>();

		public HttpHealthCheck(params string[] urls)
		{
			if (urls == null)
				_urls = null;
			else
				_urls = urls.ToList();
		}

		public override HealthCheckResponse CheckResponse()
		{
			_data.Clear();

			if (_urls == null)
				return null;

			response.Up();

			foreach (string url in _urls)
				checkHttpStatus(ref response, url);

			response.Data = _data;
			return response;
		}

		private void checkHttpStatus(ref HealthCheckResponse response, string url)
		{
			HttpWebRequest myRequest;
			HttpWebResponse myResponse = null;

			try
			{
				myRequest = (HttpWebRequest)WebRequest.Create(url);
				myResponse = (HttpWebResponse)myRequest.GetResponse();
				if ((int)myResponse.StatusCode >= 200 && (int)myResponse.StatusCode < 300)
					_data.Add(url, State.UP);
				else
				{
					response.Down();
					_data.Add(url, State.DOWN);
				}
			}
			catch
			{
				response.Down();
				_data.Add(url, State.DOWN);
			}
			finally
			{
				if (myResponse != null)
					myResponse.Close();
			}
		}
	}
}
