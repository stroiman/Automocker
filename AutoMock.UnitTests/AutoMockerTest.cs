using AutoMock.UnitTests.TestClasses;
using NUnit.Framework;

namespace AutoMock.UnitTests
{
    [TestFixture]
    public class AutoMockerTest
    {
        private AutoMocker _automocker;

        [SetUp]
        public void Setup()
        {
            _automocker = new AutoMocker();
        }

        [Test]
        public void CreateInstanceWithoutDependencies()
        {
            var instance = _automocker.GetInstance<ClassWithoutDependencies>();
            Assert.That(instance, Is.InstanceOf<ClassWithoutDependencies>());
        }
    }
}
