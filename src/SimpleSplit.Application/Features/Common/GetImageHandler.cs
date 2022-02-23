using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Features.Common;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Application.Features.Common
{
    public class GetImageHandler : Handler<GetImage, Image>
    {
        private static readonly Dictionary<string, Type> KnownTypes =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { nameof(User), typeof(User) }
            };

        private readonly IMemoryCache _cache;
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<GetImageHandler> _logger;

        public GetImageHandler(IMemoryCache memoryCache, 
            IImageRepository imageRepository,
            ILogger<GetImageHandler> logger) : base(logger)
        {
            _cache           = memoryCache;
            _imageRepository = imageRepository;
            _logger     = logger;
        }

        protected override async Task<Image> HandleCore(GetImage request, CancellationToken cancellationToken)
        {
            var cacheKey = $"IMG|{request.EntityType}|{request.EntityID}";
            if (_cache.TryGetValue(cacheKey, out var cachedImage))
            {
                _logger.LogDebug("Cache hit {EntityType}:{EntityID}", request.EntityType, request.EntityID);
                return (Image)cachedImage;
            }

            var image = await _imageRepository.GetImage(KnownTypes[request.EntityType], request.EntityID,
                cancellationToken);
            _cache.Set(cacheKey, image, TimeSpan.FromMinutes(1));

            return image;
        }
    }
}