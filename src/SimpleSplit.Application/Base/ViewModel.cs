using System.Text.Json.Serialization;

namespace SimpleSplit.Application.Base
{
    public abstract class ViewModel
    {
        /// <summary>
        /// Entity ID
        /// </summary>
        [JsonPropertyOrder(-9)]
        public long ID { get; set; }

        /// <summary>
        /// Row version (optimistic concurrency support)
        /// </summary>
        [JsonPropertyOrder(-9)]
        public int RowVersion { get; set; }
    }
}
