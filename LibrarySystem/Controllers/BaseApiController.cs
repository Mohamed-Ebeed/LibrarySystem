using LibrarySystem.Results;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
       
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return Ok(result);

            return result.ErrorType == ErrorType.NotFound
                ? NotFound(result)
                : BadRequest(result);
        }
    }
}