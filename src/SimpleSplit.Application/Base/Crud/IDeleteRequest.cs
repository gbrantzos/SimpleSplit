namespace SimpleSplit.Application.Base.Crud
{
    public interface IDeleteRequest
    {
        long ID { get; set; }
        int RowVersion { get; set; }
    }
}