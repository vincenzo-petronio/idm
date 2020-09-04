using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        [Route("version")]
        [Authorize("OnlyItDomain")]
        public string GetVersion()
        {
            return "0.0.1";
        }
    }
}
