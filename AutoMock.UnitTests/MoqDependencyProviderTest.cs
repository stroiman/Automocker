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

        [Test]
        public void ResolvingAfterGetMockShouldReturnSameInstance()
        {
            // Setup
            var mock = _repository.GetMock<ISimpleDependency>();

            // Exercise
            var mockedInstance = _repository.GetInstance<ISimpleDependency>();

            // Verify
            Assert.That(mock.Object, Is.SameAs(mockedInstance));
        }

        [Test]
        public void GetMockAfterResolvingShouldReturnSameInstance()
        {
            // Setup
            var mockedInstance = _repository.GetInstance<ISimpleDependency>();

            // Exercise
            var mock = _repository.GetMock<ISimpleDependency>();

            // Verify
            Assert.That(mockedInstance, Is.SameAs(mock.Object));
        }
    }
}
