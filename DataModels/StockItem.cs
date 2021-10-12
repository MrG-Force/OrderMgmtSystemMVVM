using System.ComponentModel;

namespace DataModels
{
    /// <summary>
    /// Defines the StockItem object.
    /// </summary>
    public class StockItem : INotifyPropertyChanged
    {
        private int _inStock;

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int InStock
        {
            get => _inStock;
            set
            {
                _inStock = value;
                if (_inStock < 0)
                {
                    _inStock = 0;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("InStock"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
