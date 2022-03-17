using SimpleSplit.Application.Base;
using SimpleSplit.Application.Base.Crud;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SaveCategory : Request<CategoryViewModel>, ISaveRequest<CategoryViewModel>
    {
        public CategoryViewModel Model { get; set; }
    }
}