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
        /// <summary>
        /// Resolves an instance of <typeparamref name="T"/>
        /// </summary>
        public T GetInstance<T>() where T : class
        {
            return new Mock<T>().Object;
        }
    }
}
