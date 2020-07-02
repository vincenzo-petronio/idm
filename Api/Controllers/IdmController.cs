using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Api.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class IdmController : ControllerBase
    {
        [HttpGet]
        [Route("claims")] // idm/claims
        public IActionResult GetClaims()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

    }
}
