using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Moq;

namespace AutoMock
{
	/// <summary>
	/// A specialized <see cref="AutoMocker{T}"/> that injects <see cref="Moq"/> objects
	/// into the instance to create.
	/// </summary>
	public class MoqAutoMocker : AutoMocker<MoqDependencyProvider>
	{
	    /// <summary>
        /// Creates a new <see cref="MoqAutoMocker"/> instance.
        /// </summary>
	    public MoqAutoMocker() : base(new MoqDependencyProvider())
		{ }

        /// <summary>
        /// Finds a previously created instance; or creates a new instance of
        /// <see cref="Mock{T}"/> with the specified type of <typeparamref name="T"/>
        /// </summary>
        public Mock<T> GetMock<T>() where T : class
	    {
            return DependencyProvider.GetMock<T>();
	    }

	    public void MockDependencies<T>(Mock<T> dependentMock) where T : class
	    {
            SetupMockGetters(dependentMock);
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
            try
            {
                genericMethod.Invoke(this, new object[] {mock, propertyInfo});
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
        private void SetupDependency<TMock, TDependency>(Mock<TMock> mock, PropertyInfo propertyInfo) 
            where TMock : class 
            where TDependency : class
        {
            // Creates an expression for (x => x.Dependency) where Dependency is a read only 
            // property of type propertyInfo.PropertyType
            var argument = Expression.Parameter(typeof(TMock), "x");
            var getPropertyExpression = Expression.Property(argument, propertyInfo.Name);
            var expression = Expression.Lambda<Func<TMock, TDependency>>(getPropertyExpression, argument);

            var dependency = GetMock<TDependency>().Object;
            mock.Setup(expression).Returns(dependency);
        }
        // ReSharper restore UnusedMember.Local
	}
}
