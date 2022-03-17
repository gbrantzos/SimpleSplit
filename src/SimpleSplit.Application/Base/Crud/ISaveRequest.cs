namespace SimpleSplit.Application.Base.Crud
{
    /// <summary>
    /// The basic information to perform a "save" operation
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public interface ISaveRequest<out TViewModel> where TViewModel : ViewModel
    {
        /// <summary>
        /// The model received from presentation layer
        /// </summary>
        TViewModel Model { get; }
    }
}