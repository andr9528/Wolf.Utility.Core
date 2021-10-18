using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Wolf.Utility.Core.Bases
{
    public abstract class BasePropertyChanged : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        /// <summary>
        /// Changes the value of the backing store to the input of value, if they are different that is.
        /// If changes happen, a PropertyChanged event is fired with the name of the property that changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingStore">a reference to the backing store of a property. i.e a reference to a field almost always.</param>
        /// <param name="value">The value that backingStore is going to be set too, if they are different.</param>
        /// <param name="propertyName">Name of the property that is being changed. Make use of nameof().</param>
        /// <param name="onChanged">Any additional action to take when the PropertyChanged event is going to be fired.</param>
        /// <returns>Returns False if the value in backingstore and value are equal, otherwise returns true after invoking PropertyChanged(s) events</returns>
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Causes a PropertyChanged event to be raised with the given <paramref name="propertyName"/>.
        /// Is called from <see cref="SetProperty{T}(ref T, T, string, Action)"/>, which should be used in 99% of cases where this one should be called/>
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
