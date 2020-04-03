using System;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain;

namespace Domain.Tests
{
    [TestFixture]
    public class UserTests
    {
        private const string Address = "foo@bar.baz";
        private const string Password = "qwerty";

        [Test]
        public void Should_create_with_unverified_email()
        {
            var user = User.CreateNew(Address, Password);

            user.Email.IsVerified.Should().BeFalse();
        }

        [Test]
        public void Should_set_correct_email()
        {
            var user = User.CreateNew(Address, Password);

            user.Email.Address.Should().BeEquivalentTo(Address);
        }

        [Test]
        public void Should_set_correct_password()
        {
            var user = User.CreateNew(Address, Password);

            Action passwordVerification = () => user.Password.Verify(Password);

            passwordVerification.Should().NotThrow();
        }
    }
}
