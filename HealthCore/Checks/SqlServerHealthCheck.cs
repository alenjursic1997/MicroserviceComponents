using System;
using System.Collections.Generic;
using System.Text;
using HealthCore.Models;
using System.Data.SqlClient;

namespace HealthCore.Checks
{
	public class SqlServerHealthCheck : HealthCheck
	{

		private readonly string _connectionString;
		private HealthCheckResponse response = new HealthCheckResponse();
		private Dictionary<string, object> _data = new Dictionary<string, object>();


		public SqlServerHealthCheck(string connectionString)
		{
			_connectionString = connectionString;
		}

		public override HealthCheckResponse CheckResponse()
		{
			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionString);
					_data.Add("server", builder.DataSource);
					_data.Add("database", builder.InitialCatalog);
					_data.Add("integrated-security", builder.IntegratedSecurity);
					response.Data = _data;

					if (connection != null && connection.State == System.Data.ConnectionState.Open)
					{
						connection.Close();
						response.Up();
						return response;
					}
					response.Down();
					return response;
				}
			}
			catch (Exception ex)
			{
				response.Down();
				return response;
			}
		}
	}
}
