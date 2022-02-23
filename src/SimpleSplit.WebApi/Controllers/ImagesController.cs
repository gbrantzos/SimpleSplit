using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SimpleSplit.Domain.Features.Common;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.WebApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class ImagesController : ControllerBase
    {
        private static readonly Dictionary<string, Type> KnownTypes =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { nameof(User), typeof(User) }
            };

        private readonly IImageRepository _imageRepository;

        public ImagesController(IImageRepository imageRepository)
            => _imageRepository = imageRepository;

        [HttpGet("{entityType:alpha}/{entityID:long}")]
        public async Task<IActionResult> GetImage(string entityType, long entityID)
        {
            var image = await _imageRepository.GetImage(KnownTypes[entityType], entityID);
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