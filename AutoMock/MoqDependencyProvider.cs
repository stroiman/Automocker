using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

namespace AutoMock
{
    /// <summary>
    /// A specialized <see cref="IDependencyProvider"/> that works by returning
    /// <see cref="Mock"/> objects.
    /// </summary>
    public class MoqDependencyProvider : IDependencyProvider
    {
        private readonly Dictionary<Type, Mock> _cache = new Dictionary<Type, Mock>();

        /// <summary>
        /// Resolves an instance of <typeparamref name="T"/>
        /// </summary>
        public T GetInstance<T>() where T : class
        {
            return GetMock<T>().Object;
        }

        /// <summary>
        /// Finds a previously created instance; or creates a new instance of
        /// <see cref="Mock{T}"/> with the specified type of <typeparamref name="T"/>
        /// </summary>
        public Mock<T> GetMock<T>() where T : class
        {
            Mock cachedMock;
            if (_cache.TryGetValue(typeof(T), out cachedMock))
                return (Mock<T>)cachedMock;
            var mock = new Mock<T>();
            _cache.Add(typeof(T), mock);
            return mock;
        }
    }
}
