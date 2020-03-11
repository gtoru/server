using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain.Authorization;

namespace Domain.Tests
{
    [TestFixture]
    public class Tests
    {
        private const string RawPassword = "1234";

        [TestCase("")]
        [TestCase("123")]
        [TestCase("veryveryveryveryveryveryveryveryveryveryveryveryverylongpassword")]
        [TestCase("!425#2@657dhfhf#$%7/")]
        public void Should_not_verify_wrond_password(string guess)
        {
            var password = Password.Create(HashAlgorithm.BCrypt, RawPassword);

            password.Verify(guess).Should().BeFalse();
        }

        [Test]
        public void Should_create_password()
        {
            var password = Password.Create(HashAlgorithm.BCrypt, RawPassword);

            password.HashAlgorithm.Should().Be(HashAlgorithm.BCrypt);
            password.HashedPassword.Should().NotBeEmpty();
        }

        [Test]
        public void Should_not_create_password_if_password_is_too_long()
        {
            var longPassword = Enumerable.Repeat('a', 51).ToString();
            Action passwordCreation = () => Password.Create(HashAlgorithm.BCrypt, longPassword);

            passwordCreation.Should().Throw<ArgumentException>("because password is longer than 50 symbols");
        }

        [Test]
        public void Should_verify_if_password_is_correct()
        {
            var password = Password.Create(HashAlgorithm.BCrypt, RawPassword);

            password.Verify(RawPassword).Should().BeTrue();
        }
    }
}
