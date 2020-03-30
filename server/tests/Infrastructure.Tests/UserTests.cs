using System;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain;
using server.core.Domain.Authentication;
using server.core.Domain.Error;
using server.core.Infrastructure.Dto;

namespace Infrastructure.Tests
{
    [TestFixture]
    public class UserTests
    {
        private const string Address = "foo@bar.baz";
        private const string Password = "qwertyuiop";

        [Test]
        public void Should_create_correct_domain_from_dto()
        {
            var dto = new UserDto
            {
                EmailAddress = Address,
                HashAlgorithm = "BCrypt",
                IsVerified = true,
                PasswordHash = "1234",
                UserId = Guid.NewGuid()
            };
            var user = UserDto.ToDomain(dto);

            user.UserId.Should().Be(dto.UserId);
            user.Email.Address.Should().BeEquivalentTo(dto.EmailAddress);
            user.Email.IsVerified.Should().Be(dto.IsVerified);
            user.Password.HashAlgorithm.Should().Be(HashAlgorithm.BCrypt);
            user.Password.HashedPassword.Should().BeEquivalentTo(dto.PasswordHash);
        }

        [Test]
        public void Should_create_correct_dto()
        {
            var user = User.CreateNew(Address, Password);
            var dto = UserDto.FromDomain(user);

            dto.EmailAddress.Should().BeEquivalentTo(user.Email.Address);
            dto.HashAlgorithm.Should().BeEquivalentTo(user.Password.HashAlgorithm.ToString());
            dto.IsVerified.Should().Be(user.Email.IsVerified);
            dto.PasswordHash.Should().BeEquivalentTo(user.Password.HashedPassword);
            dto.UserId.Should().Be(user.UserId);
        }

        [Test]
        public void Should_fail_on_unknown_hash_algorithm()
        {
            var dto = new UserDto
            {
                HashAlgorithm = "1235"
            };

            Action userCreation = () => UserDto.ToDomain(dto);

            userCreation.Should().Throw<UnknownHashAlgorithmException>();
        }
    }
}
