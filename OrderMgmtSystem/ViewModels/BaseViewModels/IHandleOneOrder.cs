using DataModels;

namespace OrderMgmtSystem.ViewModels.BaseViewModels
{
    /// <summary>
    /// An interface for ViewModels that deal with a single order.
    /// </summary>
    public interface IHandleOneOrder
    {
        Order Order { get; set; }
    }
}
