using AutoMock.UnitTests.TestClasses;
using Moq;
using NUnit.Framework;

namespace AutoMock.UnitTests
{
    [TestFixture]
    public class AutoMockerTest
    {
        private AutoMocker _automocker;
        private Mock<IDependencyProvider> _dependencyRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _dependencyRepositoryMock = new Mock<IDependencyProvider>();
            _automocker = new AutoMocker(_dependencyRepositoryMock.Object);
        }

        [Test]
        public void CreateInstanceWithoutDependencies()
        {
            var instance = _automocker.GetInstance<ClassWithoutDependencies>();
            Assert.That(instance, Is.InstanceOf<ClassWithoutDependencies>());
        }

        [Test]
        public void CreateInstanceWithDependencyShouldGetDependencyFromRepository()
        {
            // Setup
            var dependency = new Mock<ISimpleDependency>().Object;
            _dependencyRepositoryMock.Setup(x => x.GetInstance<ISimpleDependency>()).Returns(dependency);

            // Exercise
            var instance = _automocker.GetInstance<ClassWithSimpleDependency>();

            // Verify
            Assert.That(instance.Dependency, Is.SameAs(dependency));
        }

        [Test]
        public void CreateInstanceWithSpecificClassDependency()
        {
            // Exercise
            var instance = _automocker.GetInstance<ClassWithClassDependency>();

            // Verify
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Dependency, Is.Not.Null);
        }

        [Test]
        public void CreatingClassWithMultipleConstructorsShouldUseGreedyConstructor()
        {
            // Exercise
            var instance = _automocker.GetInstance<ClassWithMultipleConstructors>();

            // Verify
            Assert.That(instance.WasConstructedUsingGreedyConstructor);
        }

        [Test, ExpectedException(typeof(CircularDependencyException))]
        public void CircularConstructorDependenciesShouldBeDetected()
        {
            var instance = _automocker.GetInstance<ClassWithCircularDependency>();
        }

        [Test]
        public void InjectedInstanceShouldBeUsedInsteadOfAutoMock()
        {
            // Setup
            var injectedInstance = new Mock<ISimpleDependency>().Object;

            // Exercise
            var instance = _automocker.Using(injectedInstance).GetInstance<ClassWithSimpleDependency>();

            // Verify
            _dependencyRepositoryMock.Verify(x => x.GetInstance<ISimpleDependency>(), Times.Never());
            Assert.That(instance.Dependency, Is.SameAs(injectedInstance));
        }
    }
}
