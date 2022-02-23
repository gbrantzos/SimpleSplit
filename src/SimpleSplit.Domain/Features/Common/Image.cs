namespace SimpleSplit.Domain.Features.Common
{
    public class Image
    {
        public enum EntityType
        {
            User
        }
        
        public int ID { get; set; }
        public string Filename { get; set; }
        public EntityType Type { get; set; }
        public long EntityID { get; set; }
        public byte[] Content { get; set; }
    }
}