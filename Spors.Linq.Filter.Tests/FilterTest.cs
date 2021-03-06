using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spors.Linq.Filter.Tests
{
    [TestClass]
    public class FilterTest
    {
        [TestMethod]
        public void AddFilter_AnonymousFilter_FilterCountEqualsOne()
        {
            Filter<int> filter = new Filter<int>();

            Predicate<int> predicate = val => val > 0;
            filter.AddFilter(predicate);

            Assert.AreEqual(filter.Count, 1);
        }

        [TestMethod]
        public void Remove_KeyExists_ReturnsTrue()
        {
            Filter<int> filter = new Filter<int>();

            Predicate<int> predicate = val => val > 0;
            filter.AddFilter(predicate, key: "test");
            bool result = filter.Remove("test");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Remove_KeyDoesNotExists_ReturnsFalse()
        {
            Filter<int> filter = new Filter<int>();

            Predicate<int> predicate = val => val > 0;
            filter.AddFilter(predicate, "test");
            bool result = filter.Remove("something different");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RemoveByGroup_GroupExists_ReturnsTrue()
        {
            Filter<int> filter = new Filter<int>();

            Predicate<int> predicate = val => val > 0;
            filter.AddFilter(predicate, group: "test");
            bool result = filter.RemoveByGroup("test");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RemoveByGroup_GroupDoesNotExists_ReturnsFalse()
        {
            Filter<int> filter = new Filter<int>();

            Predicate<int> predicate = val => val > 0;
            filter.AddFilter(predicate, group: "test");
            bool result = filter.RemoveByGroup("something different");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ApplyFilter_EmptyFilterList_ReturnsEmptyList()
        {
            Filter<int> filter = new Filter<int>();

            List<int> numbers = new List<int>() { 0, 1, 3, 4 };
            List<int> resultList = filter.ApplyFilters(numbers).ToList();

            Assert.IsTrue(resultList.Count == 0);
        }

        [TestMethod]
        public void ApplyFilter_OneFilter_ReturnListCountTwo()
        {
            Filter<int> filter = new Filter<int>();

            Predicate<int> predicate = val => val > 2;
            filter.AddFilter(predicate);

            List<int> numbers = new List<int>() { 0, 1, 3, 4 };
            List<int> resultList = filter.ApplyFilters(numbers).ToList();

            Assert.IsTrue(resultList.Count == 2);
        }

        [TestMethod]
        public void ApplyFilter_MultipleFilter_ReturnListCountTwo()
        {
            Filter<int> filter = new Filter<int>();

            Predicate<int> predicate = val => val > 10;
            Predicate<int> anotherPredicate = val => val == 17;

            filter.AddFilter(predicate);
            filter.AddFilter(anotherPredicate);

            List<int> numbers = new List<int>() { 0, 1, 3, 4, 9, 5, 17 };
            List<int> resultList = filter.ApplyFilters(numbers).ToList();

            Assert.IsTrue(resultList.Count == 1);
        }

        [TestMethod]
        public void ApplyFilter_OrElseFilter_ReturnListCountOne()
        {
            Filter<int> filter = new Filter<int>();

            Predicate<int> isGreaterThanSeven = val => val > 7;
            Predicate<int> isSmallerThanTen = val => val < 10;

            filter.AddFilter(isGreaterThanSeven);
            filter.AddFilter(isSmallerThanTen);

            List<int> numbers = new List<int>() { 0, 1, 3, 4, 9, 5, 17 };
            List<int> resultList = filter.ApplyFilters(numbers).ToList();

            Assert.IsTrue(resultList.Count == 1);
        }
    }
}
