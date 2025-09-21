using Xunit;

namespace ProjectManagement.Tests.Controllers
{
    public class BasicControllerTests
    {
        [Fact]
        public void ControllerTest_ShouldPass()
        {
            var testValue = "test";
            var result = testValue.Length;
            Assert.Equal(4, result);
        }
    }
}
