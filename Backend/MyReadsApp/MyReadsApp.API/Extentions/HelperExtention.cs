using Microsoft.AspNetCore.Mvc;

namespace MyReadsApp.API.Extentions
{
    public static class HelperExtention
    {

        public static IActionResult ActionResult<T> (this ControllerBase controller, int statusCode, T result)
        {
            return statusCode switch
            {
                200 => controller.Ok(result),
                400 => controller.BadRequest(result),
                404 => controller.NotFound(result),
                _ => controller.StatusCode(statusCode, result)
            };
        }
    }
}
