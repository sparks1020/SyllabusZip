using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using SyllabusZip.Services;

namespace SyllabusZip.Tests.Services
{

    /// <summary>   (Unit Test Fixture) the canvas service tests. </summary>
    [TestFixture]
    public class CanvasServiceTests : CanvasTestSetup
    {
        [Test]
        public void GivenCanvasApplication_GetCourses_ReturnsValidContent()
        {
            var services = ServiceCollection.BuildServiceProvider();
            var service = services.GetRequiredService<ICanvasService>();

            if(!service.TryGetCourses(out var courses))
            {
                Assert.Fail("There is no reason we should not get some courses back.");
            }
            //Later this will be strongly typed, just a stop gap
            Assert.IsTrue(JsonConvert.SerializeObject(courses).Contains("Underwater Basketweaving"));
        }
    }
}
