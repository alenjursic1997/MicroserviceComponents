using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscoveryCore.discovery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroserviceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [HttpGet("unregister")]
        public IActionResult UnregisterService()
        {
            DiscoveryProvider.GetDiscovery().UnregisterService();
            return Ok();
        }
    }
}