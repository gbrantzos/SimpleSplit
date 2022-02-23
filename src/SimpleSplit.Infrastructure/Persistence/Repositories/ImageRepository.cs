using Microsoft.EntityFrameworkCore;
using SimpleSplit.Domain.Features.Common;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Infrastructure.Persistence.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private static readonly Dictionary<Type, Image.EntityType> KnownTypes = new Dictionary<Type, Image.EntityType>
        {
            { typeof(User), Image.EntityType.User }
        };

        private readonly SimpleSplitDbContext _dbContext;

        public ImageRepository(SimpleSplitDbContext dbContext) => _dbContext = dbContext;

        public async Task<Image> GetImage(Type entityType, long entityID, CancellationToken cancellationToken)
            => await GetImageAsync(entityType, entityID, cancellationToken);

        public async Task SaveImage(Type entityType, long entityID, string filename, byte[] content,
            CancellationToken cancellationToken)
        {
            var image = await GetImageAsync(entityType, entityID, cancellationToken)
                        ?? new Image
                        {
                            Type     = Image.EntityType.User,
                            EntityID = entityID
                        };
            image.Content  = content;
            image.Filename = filename;
            if (image.ID == 0)
                _dbContext.Set<Image>().Add(image);
        }

        public async Task DeleteImage(Type entityType, long entityID, CancellationToken cancellationToken = default)
        {
            var image = await GetImageAsync(entityType, entityID, cancellationToken);
            if (image == null)
                return;
            _dbContext.Set<Image>().Remove(image);
        }

        private Task<Image> GetImageAsync(Type entityType, long entityID, CancellationToken cancellationToken)
            => _dbContext
                .Set<Image>()
                .FirstOrDefaultAsync(i => i.Type == KnownTypes[entityType] && i.EntityID == entityID,
                    cancellationToken);
    }
}