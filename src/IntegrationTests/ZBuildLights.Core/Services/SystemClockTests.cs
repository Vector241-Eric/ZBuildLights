using System;
using System.Threading;
using NUnit.Framework;
using Should;
using ZBuildLights.Core.Services;

namespace IntegrationTests.ZBuildLights.Core.Services
{
    public class SystemClockTests
    {
        [TestFixture]
        public class When_using_current_time
        {
            private double _ellapsedTime;

            [SetUp]
            public void ContextSetup()
            {
                SystemClock.UseCurrentTime();

                var now1 = SystemClock.Now();
                Thread.Sleep(TimeSpan.FromSeconds(2));
                var now2 = SystemClock.Now();

                _ellapsedTime = (now2 - now1).TotalMilliseconds;
            }

            [Test]
            public void Should_change_values()
            {
                _ellapsedTime.ShouldBeGreaterThan(1500);                
            } 
        } 
    }
}