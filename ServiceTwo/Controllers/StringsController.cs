using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ServiceTwo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StringsController : ControllerBase
    {
        private readonly ILogger<StringsController> _logger;

        public StringsController(ILogger<StringsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("hello")]
        public string GetHello()
        {
            return "Hello Service Two!";
        }
    }
}
