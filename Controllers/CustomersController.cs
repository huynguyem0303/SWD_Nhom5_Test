using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Management.XEvent;
using System.Collections.Specialized;
using System.IO.Enumeration;
using System.Net;
using System.Web;
using Twilio.Rest.Api.V2010.Account;
using TableReservation.Models;
using TableReservation.ViewModels;
using Twilio.Clients;
using Twilio.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;
using System.Text;

namespace TableReservation.Controllers
{

    [ApiController]
    [Route("API/[controller]")]
    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;

        private readonly IMapper _mapper;
        private readonly ITwilioRestClient _client;
        public CustomersController(AppDbContext context, IMapper mapper, ITwilioRestClient client)
        {
            _context = context;
            _mapper = mapper;
            _client = client;
        }
        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
        [HttpPost("GenerateOTP")]
        public IActionResult GenerateOTP(string phone)
        {
            try
            {

                Random rand = new Random();
                var sentOtp = rand.Next(1000, 9999);
                var hash = GetHash(sentOtp.ToString());
                string message = "Your OTP number is :" + sentOtp;
                var messageresc = MessageResource.Create(
                    to: new PhoneNumber("+84" + phone),
                    from: new PhoneNumber("+15393283714"),
                    body: message,
                    client: _client); // pass in the custom client
                return Ok(new
                {
                    Success = true,
                    data = sentOtp
                });


            }
            catch (Exception)
            {
                return BadRequest("Error!");
            }
        }
        [HttpPost("BookReservation")]
        public IActionResult BookReservation(int restaurantId, ViewAddBooking model)
        {
            try
            {
                var restaurant = _context.Restaurants.SingleOrDefault(x => x.RestaurantId == restaurantId);
                if (restaurant == null)
                {
                    return NotFound("Cant find restaurant id!!");
                }
                //Update
                if (restaurantId != restaurant.RestaurantId)
                {
                    return BadRequest("Invalid Id!!");
                }
                var full = "0" + model.Phone;
                var checkCus = _context.Customers.Where(p => p.Phone == full).FirstOrDefault();
               
                var checkTable = _context.Tables.Where(p => p.Size == model.GuestSize && p.Status == true).FirstOrDefault();
                if (checkTable == null)
                {
                    return NotFound("Cannot find table!!");
                }

                if (checkCus != null)
                {
                    var checkRes = _context.Reservations.Where(p => p.CustomerId == checkCus.CustomerId).OrderByDescending(p => p.CreatedAt).FirstOrDefault();
                    if (checkRes != null) {
                        int result = DateTime.Compare(checkRes.CreatedAt.AddMinutes(30), DateTime.Now);
                        if (result > 0)
                        {
                            return BadRequest("You have to wait 30 minutes to create new reservation!!");

                        }
                    }
                   
                    var reservation = new Reservations()
                    {
                        CustomerName = model.CustomerName,
                        CustomerId = checkCus.CustomerId,
                        Time = DateTime.Parse(model.Time),
                        RestaurantId = restaurantId,
                        Note = model.Note,
                        Size = model.GuestSize,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        TableId = checkTable.TableId
                        ,
                        Status = Models.Enum.Status.Pending
                    };
                    _context.Add(reservation);
                    _context.SaveChanges();
                    return Ok(new
                    {
                        Success = true,
                        data = reservation
                    });
                }
                else
                {
                    var customer = new Customers()
                    {
                        Phone = "0" + model.Phone,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Add(customer);
                    _context.SaveChanges();
                    var checkCreateCus = _context.Customers.Where(p => p.Phone == "0" + model.Phone).FirstOrDefault();
                    var reservation = new Reservations()
                    {
                        CustomerName = model.CustomerName,
                        CustomerId = checkCreateCus.CustomerId,
                        Time = DateTime.Parse(model.Time),
                        RestaurantId = restaurantId,
                        Note = model.Note,
                        Size = model.GuestSize,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        TableId = checkTable.TableId
                        ,
                        Status = Models.Enum.Status.Pending
                    };
                    _context.Add(reservation);
                    _context.SaveChanges();
                    return Ok(new
                    {
                        Success = true,
                        data = reservation
                    });
                }

            }
            catch (Exception)
            {
                return BadRequest("Error!");
            }

        }
        [HttpGet("GetAllReservationByPhone")]
        public IActionResult GetAllReservationByPhone(string phone)
        {
            try
            {
                var full = "0" + phone;
                var checkCus = _context.Customers.Where(p => p.Phone == full).FirstOrDefault();
                if (checkCus == null)
                {
                    return NotFound("Cant find Customer id!!");
                }
                //Update
                if (full != checkCus.Phone)
                {
                    return BadRequest("Invalid Phone!!");
                }
                var checkReservation = _context.Reservations.Where(p => p.CustomerId.Equals(checkCus.CustomerId)).OrderByDescending(p=>p.CreatedAt).ToList();
                if (checkReservation == null)
                {
                    return NotFound("Your reservation is empty!");
                }
                else
                {
                    List<ViewBooking> book = _mapper.Map<List<ViewBooking>>(checkReservation);
                    return Ok(book);
                }
            }
            catch (Exception)
            {
                return BadRequest("Error!");
            }
        }
    }
}

