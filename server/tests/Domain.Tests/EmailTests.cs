using FluentAssertions;
using NUnit.Framework;
using server.core.Domain.Authentication;

namespace Domain.Tests
{
    [TestFixture]
    public class EmailTests
    {
        private const string Address = "foo@bar.baz";
        [Test]
        public void Should_be_unverified_on_creation()
        {
            var email = Email.Create(Address);

            email.IsVerified.Should().BeFalse();
        }

        [Test]
        public void Should_initialise_address_field()
        {
            var email = Email.Create(Address);

            email.Address.Should().BeEquivalentTo(Address);
        }

        [Test]
        public void Should_change_verification_statuc_on_verify()
        {
            var email = Email.Create(Address);

            email.Verify();

            email.IsVerified.Should().BeTrue();
        }
    }
}
