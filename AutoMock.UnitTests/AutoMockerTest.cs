using AutoMock.UnitTests.TestClasses;
using NUnit.Framework;

namespace AutoMock.UnitTests
{
    [TestFixture]
    public class AutoMockerTest
    {
        [Test]
        public void CreateInstanceWithoutDependencies()
        {
            var automocker = new AutoMocker();
            var instance = automocker.GetInstance<ClassWithoutDependencies>();
            Assert.That(instance, Is.InstanceOf<ClassWithoutDependencies>());
        }
    }
}
