using System;
using System.Collections.Generic;
using System.Text;
using Wolf.Utility.Core.Persistence.Core;

namespace Wolf.Utility.Core.Exceptions
{
    public class IncorrectEntityCountException<T> : BaseException where T : class, IEntity
    {
        public static IncorrectEntityCountException<T> Constructor(int expectedCount, int actualCount,
            bool perfectMatch = false, IEnumerable<T> elements = null)
        {
            bool toMany = false;
            switch (perfectMatch)
            {
                case true:
                    if (expectedCount < actualCount) toMany = true;
                    return new IncorrectEntityCountException<T>(
                            $"Expected to find exactly {expectedCount}, but actually found {actualCount} of the entity type {typeof(T).Name}")
                    { ActualCount = actualCount, ExpectedCount = expectedCount, ToMany = toMany, Elements = elements };
                case false:
                    return new IncorrectEntityCountException<T>(
                            $"Expected to find at least {expectedCount}, but actually found {actualCount} of the entity type {typeof(T).Name}")
                    { ActualCount = actualCount, ExpectedCount = expectedCount, Elements = elements };
                default: return null;
            }
        }

        public IEnumerable<T> Elements { get; private set; }
        public int ExpectedCount { get; private set; }
        public int ActualCount { get; private set; }
        public bool ToMany { get; private set; }
        public bool ToFew
        {
            get
            {
                return !ToMany;
            }
        }


        protected IncorrectEntityCountException(string message) : base(message)
        {

        }
    }
}
