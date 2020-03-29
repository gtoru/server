using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain.Authentication;
using server.core.Domain.Error;

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
            Action passwordCreation = () => Password.Create(HashAlgorithm.BCrypt, RawPassword);

            passwordCreation.Should().NotThrow();
        }

        [Test]
        public void Should_not_create_password_if_password_is_too_long()
        {
            var longPassword = Enumerable.Repeat('a', 51).ToString();
            Action passwordCreation = () => Password.Create(HashAlgorithm.BCrypt, longPassword);

            passwordCreation.Should().Throw<PasswordTooLongException>("because password is longer than 50 symbols");
        }

        [Test]
        public void Should_verify_if_password_is_correct()
        {
            var password = Password.Create(HashAlgorithm.BCrypt, RawPassword);

            password.Verify(RawPassword).Should().BeTrue();
        }
    }
}
