namespace SimpleSplit.Domain.Features.Common
{
    public interface IImageRepository
    {
        /// <summary>
        /// Get image for requested entity
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityID">Entity ID (as long)</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task<Image> GetImage(Type entityType, long entityID, CancellationToken token = default);
        
        /// <summary>
        /// Save image fro requested entity (insert or update)
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="entityID"></param>
        /// <param name="filename"></param>
        /// <param name="content"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SaveImage(Type entityType, long entityID, string filename, byte[] content, CancellationToken token = default);
    }

    public static class ImageRepositoryExtensions
    {
        public static Task SaveImage<TEntity>(this IImageRepository imageRepository, 
            long entityID,
            string filename,
            byte[] content,
            CancellationToken token = default)
            => imageRepository.SaveImage(typeof(TEntity), entityID, filename, content, token);
    }
}