using System;

namespace OrderMgmtSystem.CommonEventArgs
{
    public class OrderItemRemovedEventArgs : EventArgs
    {
        public int StockItemId { get; set; }
        public int Quantity { get; set; }
        public int OnBackOrder { get; set; }
    }
}
