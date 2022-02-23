using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SimpleSplit.Application.Features.Common;

namespace SimpleSplit.WebApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class ImagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ImagesController(IMediator mediator) 
            => _mediator   = mediator;

        [HttpGet("{entityType:alpha}/{entityID:long}"), AllowAnonymous]
        public async Task<IActionResult> GetImage(string entityType, long entityID)
        {
            var result = await _mediator.Send(new GetImage
            {
                EntityType = entityType,
                EntityID   = entityID
            });
            var image = result.Value;
            if (image == null)
                return BadRequest();

            var contentType = GetMimeTypeForFileExtension(image.Filename);
            return File(image.Content, contentType, image.Filename);
        }

        private static string GetMimeTypeForFileExtension(string filePath)
        {
            const string defaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();
            return !provider.TryGetContentType(filePath, out var contentType)
                ? defaultContentType
                : contentType;
        }
    }
}