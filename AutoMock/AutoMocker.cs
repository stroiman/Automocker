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
        private readonly IDependencyRepository _dependencyRepository;

        /// <summary>
        /// Creates a new <see cref="AutoMocker"/> instance.
        /// </summary>
        /// <param name="dependencyRepository">
        /// An <see cref="IDependencyRepository"/> to be used for instantiating constructor dependencies.
        /// </param>
        public AutoMocker(IDependencyRepository dependencyRepository)
        {
            _dependencyRepository = dependencyRepository;
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
            var getInstanceMethod = typeof(IDependencyRepository).GetMethod("GetInstance");
            var genericMethod = getInstanceMethod.MakeGenericMethod(argumentType);
            return genericMethod.Invoke(_dependencyRepository, new object[0]);
        }
    }
}
