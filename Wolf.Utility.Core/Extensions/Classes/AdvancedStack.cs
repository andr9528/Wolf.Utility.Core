using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Core.Extensions.Classes
{
    public class AdvancedStack<T> : Stack<T>
    {
        public delegate void PushedDelegate(T item, EventArgs args);
        public event PushedDelegate Pushed;

        public delegate void PoppedDelegate(T poppedItem, T peekedItem, EventArgs args);
        public event PoppedDelegate Popped;

        public delegate void PeekedDelegate(T item, EventArgs args);
        public event PeekedDelegate Peeked;

        public new T Push(T item)
        {
            base.Push(item);
            Pushed?.Invoke(item, EventArgs.Empty);
            return item;
        }

        public new T Pop()
        {
            var item = base.Pop();
            Popped?.Invoke(item, base.Peek(),EventArgs.Empty);
            return item;
        }

        public new T Peek()
        {
            var item = base.Peek();
            Peeked?.Invoke(item, EventArgs.Empty);
            return item;
        }
    }
}
