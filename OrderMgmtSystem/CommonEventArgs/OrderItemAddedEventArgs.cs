using DataModels;
using System;

namespace OrderMgmtSystem.CommonEventArgs
{
    /// <summary>
    /// Provides the relevant event data to handle adding new or existing items to an order.
    /// </summary>
    public class OrderItemAddedEventArgs : EventArgs
    {
        public OrderItem Item { get; set; }
        public bool OrderItemExists { get; set; }
    }
}
