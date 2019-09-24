using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{

    [ApiController]
    public class IdentityController : ControllerBase
    {
        // GET api/v1/[controller]
        [HttpGet]
        [Route("api/identity")]
        [Authorize]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> HasIdentity()
        {
            return Ok();
        }

        [HttpGet]
        [Route("api/identity/no")]
        public async Task<IActionResult> NoIdentity()
        {
            return Ok();
        }
    }
}
