using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ServiceOne.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NumbersController : ControllerBase
    {
        private readonly ILogger<NumbersController> _logger;

        public NumbersController(ILogger<NumbersController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("random")]
        public int GetRandom()
        {
            return (new Random()).Next(0, 10000);
        }

        [HttpGet]
        [Route("max")]
        public int GetMax()
        {
            return int.MaxValue;
        }
    }
}
