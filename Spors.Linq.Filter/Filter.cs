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
        /// <summary>
        /// Returns the count of active filters.
        /// </summary>
        public int Count
        {
            get => _activeFilters.Count;
        }

        /// <summary>
        /// List that holds a filter by key and group.
        /// </summary>
        private readonly List<Tuple<string, string, Expression<Func<T, bool>>>> _activeFilters;

        /// <summary>
        /// Constructor
        /// </summary>
        public Filter()
        {
            _activeFilters = new List<Tuple<string, string, Expression<Func<T, bool>>>>();
        }

        /// <summary>
        /// Adds a filter.
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(Predicate<T> filter, string key = "", string group = "")
        {
            if (filter != null)
            {
                Func<T, bool> f = new Func<T, bool>(filter);
                Expression<Func<T, bool>> expression = x => f(x);

                if (string.IsNullOrEmpty(key))
                    key = StringHelper.RandomString(6);
                if (string.IsNullOrEmpty(group))
                    group = StringHelper.RandomString(6);

                _activeFilters.Add(Tuple.Create(key, group, expression));
            }
        }

        /// <summary>
        /// Removes an active filter by key.
        /// </summary>
        /// <param name="key">Key that represents the value.</param>
        public bool Remove(string key)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(key))
            {
                int filtersRemoved = _activeFilters.RemoveAll(x => x.Item1 == key);

                if (filtersRemoved > 0)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Removes an active filter by group.
        /// </summary>
        /// <param name="key">Key that represents the value.</param>
        public bool RemoveByGroup(string group)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(group))
            {
                int filtersRemoved = _activeFilters.RemoveAll(x => x.Item2 == group);

                if (filtersRemoved > 0)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Removes all active filters.
        /// </summary>
        public void Reset()
            => _activeFilters.Clear();

        /// <summary>
        /// Applies all filters to the given <see cref="Enumerable"/>.
        /// </summary>
        /// <param name="currentList">The list on which all filters should be applied.</param>
        /// <returns>A filtered list</returns>
        public IEnumerable<T> ApplyFilters(IEnumerable<T> currentList)
        {
            Dictionary<string, Expression<Func<T, bool>>> groupedFilter = new Dictionary<string, Expression<Func<T, bool>>>();
            foreach (var filter in _activeFilters)
            {
                if(groupedFilter.TryGetValue(filter.Item2, out Expression<Func<T, bool>> currentExpression))
                {
                    var parameter = Expression.Parameter(typeof(T));
                    Expression<Func<T, bool>> expression = (Expression<Func<T, bool>>)
                        Expression.Lambda(
                            Expression.OrElse(
                                Expression.Invoke(currentExpression, parameter),
                                Expression.Invoke(filter.Item3, parameter)
                            ), parameter);

                    groupedFilter[filter.Item2] = expression;
                }
                else
                {
                    groupedFilter.Add(filter.Item2, filter.Item3);
                }
            }

            Expression<Func<T, bool>> combinedExpression = null;
            foreach (var filter in groupedFilter)
            {
                if(combinedExpression != null)
                {
                    var parameter = Expression.Parameter(typeof(T));
                    Expression<Func<T, bool>> expression = (Expression<Func<T, bool>>)
                        Expression.Lambda(
                            Expression.AndAlso(
                                Expression.Invoke(combinedExpression, parameter),
                                Expression.Invoke(filter.Value, parameter)
                            ), parameter);

                    combinedExpression = expression;
                }
                else
                {
                    combinedExpression = filter.Value;
                }
            }

            if(currentList.Count() > 0)
            {
                Func<T, bool> predicate = combinedExpression.Compile();
                return currentList.Where(predicate);
            }

            return Enumerable.Empty<T>();
        }
    }
}
