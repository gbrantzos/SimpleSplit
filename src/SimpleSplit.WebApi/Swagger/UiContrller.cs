using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SimpleSplit.Common;

namespace SimpleSplit.WebApi.Swagger
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("swagger/ui")]
    public class UiController : ControllerBase
    {
        private static readonly HashSet<string> Themes = new HashSet<string>
        {
            "feeling-blue",
            "flattop",
            "material",
            "monokai",
            "muted",
            "outline",
            "dark"
        };
        private const string ResourceRoot = "SimpleSplit.WebApi.Swagger.Themes";
        private static readonly Assembly Assembly = typeof(UiController).Assembly;

        [HttpGet("css")]
        public IActionResult GetTheme([FromQuery] string themeName = "flattop")
        {
            var css = Themes.Contains(themeName)
                ? EmbeddedResourceManager.GetText(Assembly, ResourceRoot, $"theme-{themeName}.css")
                : string.Empty;
            return Content(css, "text/css");
        }
    }
}
