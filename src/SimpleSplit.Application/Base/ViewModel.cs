using System.Text.Json.Serialization;
using SimpleSplit.Application.Features.Expenses;
using SimpleSplit.Application.Features.Security;
using SimpleSplit.Domain.Features.Expenses;
using SimpleSplit.Domain.Features.Security;

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

        [JsonIgnore]
        public bool IsNew => ID == 0 && RowVersion == 0;

        public static TViewModel FromDomainObject<TEntity, TViewModel>(TEntity entity) where TViewModel : ViewModel
        {
            return entity switch
            {
                Expense exp => exp.ToViewModel() as TViewModel,
                Category ctg => ctg.ToViewModel() as TViewModel,
                User usr => usr.ToViewModel() as TViewModel,
                _ => throw new Exception($"Unknown mapping from {typeof(TEntity).Name}")
            };
        }
    }
}
