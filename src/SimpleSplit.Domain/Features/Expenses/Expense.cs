using SimpleSplit.Domain.Base;

namespace SimpleSplit.Domain.Features.Expenses
{
    public class ExpenseID : EntityID
    {
        public ExpenseID(long id) : base(id) { }
        public ExpenseID() : base(0) { }
    }

    public class Expense : Entity<ExpenseID>
    {
        public override ExpenseID ID { get; set; } = new ExpenseID();

        public string Description { get; set; }
        public DateTime EnteredAt { get; set; }
        public Money Amount { get; set; }
        public Category Category { get; set; }
        public bool IsOwnerCharge { get; set; }
        public DateTime? SharedAt { get; set; }
    }
}
