using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableReservation;
using TableReservation.Controllers;
using TableReservation.Mappers;
using TableReservation.Models;
using TableReservation.ViewModels;
using Twilio.Clients;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tests
{
    public class CustomersTest:SetupTest
    {
        private readonly CustomersController _customerController;
        public CustomersTest()
        {
           
        }

        [Fact]
        public async void CreateBooking_ShouldReturnOkWhenSuccess()
        {
            CustomersController controller = new CustomersController(_dbContext, _mapperConfig, _client.Object);
            // arrange
            var ViewAddBooking = new ViewAddBooking();
            ViewAddBooking.CustomerName = "Huy";
            ViewAddBooking.Time = DateTime.Now.ToString();
            ViewAddBooking.Phone = "582582582";
            ViewAddBooking.GuestSize = 4;
            ViewAddBooking.Note = "";
            var restaurantid = 1;

            // act
            var result = controller.BookReservation(restaurantid, ViewAddBooking);

            // assert
            result.Should().BeOfType(typeof(OkObjectResult));
        }
        [Fact]
        public async void CreateBooking_ShouldReturnNotFoundResultWhenNotFindTable()
        {
            CustomersController controller = new CustomersController(_dbContext, _mapperConfig, _client.Object);
            // arrange
            var ViewAddBooking = new ViewAddBooking();
            ViewAddBooking.CustomerName = "Huy";
            ViewAddBooking.Time = DateTime.Now.ToString();
            ViewAddBooking.Phone = "369852147";
            ViewAddBooking.GuestSize = 69;
            ViewAddBooking.Note = "";
            var restaurantid = 1;

            // act
            var result = controller.BookReservation(restaurantid, ViewAddBooking);

            // assert
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }
        [Fact] 
        public async void CreateBooking_ShouldReturnNotFoundResultWhenNotFindRestaurant()
        {
            CustomersController controller = new CustomersController(_dbContext, _mapperConfig, _client.Object);
            // arrange
            var ViewAddBooking = new ViewAddBooking();
            ViewAddBooking.CustomerName = "Huy";
            ViewAddBooking.Time = DateTime.Now.ToString();
            ViewAddBooking.Phone = "369852147";
            ViewAddBooking.GuestSize = 4;
            ViewAddBooking.Note = "";
            var restaurantid = 99;

            // act
            var result = controller.BookReservation(restaurantid, ViewAddBooking);

            // assert
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }
        [Fact]
        public async void CreateBooking_ShouldReturnBadRequestResultWhenCreateBefore30Minutes()
        {
            CustomersController controller = new CustomersController(_dbContext, _mapperConfig, _client.Object);
            // arrange
            var ViewAddBooking = new ViewAddBooking();
            ViewAddBooking.CustomerName = "Huy";
            ViewAddBooking.Time = DateTime.Now.ToString();
            ViewAddBooking.Phone = "582582582";
            ViewAddBooking.GuestSize = 4;
            ViewAddBooking.Note = "";
            var restaurantid = 1;

            // act
            controller.BookReservation(restaurantid, ViewAddBooking);
            var result = controller.BookReservation(restaurantid, ViewAddBooking);
            // assert
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }
    }
}
