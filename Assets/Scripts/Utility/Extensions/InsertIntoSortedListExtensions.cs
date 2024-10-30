using System;
using System.Collections.Generic;

namespace Scripts.Utility.Extensions
{
    public static class InsertIntoSortedListExtensions
    {
        public static int InsertIntoSortedList<T>(this List<T> list, T item) where T : IComparable<T>
        {
            return InsertIntoSortedList<T>(list, item, Comparer<T>.Default);
        }

        private static int InsertIntoSortedList<T>(List<T> list, T item, Comparer<T> comparator) where T : IComparable<T>
        {
            int startIndex = 0;
            int endIndex = list.Count;

            while (endIndex > startIndex)
            {
                int middleIndex = startIndex + ((endIndex - startIndex) / 2);
                T middleItem = list[middleIndex];
                int comparerResult = comparator.Compare(middleItem, item);

                if (comparerResult < 0)
                    startIndex = middleIndex + 1;
                else
                    endIndex = middleIndex;
            }

            list.Insert(startIndex, item);
            return startIndex;
        }
    }
}
