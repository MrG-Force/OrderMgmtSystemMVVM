using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DataModels
{
    /// <summary>
    /// A class that represents an order in the system.
    /// </summary>
    public class Order : INotifyPropertyChanged
    {
        #region Fields
        private List<OrderItem> _orderItems;
        private static int _id = 1000;
        private int _orderStateId;
        private DateTime _dateTime;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the Order with the creation DateTime and sets the OrderState to New.
        /// </summary>
        public Order()
        {
            _orderItems = new List<OrderItem>();
            _dateTime = DateTime.Now;
            OrderStateId = 1;
            Id = _id++;
        }
        /// <summary>
        /// Production constructor, gets the Id from the database query.
        /// </summary>
        /// <param name="id"></param>
        public Order(int id) // TODO: this ctor needs to pass the date time from db, conflicted with next ctor
        {
            _orderItems = new List<OrderItem>();
            _dateTime = DateTime.Now;
            OrderStateId = 1;
            Id = id;
            _id = id++; // This line to be deleted when dummy data is no longer needed.
        }

        public Order(int id, DateTime dateTime, int stateId)
        {
            Id = id;
            _dateTime = dateTime;
            _orderStateId = stateId;
            _orderItems = new List<OrderItem>();
        }

        /// <summary>
        /// This constructor is used to create a dummy order with a passed random
        /// date and a random state.
        /// </summary>
        /// <param name="StateId"></param>
        /// <param name="randomDate"></param>
        public Order(int StateId, DateTime randomDate)
        {
            _orderItems = new List<OrderItem>();
            _dateTime = randomDate;
            OrderStateId = StateId;
            Id = _id++;
        }

        public Order(Order order)
        {
            Id = order.Id;
            _dateTime = DateTime.Now;
            OrderStateId = order.OrderStateId;
            OrderItems = new List<OrderItem>(order.OrderItems);
            HasItemsOnBackOrder = order.HasItemsOnBackOrder;
        }
        #endregion

        #region Props
        public int Id { get; set; }
        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                _dateTime = value;
                RaisePropertyChanged();
            }
        }

        public int OrderStateId
        {
            get => _orderStateId;
            set
            {
                _orderStateId = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(OrderStatus));
            }
        }

        public string OrderStatus => Enum.GetName(typeof(OrderState), OrderStateId);

        //--- enum ---
        public enum OrderState { New = 1, Pending = 2, Rejected = 3, Complete = 4 };

        /// <summary>
        /// The sum of each item total in this order.
        /// </summary>
        public decimal Total
        {
            get
            {
                return _orderItems.Sum(x => x.Total);
            }
        }
     
        public List<OrderItem> OrderItems
        {
            get
            {
                return _orderItems;
            }
            set
            {
                _orderItems = value;
                RaisePropertyChanged(nameof(Total));
                RaisePropertyChanged(nameof(ItemsCount));
                RaisePropertyChanged(nameof(DateTime));
                RaisePropertyChanged(nameof(HasItemsOnBackOrder));
            }
        }

        public int ItemsCount => _orderItems.Count;

        public bool HasItemsOnBackOrder { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a new OrderItem to the Order
        /// </summary>
        /// <param name="item">An OrderItem to be added</param>
        public void AddItem(OrderItem item)
        {
            if (item.HasItemsOnBackOrder)
            {
                HasItemsOnBackOrder = true;
                RaisePropertyChanged(nameof(HasItemsOnBackOrder));
            }
            OrderItems.Add(item);
            RaisePropertyChanged(nameof(Total));
        }

        /// <summary>
        /// Removes the item with the given id from the order.
        /// </summary>
        /// <remarks>Note that this method removes the item regardless of the Quantity of the item</remarks>
        /// <param name="id">This is usually the SKU of the item</param>
        public void RemoveItem(int id)
        {
            OrderItem item = _orderItems.Find(i => i.StockItemId.Equals(id));
            _orderItems.Remove(item);
            item = _orderItems.Find(i => i.OnBackOrder > 0);
            HasItemsOnBackOrder = item != null;
            RaisePropertyChanged(nameof(Total));
            RaisePropertyChanged(nameof(HasItemsOnBackOrder));
        }

        /// <summary>
        /// When the last order was cancelled before submit the Id goes back one position.
        /// </summary>
        public void CancelLastOrder()
        {
            _id--;
        }

        /// <summary>
        /// Commits the order and saves it to the database.
        /// </summary>
        public void Submit()
        {
            OrderStateId = 2;
        }

        /// <summary>
        /// Raises the PropertyChanged event when the property value has changed.
        /// </summary>
        /// <param name="propertyName"></param>
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
