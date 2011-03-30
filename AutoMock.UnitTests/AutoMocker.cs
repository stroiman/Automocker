using System;
using AutoMock.UnitTests.TestClasses;

namespace AutoMock.UnitTests
{
    public class AutoMocker : AutoMocker<IDependencyProvider>
    {
        public AutoMocker(IDependencyProvider dependencyProvider)
            : base(dependencyProvider)
        {
        }
    }
}