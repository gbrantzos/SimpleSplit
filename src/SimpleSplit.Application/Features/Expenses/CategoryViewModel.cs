using System.ComponentModel;
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
}