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
            return GetMock<T>(new Stack<Type>());
        }

        private Mock<T> GetMock<T>(Stack<Type> buildStack) where T : class
        {
            Mock cachedMock;
            var typeToBuild = typeof(T);
            if (_cache.TryGetValue(typeToBuild, out cachedMock))
                return (Mock<T>)cachedMock;
            if (buildStack.Contains(typeToBuild))
                throw new CircularDependencyException(typeToBuild);
            buildStack.Push(typeToBuild);
            var mock = new Mock<T>();
            _cache.Add(typeToBuild, mock);
            buildStack.Pop();
            return mock;
        }

        public void SetupMockGetters<T>(Mock<T> mock, Stack<Type> buildStack) where T : class
        {
            var type = typeof(T);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.CanRead && !propertyInfo.CanWrite && propertyInfo.PropertyType.IsInterface)
                {
                    SetupMockPropertyGetter(type, propertyInfo, mock, buildStack);
                }
            }
        }

        private void SetupMockPropertyGetter(Type type, PropertyInfo propertyInfo, Mock mock, Stack<Type> buildStack)
        {
            var methodTemplate = GetType().GetMethod("SetupDependency", BindingFlags.Instance | BindingFlags.NonPublic);
            var genericMethod = methodTemplate.MakeGenericMethod(new[] { type, propertyInfo.PropertyType });
            try
            {
                genericMethod.Invoke(this, new object[] {mock, propertyInfo, buildStack});
            }
            catch(TargetInvocationException e)
            {
                var innerException = e.InnerException as CircularDependencyException;
                if (innerException == null)
                    throw;
                throw innerException;
            }
        }

        // Resharper cannot see that this method is used becuase it is created using reflection
        // ReSharper disable UnusedMember.Local
        private void SetupDependency<TMock, TDependency>(Mock<TMock> mock, PropertyInfo propertyInfo, Stack<Type> buildStack) 
            where TMock : class 
            where TDependency : class
        {
            // Creates an expression for (x => x.Dependency) where Dependency is a read only 
            // property of type propertyInfo.PropertyType
            var argument = Expression.Parameter(typeof(TMock), "x");
            var getPropertyExpression = Expression.Property(argument, propertyInfo.Name);
            var expression = Expression.Lambda<Func<TMock, TDependency>>(getPropertyExpression, argument);

            var dependency = GetMock<TDependency>(buildStack).Object;
            mock.Setup(expression).Returns(dependency);
        }
        // ReSharper restore UnusedMember.Local
    }
}
