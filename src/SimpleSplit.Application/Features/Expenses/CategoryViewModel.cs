using System.ComponentModel;
using Mapster;
using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    [DisplayName("Category")]
    public class CategoryViewModel : ViewModel
    {
        /// <summary>
        /// Category description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Category kind
        /// </summary>
        public int Kind { get; set; }
    }

    public static class CategoryViewModelExtensions
    {
        public static CategoryViewModel ToViewModel(this Category domainObject)
        {
            return new CategoryViewModel
            {
                ID          = domainObject.ID.Value,
                RowVersion  = domainObject.RowVersion,
                Description = domainObject.Description,
                Kind        = (int)domainObject.Kind,
            };
        }
    }

    public class CategoryViewModelMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Category, CategoryViewModel>()
                .Map(dest => dest.ID, src => src.ID.Value)
                .Map(dest => dest.Kind, src => (int)src.Kind);
        }
    }
}