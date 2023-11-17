namespace Tests
{
    using CastManager.Client;
    using CastManager.Client.Poll;
    using CastManager.Models.Desktops;
    using CastManager.Models.Device;
    using CastManager.CoreImpl;
    using CastManager.CoreImpl.Lookup.Addresses;
    using CastManager.CoreImpl.Lookup.RoundRobin;
    using FluentAssertions;
    using Moq;
    using CastManager.Core;

    [TestClass]
    public class LookupTests
    {
        Mock<IAppGlobalEvents> appEvents = new();

        [TestMethod]
        public void RoundRobin_ShouldBeSuccess()
        {
            var rr = new RoundRobin(0, 2);

            rr.Next.Should().Be(0);
            rr.Next.Should().Be(1);
            rr.Next.Should().Be(0);
            rr.Next.Should().Be(1);

            rr = new RoundRobin(0, 2, includeTheEnd: true);

            rr.Next.Should().Be(0);
            rr.Next.Should().Be(1);
            rr.Next.Should().Be(2);
            rr.Next.Should().Be(0);

            rr = new RoundRobin(0, 3, includeTheEnd: false);

            rr.Next.Should().Be(0);
            rr.Next.Should().Be(1);
            rr.Next.Should().Be(2);
            rr.Next.Should().Be(0);
        }

        [TestMethod]
        public void ActiveAddresses_ShouldBeSuccess()
        {
            IAddressesLoop a = new ActiveAddresses(new string[] { "1", "2", "3", "4", "5"});

            a.AddFilter("3");
            a.Next.Should().Be("1");
            a.Next.Should().Be("2");
            a.Next.Should().Be("4");
            a.AddFilter("4");
            a.Next.Should().Be("5");
            a.AddFilter("5");
            a.Next.Should().Be("1");
            a.Next.Should().Be("2");
            a.IsAtFilter("1").Should().BeFalse();
            a.IsAtFilter("3").Should().BeTrue();
            a.Next.Should().Be("1");
            a.AddFilter("1");
            a.Next.Should().Be("2");
            a.AddFilter("2");
            try
            {
                a.Next.Should().NotBe("3");
            }
            catch (Exception ex)
            {
                ex.Should().As<ActiveAddressesException>();
                ex.Message.Should().Be("Ups, all active addresses was filtered");
            }
            a.ClearFilters();
            a.Next.Should().Be("3");
            a.Next.Should().Be("4");
            a.Next.Should().Be("5");
            a.Next.Should().Be("1");
            a.Next.Should().Be("2");
        }

        [TestMethod]
        public void Lookups_ShouldBeSuccess()
        {
            var mockClients = new Mock<IClientsFactory>();
            mockClients
                .Setup(s => s.LookupActiveLocalsAsync(It.IsAny<Action<string>>()))
                .Returns( (Action<string> a) =>
                {
                    a?.Invoke("mockip");
                    return Task.CompletedTask;
                });
            mockClients
                .Setup(s => s.GetPollClient(It.IsAny<TimeSpan>(), It.IsAny<CancellationTokenSource>()))
                .Returns((TimeSpan t, CancellationTokenSource cs) => new PollClientMock(t, cs));

            var clinets = mockClients.Object;

            DesktopInfo? foundDesktop = null;
            DeviceData? foundDevice = null;

            var lookups = new LookupService(clinets, appEvents.Object)
            {
                OnDesktopFound = desktopInfo =>  
                {
                    foundDesktop= desktopInfo;
                },
                OnDeviceFound= deviceInfo => 
                {
                    foundDevice = deviceInfo;
                }
            };
            lookups.Start();
            lookups.Wait();

            foundDesktop.Should().NotBeNull();
            foundDesktop?.TatamiText.Should().Be("Mock_Name");
            foundDesktop?.Ip.Should().Be("mockip");
            
            foundDevice.Should().NotBeNull();
            foundDevice?.Info.IPAddress.Should().Be("mockip");
        }
    }


}