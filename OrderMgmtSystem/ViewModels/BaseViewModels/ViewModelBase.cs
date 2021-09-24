using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OrderMgmtSystem.ViewModels.BaseViewModels
{
    /// <summary>
    /// This class implements INotifyPropertyChanged to support one-way and two-way bindings
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// This method is intended to be called from the 'set' block of each bindable property
        /// to set the value of that property and notify the change if the change actually happened.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member">The member of the class(field) passed as reference</param>
        /// <param name="val">The value to be set</param>
        /// <param name="propertyName">Name of the caller property</param>
        protected virtual void SetProperty<T>(ref T member, T val,
            [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(member, val)) return;
            member = val;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Calling member's name will be used as the parameter.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
