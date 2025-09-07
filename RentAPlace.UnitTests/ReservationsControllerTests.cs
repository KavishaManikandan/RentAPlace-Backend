using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RentAPlace.API.Controllers;
using RentAPlace.API.Dtos;
using RentAPlace.API.Services;
using RentAPlace.Core.Models;
using RentAPlace.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace RentAPlace.UnitTests
{
    public class ReservationsControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateReservation_ShouldFail_WhenPropertyDoesNotExist()
        {
            var db = GetDbContext();
            var emailService = new Mock<IEmailService>();
            var controller = new ReservationsController(db);

            var dto = new ReservationDto
            {
                PropertyId = 999,
                UserId = 1,
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(1),
                Guests = 2
            };

            var result = await controller.CreateReservation(dto, emailService.Object);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetReservation_ShouldReturnReservation()
        {
            var db = GetDbContext();
            db.Properties.Add(new Property { PropertyId = 1, Title = "Villa" });
            db.Users.Add(new User { UserId = 1, FullName = "User1", Email = "user@test.com" });
            db.Reservations.Add(new Reservation
            {
                ReservationId = 1,
                PropertyId = 1,
                UserId = 1,
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(2),
                Guests = 2
            });
            db.SaveChanges();

            var controller = new ReservationsController(db);

            var result = await controller.GetReservation(1);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            var reservation = ok!.Value as Reservation;
            reservation!.ReservationId.Should().Be(1);
        }

        [Fact]
        public async Task UpdateStatus_ShouldChangeReservationStatus()
        {
            var db = GetDbContext();
            db.Reservations.Add(new Reservation
            {
                ReservationId = 1,
                PropertyId = 1,
                UserId = 1,
                Status = "Pending"
            });
            db.SaveChanges();

            var controller = new ReservationsController(db);

            var statusDto = new UpdateStatusDto { Status = "Confirmed" };

            var result = await controller.UpdateStatus(1, statusDto);

            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            var updated = ok!.Value as Reservation;
            updated!.Status.Should().Be("Confirmed");
        }

        [Fact]
        public async Task DeleteReservation_ShouldRemoveReservation()
        {
            var db = GetDbContext();
            db.Reservations.Add(new Reservation
            {
                ReservationId = 1,
                PropertyId = 1,
                UserId = 1,
                Status = "Pending"
            });
            db.SaveChanges();

            var controller = new ReservationsController(db);

            var result = await controller.DeleteReservation(1);

            result.Should().BeOfType<NoContentResult>();
            db.Reservations.Should().BeEmpty();
        }
    }
}
