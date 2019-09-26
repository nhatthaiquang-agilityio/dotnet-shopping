using dotnet_express_mapper.Models;
using Microsoft.AspNetCore.Mvc;
using ExpressMapper;

namespace dotnet_express_mapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : ControllerBase
    {

        [HttpGet("getauthor")]
        public ActionResult<Author> Get()
        {
            var entity = new AuthorDTO
            {
                Address = "Nui Thanh",
                FirstName = "Nhat",
                LastName = "Thai",
                Id = 1
            };
            return new OkObjectResult(Mapper.Map<AuthorDTO, Author>(entity));
        }
    }
}
