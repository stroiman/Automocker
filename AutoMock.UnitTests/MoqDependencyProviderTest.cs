using AutoMock.UnitTests.TestClasses;
using Moq;
using NUnit.Framework;

namespace AutoMock.UnitTests
{
    [TestFixture]
    public class MoqDependencyProviderTest
    {
        [Test]
        public void ResolvingAnInterfaceShouldReturnAMoq()
        {
            // Setup
            var repository = new MoqDependencyProvider();

            // Exercise
            var mockedInstance = repository.GetInstance<ISimpleDependency>();

            // Verify
            var mock = Mock.Get(mockedInstance);
            Assert.That(mock, Is.Not.Null);
        }
    }
}
