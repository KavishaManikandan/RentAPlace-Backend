using Xunit;
using FluentAssertions;
using RentAPlace.Controllers;
using RentAPlace.Core.Models;
using RentAPlace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace RentAPlace.UnitTests
{
    public class UsersControllerTests
    {
        // Helper method: creates a fresh in-memory database for each test
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // new DB each time
                .Options;

            return new ApplicationDbContext(options);
        }

        private UsersController GetController(ApplicationDbContext db)
        {
            return new UsersController(db);
        }

        [Fact]
        public void Register_ShouldAddUser()
        {
            var db = GetDbContext();
            var controller = GetController(db);
            var user = new User { FullName = "Alice", Email = "alice@test.com", Password = "1234", Role = "User" };

            var result = controller.Register(user);

            result.Should().BeOfType<OkObjectResult>();
            db.Users.Should().Contain(u => u.Email == "alice@test.com");
        }

        [Fact]
        public void Login_WithValidCredentials_ShouldReturnUser()
        {
            var db = GetDbContext();
            db.Users.Add(new User { FullName = "Bob", Email = "bob@test.com", Password = "pass", Role = "User" });
            db.SaveChanges();
            var controller = GetController(db);

            var loginUser = new User { Email = "bob@test.com", Password = "pass" };

            var result = controller.Login(loginUser);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            var db = GetDbContext();
            db.Users.Add(new User { FullName = "Charlie", Email = "charlie@test.com", Password = "abc", Role = "User" });
            db.SaveChanges();
            var controller = GetController(db);

            var wrongLogin = new User { Email = "charlie@test.com", Password = "wrong" };

            var result = controller.Login(wrongLogin);

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public void GetAll_ShouldReturnAllUsers()
        {
            var db = GetDbContext();
            db.Users.AddRange(
                new User { FullName = "User1", Email = "u1@test.com", Password = "123", Role = "User" },
                new User { FullName = "User2", Email = "u2@test.com", Password = "123", Role = "User" }
            );
            db.SaveChanges();
            var controller = GetController(db);

            var result = controller.GetAll();

            var okResult = result as OkObjectResult;
            var users = okResult!.Value as List<User>;

            users.Should().HaveCount(2);
        }

        [Fact]
        public void Update_ShouldModifyExistingUser()
        {
            var db = GetDbContext();
            var user = new User { FullName = "Old Name", Email = "old@test.com", Password = "111", Role = "User" };
            db.Users.Add(user);
            db.SaveChanges();

            var controller = GetController(db);
            var updatedUser = new User { FullName = "New Name", Email = "new@test.com", Password = "222", Role = "Admin" };

            var result = controller.Update(user.UserId, updatedUser);

            result.Should().BeOfType<OkObjectResult>();
            db.Users.First().FullName.Should().Be("New Name");
        }

        [Fact]
        public void Delete_ShouldRemoveUser()
        {
            var db = GetDbContext();
            var user = new User { FullName = "To Delete", Email = "del@test.com", Password = "333", Role = "User" };
            db.Users.Add(user);
            db.SaveChanges();

            var controller = GetController(db);

            var result = controller.Delete(user.UserId);

            result.Should().BeOfType<OkResult>();
            db.Users.Should().BeEmpty();
        }
    }
}
