using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DataModels
{
    /// <summary>
    /// Defines the OrderItem object. 
    /// </summary>
    /// <remarks>
    /// An OrderItem is a StockItem assigned to an Order with a number(Quantity) of products.
    /// </remarks>
    public class OrderItem : INotifyPropertyChanged
    {
        #region Fields
        private int _onBackOrder;
        private int _quantity;
        #endregion

        #region Constructors
        public OrderItem()
        { }
        public OrderItem(int orderId, int itemId)
        {
            OrderHeaderId = orderId;
            StockItemId = itemId;
        }
        public OrderItem(int orderId, StockItem stockItem)
        {
            OrderHeaderId = orderId;
            StockItemId = stockItem.Id;
            Price = stockItem.Price;
            Description = stockItem.Name;
            Quantity = 0;
            _onBackOrder = 0;
        }
        public OrderItem(StockItem stockItem)
        {
            StockItemId = stockItem.Id;
            Price = stockItem.Price;
            Description = stockItem.Name;
            Quantity = 0;
            _onBackOrder = 0;
        }

        /// <summary>
        /// To create a temporary orderItem while editing an existing order.
        /// </summary>
        /// <param name="item"></param>
        public OrderItem(OrderItem item, int id = 0)
        {
            OrderHeaderId = id;
            StockItemId = item.StockItemId;
            Description = item.Description;
            Price = item.Price;
            Quantity = item.Quantity;
            OnBackOrder = item.OnBackOrder;
        }
        #endregion

        #region Props
        public int OrderHeaderId { get; set; }
        public int StockItemId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Total));
            }
        }
        public int OnBackOrder
        {
            get => _onBackOrder;
            set
            {
                _onBackOrder = value;
                RaisePropertyChanged(nameof(HasItemsOnBackOrder));
            }
        }
        public bool HasItemsOnBackOrder => OnBackOrder > 0;
        public decimal Total => Price * Quantity;
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Raises the PropertyChanged event when the property value has changed.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
