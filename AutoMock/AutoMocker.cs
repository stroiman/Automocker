using System;

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
        /// <summary>
        /// Creates an instance of the class <typeparamref name="T"/>.
        /// </summary>
        public T GetInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }
    }
}
