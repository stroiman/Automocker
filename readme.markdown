Automocker for .NET
===================

This is a simple lightweight automocker (currently <250 lines of code) for the Moq mocking framework. See http://code.google.com/p/moq/

The code was at first designed in order to be able to easily use use different mocking frameworks, but
in the end >50% of the code is moq specific. Still it is possible to create variations, and reuse parts
of the code.

An automocker is just a simple tool for writing unit tests for components use dependency 
injection through constructor parameters. It is probably best explained by this unit test.

    public interface ISimpleDependency { }

    public class ClassWithSimpleDependency
    {
        public ISimpleDependency Dependency { get; private set; }

        public ClassWithSimpleDependency(ISimpleDependency dependency) { Dependency = dependency; }
    }

    [TestFixture]
    public class MoqAutoMockerIntegrationTests
    {
        private MoqAutoMocker _automocker;

        [SetUp]
        public void Setup()
        {
            _automocker = new MoqAutoMocker();
        }

        [Test]
        public void ClassWithDependencyShouldBeConstructedWithMockedDependency()
        {
            // Exercise
            var instance = _automocker.GetInstance<ClassWithSimpleDependency>();

            // Verify
            var dependencyMock = Mock.Get(instance.Dependency);
            Assert.That(dependencyMock, Is.Not.Null);
        }

        [Test]
        public void CreateInstanceAfterGetMockShouldUseSameMock()
        {
            // Setup
            var mock = _automocker.GetMock<ISimpleDependency>();

            // Exercise
            var instance = _automocker.GetInstance<ClassWithSimpleDependency>();

            // Verify
            Assert.That(instance.Dependency, Is.SameAs(mock.Object));
        }
    }

Using an automocker can make the unit tests quicker to write, and can result in less maintenance of test code, as tests
will still compile if dependencies change for a specific class in the system (assuming that the new dependency does not
influence the behavior that is being covered by existing tests)

A special case handled by this automocker is if an interface defines a dependency to another interface. In this case, the
automocker will automatically set up the getter on this interface to return a new mock.

    public interface IInterfaceWithDependency
    {
        ISimpleDependency Dependency { get; }
    }

    public class ClassWithNestedDependency
    {
        public IInterfaceWithDependency Dependency { get; private set; }

        public ClassWithNestedDependency(IInterfaceWithDependency dependency) { Dependency = dependency; }
    }


        [Test]
        public void GetMockBeforeCreateInstanceWithNestedDependencyShouldReuseSameMock()
        {
            // Setup
            var mock = _automocker.GetMock<ISimpleDependency>();

            // Exercise
            var instance = _automocker.GetInstance<ClassWithNestedDependency>();

            // Veridy
            Assert.That(instance.Dependency.Dependency, Is.SameAs(mock.Object));
        }

This behavior exists because I needed it for a special case. But I plan to make it optional.

Usage
-----

I would suggest simply copying the files to your own unit test project, the codebase being so small as it is. But if 
you don't want to do that, there is also a compiled version available for download.
