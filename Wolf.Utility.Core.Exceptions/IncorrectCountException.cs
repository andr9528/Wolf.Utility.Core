﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Core.Exceptions
{
    public class IncorrectCountException<T> : BaseException
    {
        public static IncorrectCountException<T> Constructor(int expectedCount, int actualCount,
            bool perfectMatch = false, IEnumerable<T> elements = null)
        {
            bool toMany = false;
            switch (perfectMatch)
            {
                case true:
                    if (expectedCount < actualCount) toMany = true;
                    return new IncorrectCountException<T>(
                            $"Expected to find exactly {expectedCount}, but actually found {actualCount} of the entity type {typeof(T).Name}")
                    { ActualCount = actualCount, ExpectedCount = expectedCount, ToMany = toMany, Elements = elements };
                case false:
                    return new IncorrectCountException<T>(
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


        protected IncorrectCountException(string message) : base(message)
        {

        }
    }
}
