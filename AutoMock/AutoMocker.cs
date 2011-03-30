using System;
using System.Linq;
using System.Reflection;

namespace AutoMock
{
    /// <summary>
    /// This class responsible for objects to be used in unit tests, but with the dependencies of
    /// these objects automatically replaced by mock objects, reducing the need to manually
    /// configure this in unit tests, and manually adjusting tests when dependencies for the
    /// SUT changes.
    /// </summary>
    public class AutoMocker<TDependencyProvider> where TDependencyProvider : IDependencyProvider
    {
        private readonly TDependencyProvider _dependencyProvider;

        /// <summary>
        /// Creates a new <see cref="AutoMocker{T}"/> instance.
        /// </summary>
        /// <param name="dependencyProvider">
        /// An <see cref="IDependencyProvider"/> to be used for instantiating constructor dependencies.
        /// </param>
        public AutoMocker(TDependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
        }

        internal TDependencyProvider DependencyProvider { get { return _dependencyProvider; } }

        /// <summary>
        /// Creates an instance of the class <typeparamref name="T"/>.
        /// </summary>
        public T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        private object GetInstance(Type type)
        {
            var constructors = type.GetConstructors();
            var greedyConstructor = constructors.OrderBy(x => x.GetParameters().Count()).Last();
            var parameters = greedyConstructor.GetParameters().Select(x => GetArgument(x.ParameterType)).ToArray();
            return greedyConstructor.Invoke(parameters);            
        }

        private object GetArgument(Type argumentType)
        {
            if (argumentType.IsInterface)
            {
                var getInstanceMethod = typeof (IDependencyProvider).GetMethod("GetInstance");
                var genericMethod = getInstanceMethod.MakeGenericMethod(argumentType);
                return genericMethod.Invoke(_dependencyProvider, new object[0]);
            }
            else
            {
                return GetInstance(argumentType);
            }
        }
    }
}
