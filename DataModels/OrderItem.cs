using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DataModels
{
    public class OrderItem : INotifyPropertyChanged
    {
        private int _onBackOrder;
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
        public int OrderHeaderId { get; set; }
        public int StockItemId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
