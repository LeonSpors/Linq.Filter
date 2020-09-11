using Spors.Linq.Filter.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Spors.Linq
{
    /// <summary>
    /// This class helps to store and apply multiple filters on a <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">Input type of the <see cref="Enumerable"/></typeparam>
    public class Filter<T>
    {
        public int Count
        {
            get => _activeFilters.Count;
        }

        /// <summary>
        /// Actual list that holds a filter by key and group.
        /// </summary>
        private readonly List<Tuple<string, string, Expression<Func<T, bool>>>> _activeFilters;

        /// <summary>
        /// List of all combined 
        /// </summary>
        private readonly Dictionary<string, Expression<Func<T, bool>>> _combinedFilters;

        /// <summary>
        /// Constructor
        /// </summary>
        public Filter()
        {
            _activeFilters = new();
            _combinedFilters = new();
        }

        /// <summary>
        /// Adds a filter.
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(Predicate<T> filter, string key = "", string group = "")
        {
            if (filter != null)
            {
                Func<T, bool> f = new(filter);
                Expression<Func<T, bool>> expression = x => f(x);

                if (string.IsNullOrEmpty(key))
                    key = StringHelper.RandomString(6);
                if (string.IsNullOrEmpty(group))
                    key = StringHelper.RandomString(6);
                _activeFilters.Add(Tuple.Create(key, group, expression));
            }
        }

        //public void AddOrUpdateAndFilter(string key, Predicate<T> filter)
        //{
        //    var filterToBeUpdated = _activeFilters.FirstOrDefault(x => x.Item1 == key);
        //    if (filterToBeUpdated != null)
        //        filterToBeUpdated.Item2 = Predicate.And(filterToBeUpdated.Item2, filter);
        //    else
        //        AddFilter(filter, key);
        //}

        /// <summary>
        /// Removes an actual active filter.
        /// </summary>
        /// <param name="filter"><see cref="Predicate{T}"/></param>
        public bool Remove(Predicate<T> filter)
        {
            bool result = false;

            if (filter != null)
            {
                Func<T, bool> f = new(filter);
                Expression<Func<T, bool>> expression = x => f(x);
                int filtersRemoved = _activeFilters.RemoveAll(x => x.Item3 == expression);

                if (filtersRemoved > 0)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Removes an actual active filter by key.
        /// </summary>
        /// <param name="key">Key that represents the value.</param>
        public bool Remove(string key)
        {
            bool result = false;

            if (string.IsNullOrEmpty(key))
            {
                int filtersRemoved = _activeFilters.RemoveAll(x => x.Item1 == key);

                if (filtersRemoved > 0)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Removes an actual active filter by group.
        /// </summary>
        /// <param name="key">Key that represents the value.</param>
        public bool RemoveByGroup(string group)
        {
            bool result = false;

            if (string.IsNullOrEmpty(group))
            {
                int filtersRemoved = _activeFilters.RemoveAll(x => x.Item2 == group);

                if (filtersRemoved > 0)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Removes an active filter at index.
        /// </summary>
        /// <param name="index">List index of the filter tuple.</param>
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
            _combinedFilters.Clear();
            foreach (var filter in _activeFilters)
            {
                var currentExpression = _combinedFilters.FirstOrDefault(x => x.Key == filter.Item2);

                if ((currentExpression.Key, currentExpression.Value) != default)
                {
                    var p = Expression.Parameter(typeof(T));
                    Expression<Func<T, bool>> expression = (Expression<Func<T, bool>>)
                        Expression.Lambda(
                            Expression.OrElse(
                                Expression.Invoke(currentExpression.Value, p),
                                Expression.Invoke(filter.Item3, p)
                            )
                        , p);

                    currentExpression = new(currentExpression.Key, expression);
                }
                else
                    _combinedFilters.Add(filter.Item2, filter.Item3);
            }

            IEnumerable<T> temporaryList = Enumerable.Empty<T>();
            foreach (var filter in _combinedFilters)
            {
                Func<T, bool> predicate = filter.Value.Compile();
                var filterResult = currentList.Where(predicate);

                if (temporaryList.Count() > 0)
                    temporaryList = temporaryList.Intersect(filterResult);
                else
                    temporaryList = filterResult;
            }

            return temporaryList.ToList();
        }
    }
}
