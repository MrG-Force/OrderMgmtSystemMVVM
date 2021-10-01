using System;

namespace OrderMgmtSystem.CommonEventArgs
{
    /// <summary>
    /// This class provides the relevant even information to handle an ItemRemovedEvent
    /// </summary>
    public class OrderItemRemovedEventArgs : EventArgs
    {
        public int OrderHeaderId { get; set; }
        public int StockItemId { get; set; }
        public int Quantity { get; set; }
        public int OnBackOrder { get; set; }
    }
}
