namespace AutoMock.UnitTests.TestClasses
{
    public interface ISimpleDependency { }

    public class ClassWithoutDependencies { }

    public class ClassWithSimpleDependency
    {
        public ISimpleDependency Dependency { get; private set; }

        public ClassWithSimpleDependency(ISimpleDependency dependency) { Dependency = dependency; }
    }

    public class ClassWithClassDependency
    {
        public ClassWithSimpleDependency Dependency { get; private set; }

        public ClassWithClassDependency(ClassWithSimpleDependency dependency) { Dependency = dependency; }
    }
}
