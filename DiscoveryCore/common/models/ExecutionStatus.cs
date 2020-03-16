using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoveryCore.common.models
{
	public class ExecutionStatus
	{
		public bool Successful { get; set; }
		public string Message { get; set; }

		public ExecutionStatus()
		{

		}

		public ExecutionStatus(bool successful, string message)
		{
			Successful = successful;
			Message = message;
		}

		public static ExecutionStatus Bad(string message = null)
		{
			if (message == null)
				message = "Method execution failure"; 

			return new ExecutionStatus(false, message);
		}

		public static ExecutionStatus Good(string message = null)
		{
			if (message == null)
				message = "Method execution successful";

			return new ExecutionStatus(true, message);
		}
	}
}
