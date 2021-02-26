using JPlayer.Api.AppStart;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;

namespace JPlayer.Test
{
    [TestFixture]
    public class StartAppTest
    {
        [Test]
        public void StartApp_Should_Run_WithoutErrors()
        {
            IWebHost webHost = WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            Assert.IsNotNull(webHost);
        }
    }
}