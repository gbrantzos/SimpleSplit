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

        public async Task<Image> GetImage(Type entityType, long entityID, CancellationToken token)
        {
            return await _dbContext
                .Set<Image>()
                .FirstOrDefaultAsync(i => i.Type == KnownTypes[entityType] && i.ID == entityID, token);
        }

        public async Task SaveImage(Type entityType, long entityID, string filename, byte[] content,
            CancellationToken token)
        {
            var image = await _dbContext
                            .Set<Image>()
                            .FirstOrDefaultAsync(i => i.Type == KnownTypes[entityType] && i.ID == entityID, token)
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
    }
}