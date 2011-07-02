using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoMock
{
    /// <summary>
    /// This exception is throw when trying to create an instance of a type that has circular
    /// dependencies to itself.
    /// </summary>
    public class CircularDependencyException : Exception
    {
        private readonly Type _type;

        public CircularDependencyException(Type type)
        {
            _type = type;
        }

        public override string ToString()
        {
            return string.Format(
                "Error creating an instance of type {0}, as it has circular dependencies to itself\r\n\r\n{1}",
                _type.Name, base.ToString());
        }
    }

    /// <summary>
    /// This class responsible for objects to be used in unit tests, but with the dependencies of
    /// these objects automatically replaced by mock objects, reducing the need to manually
    /// configure this in unit tests, and manually adjusting tests when dependencies for the
    /// SUT changes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The automocker will always try to find the greediest constructor, i.e. the constructor
    /// taking the greates number of parameters.
    /// </para>
    /// <para>
    /// An instance created using the automocker can not take several constructor parameters of
    /// the same type.
    /// </para>
    /// </remarks>
    public class AutoMocker<TDependencyProvider> where TDependencyProvider : IDependencyProvider
    {
        private readonly TDependencyProvider _dependencyProvider;
        private readonly Dictionary<Type, object> _injectedInstances;
        
        /// <summary>
        /// Creates a new <see cref="AutoMocker{T}"/> instance.
        /// </summary>
        /// <param name="dependencyProvider">
        /// An <see cref="IDependencyProvider"/> to be used for instantiating constructor dependencies.
        /// </param>
        public AutoMocker(TDependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
            _injectedInstances = new Dictionary<Type, object>();
        }

        internal TDependencyProvider DependencyProvider { get { return _dependencyProvider; } }

        /// <summary>
        /// Creates an instance of the class <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="CircularDependencyException">
        /// While building an instance of <typeparam name="T"/>, a circular dependency
        /// was detected, making it impossible to build the instance.
        /// </exception>
        public T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T), new Stack<Type>());
        }

        private object GetInstance(Type type, Stack<Type> buildStack)
        {
            if (buildStack.Contains(type))
                throw new CircularDependencyException(type);
            buildStack.Push(type);
            var constructors = type.GetConstructors();
            var greedyConstructor = constructors.OrderBy(x => x.GetParameters().Count()).Last();
            var parameters = greedyConstructor.GetParameters().Select(x => GetArgument(x.ParameterType, buildStack)).ToArray();
            buildStack.Pop();
            return greedyConstructor.Invoke(parameters);            
        }

        private object GetArgument(Type argumentType, Stack<Type> stack)
        {
            object result;
            if (_injectedInstances.TryGetValue(argumentType, out result))
                return result;
            if (argumentType.IsInterface)
            {
                var getInstanceMethod = typeof (IDependencyProvider).GetMethod("GetInstance");
                var genericMethod = getInstanceMethod.MakeGenericMethod(argumentType);
                return genericMethod.Invoke(_dependencyProvider, new object[0]);
            }
            return GetInstance(argumentType, stack);
        }

        /// <summary>
        /// Specifies a concrete instance to be used as argument to the constructor instead
        /// of generating it automatically.
        /// </summary>
        /// <typeparam name="T">The data type for the parameter to inject.</typeparam>
        /// <param name="injectedInstance">The actual value for the parameter.</param>
        public AutoMocker<TDependencyProvider> Using<T>(T injectedInstance)
        {
            _injectedInstances[typeof(T)] = injectedInstance;
            return this;
        }
    }
}
