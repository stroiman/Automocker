using System;
using System.Linq;

namespace AutoMock
{
    /// <summary>
    /// This class responsible for objects to be used in unit tests, but with the dependencies of
    /// these objects automatically replaced by mock objects, reducing the need to manually
    /// configure this in unit tests, and manually adjusting tests when dependencies for the
    /// SUT changes.
    /// </summary>
    public class AutoMocker
    {
        private readonly IDependencyProvider _dependencyProvider;

        /// <summary>
        /// Creates a new <see cref="AutoMocker"/> instance.
        /// </summary>
        /// <param name="dependencyProvider">
        /// An <see cref="IDependencyProvider"/> to be used for instantiating constructor dependencies.
        /// </param>
        public AutoMocker(IDependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
        }

        /// <summary>
        /// Creates an instance of the class <typeparamref name="T"/>.
        /// </summary>
        public T GetInstance<T>()
        {
            var type = typeof(T);
            var constructor = type.GetConstructors().First();
            var parameters = constructor.GetParameters().Select(x => GetArgument(x.ParameterType)).ToArray();
            return (T)constructor.Invoke(parameters);
        }

        private object GetArgument(Type argumentType)
        {
            var getInstanceMethod = typeof(IDependencyProvider).GetMethod("GetInstance");
            var genericMethod = getInstanceMethod.MakeGenericMethod(argumentType);
            return genericMethod.Invoke(_dependencyProvider, new object[0]);
        }
    }
}
