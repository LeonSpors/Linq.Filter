using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Spors.Linq.Filter.Tests
{
    [TestClass]
    public class FilterTest
    {
        [TestMethod]
        public void AddFilter_AnonymousFilter_FilterCountEqualsOne()
        {
            Filter<int> filter = new();

            Predicate<int> predicate = val => val > 0;
            filter.AddFilter(predicate);

            Assert.AreEqual(filter.Count, 1, "Predicate has not been added.");
        }
    }
}
