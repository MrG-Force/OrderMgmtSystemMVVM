using System;
using System.ComponentModel;

namespace DataModels
{
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
                    OnBackOrder = Math.Abs(_inStock);
                    _inStock = 0;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("InStock"));
            }
        }
        public int OnBackOrder { get; set; } = 0;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
