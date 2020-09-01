using System;
using System.Collections.Generic;
using System.Linq;

namespace Spors.Linq
{
    /// <summary>
    /// This class helps to store and apply multiple filters on a <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">Input type of the <see cref="Enumerable"/></typeparam>
    public class Filter<T>
    {
        private List<Predicate<T>> _activeFilters;

        /// <summary>
        /// Constructor
        /// </summary>
        public Filter()
        {
            _activeFilters = new List<Predicate<T>>();
        }

        /// <summary>
        /// Adds a filter.
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(Predicate<T> filter)
            => _activeFilters.Add(filter);

        /// <summary>
        /// Removes an actual active filter.
        /// </summary>
        /// <param name="filter"></param>
        public void Remove(Predicate<T> filter)
            => _activeFilters.Remove(filter);

        /// <summary>
        /// Removes an active filter at index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
            => _activeFilters.RemoveAt(index);

        /// <summary>
        /// Removes all active filters.
        /// </summary>
        public void Reset()
            => _activeFilters.Clear();

        /// <summary>
        /// Applies all filters to the given <see cref="Enumerable"/>.
        /// </summary>
        /// <param name="currentList">The list on which all filters should be applied.</param>
        /// <returns></returns>
        public IEnumerable<T> ApplyFilters(IEnumerable<T> currentList)
        {
            IEnumerable<T> temporaryList = Enumerable.Empty<T>();

            foreach (var filter in _activeFilters)
                temporaryList = Enumerable.Concat(temporaryList, Enumerable.Where(currentList, filter.Invoke));
            return temporaryList.ToList();
        }
    }
}
