using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

namespace AutoMock
{
	/// <summary>
	/// A specialized <see cref="AutoMocker"/> that injects <see cref="Moq"/> objects
	/// into the instance to create.
	/// </summary>
	public class MoqAutoMocker : AutoMocker
	{
	    private readonly MoqDependencyProvider _dependencyProvider;

        /// <summary>
        /// Creates a new <see cref="MoqAutoMocker"/> instance.
        /// </summary>
	    public MoqAutoMocker() : this(new MoqDependencyProvider())
		{ }

        private MoqAutoMocker(MoqDependencyProvider dependencyProvider)
            : base(dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
        }

        /// <summary>
        /// Finds a previously created instance; or creates a new instance of
        /// <see cref="Mock{T}"/> with the specified type of <typeparamref name="T"/>
        /// </summary>
        public Mock<T> GetMock<T>() where T : class
	    {
            return _dependencyProvider.GetMock<T>();
	    }
	}
}
