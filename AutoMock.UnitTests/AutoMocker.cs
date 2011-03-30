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