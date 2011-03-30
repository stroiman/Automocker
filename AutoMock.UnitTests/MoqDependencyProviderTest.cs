using AutoMock.UnitTests.TestClasses;
using Moq;
using NUnit.Framework;

namespace AutoMock.UnitTests
{
    [TestFixture]
    public class MoqDependencyProviderTest
    {
        private MoqDependencyProvider _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new MoqDependencyProvider();
        }

        [Test]
        public void ResolvingAnInterfaceShouldReturnAMoq()
        {
            // Exercise
            var mockedInstance = _repository.GetInstance<ISimpleDependency>();

            // Verify
            var mock = Mock.Get(mockedInstance);
            Assert.That(mock, Is.Not.Null);
        }
    }
}
