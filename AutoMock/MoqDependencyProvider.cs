using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
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
            SetupMockGetters(mock);
            _cache.Add(typeof(T), mock);
            return mock;
        }

        private void SetupMockGetters<T>(Mock<T> mock) where T : class
        {
            var type = typeof(T);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.CanRead && !propertyInfo.CanWrite && propertyInfo.PropertyType.IsInterface)
                {
                    SetupMockPropertyGetter(type, propertyInfo, mock);
                }
            }
        }

        private void SetupMockPropertyGetter(Type type, PropertyInfo propertyInfo, Mock mock)
        {
            var methodTemplate = GetType().GetMethod("SetupDependency", BindingFlags.Instance | BindingFlags.NonPublic);
            var genericMethod = methodTemplate.MakeGenericMethod(new[] { type, propertyInfo.PropertyType });
            genericMethod.Invoke(this, new object[] { mock, propertyInfo });
        }

        // Resharper cannot see that this method is used becuase it is created using reflection
        // ReSharper disable UnusedMember.Local
        private void SetupDependency<TMock, TDependency>(Mock<TMock> mock, PropertyInfo propertyInfo) 
            where TMock : class 
            where TDependency : class
        {
            // Creates an expression for (x => x.Dependency) where Dependency is a read only 
            // property of type propertyInfo.PropertyType
            var argument = Expression.Parameter(typeof(TMock), "x");
            var getPropertyExpression = Expression.Property(argument, propertyInfo.Name);
            var expression = Expression.Lambda<Func<TMock, TDependency>>(getPropertyExpression, argument);

            var dependency = GetInstance<TDependency>();
            mock.Setup(expression).Returns(dependency);
        }
        // ReSharper restore UnusedMember.Local
    }
}
