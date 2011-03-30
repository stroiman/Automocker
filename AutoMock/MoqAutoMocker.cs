using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	}
}
