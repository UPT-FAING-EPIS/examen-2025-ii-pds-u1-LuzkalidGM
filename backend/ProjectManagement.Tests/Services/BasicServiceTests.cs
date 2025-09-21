using Xunit;

namespace ProjectManagement.Tests.Services
{
    public class BasicServiceTests
    {
        [Fact]
        public void BasicTest_ShouldPass()
        {
            var expected = true;
            var actual = true;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BasicMath_ShouldWork()
        {
            var a = 2;
            var b = 3;
            var result = a + b;
            Assert.Equal(5, result);
        }
    }
}
