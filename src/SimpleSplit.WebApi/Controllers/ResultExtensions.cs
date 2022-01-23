using Microsoft.AspNetCore.Mvc;
using SimpleSplit.Application.Base;

namespace SimpleSplit.WebApi.Controllers
{
    public static class ResultExtensions
    {
        public static ActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.HasException)
                return new ObjectResult(result.AllErrors())
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            if (result.HasErrors)
                return new BadRequestObjectResult(result.AllErrors());
            return new OkObjectResult(result.Value);
        }
        
        public static ActionResult ToActionResult(this Result result)
        {
            if (result.HasException)
                return new ObjectResult(result.AllErrors())
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            if (result.HasErrors)
                return new BadRequestObjectResult(result.AllErrors());
            return new OkResult();
        }
    }
}