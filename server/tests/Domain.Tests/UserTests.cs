using System;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain;

namespace Domain.Tests
{
    [TestFixture]
    public class UserTests
    {
        [SetUp]
        public void SetUp()
        {
            _personalInfo = new PersonalInfo(
                "John Doe",
                new DateTime(1970, 01, 01),
                "Over the Rainbow",
                "Believer",
                "Monsters inc.");
        }

        private const string Address = "foo@bar.baz";
        private const string Password = "qwerty";
        private PersonalInfo _personalInfo;

        [Test]
        public void Should_create_with_unverified_email()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);

            user.Email.IsVerified.Should().BeFalse();
        }

        [Test]
        public void Should_set_correct_email()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);

            user.Email.Address.Should().BeEquivalentTo(Address);
        }

        [Test]
        public void Should_set_correct_password()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);

            Action passwordVerification = () => user.Password.Verify(Password);

            passwordVerification.Should().NotThrow();
        }

        [Test]
        public void Should_set_personal_info()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);

            user.PersonalInfo.Should().BeEquivalentTo(_personalInfo);
        }
    }
}
