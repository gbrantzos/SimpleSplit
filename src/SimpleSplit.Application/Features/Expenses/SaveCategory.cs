using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SaveCategory : Request<CategoryViewModel>
    {
        public CategoryViewModel Model { get; set; }
    }
}