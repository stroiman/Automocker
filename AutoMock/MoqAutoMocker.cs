using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoMock
{
	/// <summary>
	/// A specialized <see cref="AutoMocker"/> that injects <see cref="Moq"/> objects
	/// into the instance to create.
	/// </summary>
	public class MoqAutoMocker : AutoMocker
	{
		public MoqAutoMocker() : base(new MoqDependencyProvider())
		{
		}
	}
}
