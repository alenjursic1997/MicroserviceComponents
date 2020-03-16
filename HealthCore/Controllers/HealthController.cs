//using HealthCore.Models;
//using HealthCore.Services;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace HealthCore.Controllers
//{

//	public class HealthController : Controller
//	{
//		private readonly IHealth _healthCheckRegistry;

//		public HealthController(IHealth healthCheckRegistry)
//		{
//			_healthCheckRegistry = healthCheckRegistry;
//		}

//		private IActionResult GetHealthByType(HealthType type)
//		{
//			if (!_healthCheckRegistry.IsEnabled())
//			{
//				return NotFound();
//			}

//			try
//			{
//				// get results
//				var results = _healthCheckRegistry.GetResults(type);

//				// prepare response
//				HealthResponse healthResponse = new HealthResponse();
//				healthResponse.Checks = results;
//				healthResponse.Status = State.UP;

//				// check if any check is down
//				if (results.Where(e => State.DOWN.Equals(e.Status)).Any())
//					healthResponse.Status = State.DOWN;

//				if (healthResponse.Status == State.DOWN)
//					return StatusCode(statusCode: 503, value: healthResponse);

//				return Ok(healthResponse);
//			}
//			catch
//			{
//				return StatusCode(500);
//			}
//		}


//		public IActionResult GetHealth()
//		{
//			return GetHealthByType(HealthType.Any);
//		}

//		public IActionResult GetHealthLiveness()
//		{
//			return GetHealthByType(HealthType.Liveness);
//		}

//		public IActionResult GetHealthReadiness()
//		{
//			return GetHealthByType(HealthType.Readiness);
//		}
//	}
//}
