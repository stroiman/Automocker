namespace AutoMock
{
    /// <summary>
    /// This interface is responsible for managing and creating dependencies for a component to
    /// be created by <see cref="AutoMocker"/>
    /// </summary>
    public interface IDependencyProvider
    {
        /// <summary>
        /// Resolves an instance of <typeparamref name="T"/>
        /// </summary>
        T GetInstance<T>() where T : class;
    }
}
