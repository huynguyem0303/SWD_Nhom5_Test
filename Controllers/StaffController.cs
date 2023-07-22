using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using TableReservation.Models;
using TableReservation.ViewModels;

namespace TableReservation.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class StaffController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _con;
        private readonly IMapper _mapper;
        public StaffController(AppDbContext context, IConfiguration con, IMapper mapper)
        {
            _context = context;
            _con = con;
            _mapper = mapper;
        }
        [HttpGet("GetAllReservations")]
        public IActionResult GetAllReservations()
        {
            var stream = UserController.currentJWT;
            if (stream == null)
            {
                return BadRequest("You need to Login to use this function!!");
            }
            else
            {
                string author = Authorize();
                string check = DecodeJWT();
                if (check == "False" && author == "False")
                {

                    var reservationList = _context.Reservations.OrderByDescending(p=>p.ReservationId).ToList();
                    List<ViewStaffReservation> list = _mapper.Map<List<ViewStaffReservation>>(reservationList);
                    return Ok(list);
                }
                else if (check == "True")
                {
                    return BadRequest("You have been banned, you cant use this function!");
                }
                else
                {
                    return BadRequest("You are not allowed to use this function!"); 
                }
            }
        }

        //METHOD GET: lấy thành phần theo id
        [HttpGet("GetReservation/{customerName}")]
        public IActionResult GetReservation(string customerName)
        {
            //LINQ query
            try
            {
                var stream = UserController.currentJWT;
                if (stream == null)
                {
                    return BadRequest("You need to Login to use this function!!");
                }
                else
                {
                    string check = DecodeJWT();
                    string author = Authorize();
                    if (check == "False" && author == "False")
                    {
                        var reservation = _context.Reservations.SingleOrDefault(s => s.CustomerName.Trim().ToLower().Contains(customerName.Trim().ToLower()));
                        ViewStaffReservation list = _mapper.Map<ViewStaffReservation>(reservation);
                        if (reservation == null)
                        {
                            return NotFound();
                        }
                        return Ok(list);
                    }
                    else if (check == "True")
                    {
                        return BadRequest("You have been banned, you cant use this function!");
                    }
                    else
                    {
                        return BadRequest("You are not allowed to use this function!");
                    }
                }
            }
            catch
            {
                return BadRequest("Error!");
            }

        }


        //METHOD POST: Create tạo mới
        [HttpPost("CreateReservation")]
        public IActionResult CreateReservation(ViewStaffReservation model)
        {
            try
            {
                //    var stream = UserController.currentJWT;
                //    if (stream == null)
                //    {
                //        return BadRequest("You need to Login to use this function!!");
                //    }
                //    else
                //{
                //    string author = Authorize();
                //    string check = DecodeJWT();
                //    if (check == "False" && author == "False")
                //    {
                var restaurant = _context.Restaurants.SingleOrDefault(x => x.RestaurantId == model.RestaurantId);
                if (restaurant == null)
                {
                    return NotFound("Cant find restaurant id!!");
                }
                //Update
                if (model.RestaurantId != restaurant.RestaurantId)
                {
                    return BadRequest("Invalid Id!!");
                }
                var customer = _context.Customers.SingleOrDefault(x => x.CustomerId == model.CustomerId);
                if (customer == null)
                {
                    return NotFound("Cant find customer id!!");
                }
                //Update
                if (model.CustomerId != customer.CustomerId)
                {
                    return BadRequest("Invalid Id!!");
                }
                else
                {
                    var reservations = new Reservations();
                    _mapper.Map(model, reservations);
                    reservations.CreatedAt = DateTime.Now;
                    reservations.UpdatedAt = DateTime.Now;
                    _context.Add(reservations);
                    _context.SaveChanges();
                    return Ok(new
                    {
                        Success = true,
                        Data = model
                    });
                }
            }
            //    else if (check == "True")
            //    {
            //        return BadRequest("You have been banned, you cant use this function!");
            //    }
            //    else
            //    {
            //        return BadRequest("You are not allowed to use this function!");
            //    }
            //}
            //}
            catch (Exception)
            {
                return BadRequest("Error!");
            }

        }

        //METHOD PUT: Cập nhật (có truyền vào id)
        [HttpPut("UpdateReservation/{id}")]
        public IActionResult UpdateReservation(int id, ViewStaffReservation model)
        {
            try
            {
                ////LINQ Query
                //var stream = UserController.currentJWT;
                //if (stream == null)
                //{
                //    return BadRequest("You need to Login to use this function!!");
                //}
                //else
                //{
                //    string author = Authorize();
                //    string check = DecodeJWT();
                //    if (check == "False" && author == "False")
                //    {
                var restaurant = _context.Reservations.SingleOrDefault(x => x.RestaurantId == model.RestaurantId);
                if (restaurant == null)
                {
                    return NotFound("Cant find restaurant id!!");
                }
                //Update
                if (model.RestaurantId != restaurant.RestaurantId)
                {
                    return BadRequest("Invalid Id!!");
                }
                var customer = _context.Customers.SingleOrDefault(x => x.CustomerId == model.CustomerId);
                if (customer == null)
                {
                    return NotFound("Cant find customer id!!");
                }
                var reservation = _context.Reservations.SingleOrDefault(x => x.RestaurantId == id);
                if (reservation == null)
                {
                    return NotFound("Cant find menu id!!");
                }
                //Update
                if (id != reservation.ReservationId)
                {
                    return BadRequest("Invalid Id!!");
                }
                else
                {
                    _mapper.Map(model, reservation);
                    reservation.UpdatedAt = DateTime.Now;
                   
                    var tracker = _context.Attach(reservation);
                    tracker.State = EntityState.Modified;
                    _context.SaveChanges();
                    return Ok(new
                    {
                        Success = true,
                        data = model
                    });
                }
            }
            //        else if (check == "True")
            //        {
            //            return BadRequest("You have been banned, you cant use this function!");
            //        }
            //        else
            //        {
            //            return BadRequest("You are not allowed to use this function!");
            //        }
            //    }
            //}
            catch (Exception)
            {
                return BadRequest("Error!");
            }
        }
        [HttpPut("UpdateTableStatus/{id}")]
        public IActionResult UpdateTableStatus(int id, EditStatusTableView model)
        {
            try
            {
                //LINQ Query
                var stream = UserController.currentJWT;
                if (stream == null)
                {
                    return BadRequest("You need to Login to use this function!!");
                }
                else
                {
                    string author = Authorize();
                    string check = DecodeJWT();
                    if (check == "False" && author == "False")
                    {
                        var table = _context.Tables.SingleOrDefault(x => x.TableId == id);
                        if (table == null)
                        {
                            return NotFound("Cant find menu id!!");
                        }
                        //Update
                        if (id != table.TableId)
                        {
                            return BadRequest("Invalid Id!!");
                        }
                        else
                        {
                            _mapper.Map(model, table);
                            table.UpdatedAt = DateTime.Now;
                            var tracker = _context.Attach(table);
                            tracker.State = EntityState.Modified;
                            _context.SaveChanges();
                            ViewTables list = _mapper.Map<ViewTables>(table);
                            return Ok(new
                            {
                                Success = true,
                                data = list
                            });
                        }
                    }
                    else if (check == "True")
                    {
                        return BadRequest("You have been banned, you cant use this function!");
                    }
                    else
                    {
                        return BadRequest("You are not allowed to use this function!");
                    }

                }
            }
            catch
            {
                return BadRequest("Error!");
            }
        }
        [HttpPut("Checkout/{id}")]
        public IActionResult Checkout(int id)
        {
            try
            {
                //LINQ Query
                var stream = UserController.currentJWT;
                if (stream == null)
                {
                    return BadRequest("You need to Login to use this function!!");
                }
                else
                {
                    string author = Authorize();
                    string check = DecodeJWT();
                    if (check == "False" && author == "False")
                    {
                        var reservation = _context.Reservations.SingleOrDefault(x => x.ReservationId == id);
                        var table = _context.Tables.SingleOrDefault(x => x.TableId == reservation.TableId);
                        if (reservation == null)
                        {
                            return NotFound("Cant find reservation !!");
                        }
                        //Update
                        if (id != reservation.ReservationId)
                        {
                            return BadRequest("Invalid reservation Id!!");
                        }if(reservation.Status==Models.Enum.Status.Pending || reservation.Status == Models.Enum.Status.Cancel || reservation.Status == Models.Enum.Status.CheckOut)
                        {
                            return BadRequest("Can't use this function because reservation status is not On Going!!");
                        }
                        else
                        {
                            table.Status = true;
                            table.UpdatedAt = DateTime.Now;
                            reservation.Status = Models.Enum.Status.CheckOut;
                            var tracker = _context.Attach(table);
                            var tracker1 = _context.Attach(reservation);
                            tracker1.State = EntityState.Modified;
                            tracker.State = EntityState.Modified;
                            _context.SaveChanges();

                            ViewStaffReservation list = _mapper.Map<ViewStaffReservation>(reservation);
                            return Ok(new
                            {
                                Success = true,
                                data = list
                            });
                        }
                    }
                    else if (check == "True")
                    {
                        return BadRequest("You have been banned, you cant use this function!");
                    }
                    else
                    {
                        return BadRequest("You are not allowed to use this function!");
                    }

                }
            }
            catch
            {
                return BadRequest("Error!");
            }
        }
        [HttpPut("Approve/{id}")]
        public IActionResult Approve(int id)
        {
            try
            {
                //LINQ Query
                var stream = UserController.currentJWT;
                if (stream == null)
                {
                    return BadRequest("You need to Login to use this function!!");
                }
                else
                {
                    string author = Authorize();
                    string check = DecodeJWT();
                    if (check == "False" && author == "False")
                    {
                        var reservation = _context.Reservations.SingleOrDefault(x => x.ReservationId == id);
                        var table = _context.Tables.SingleOrDefault(x => x.TableId == reservation.TableId);
                        if (reservation == null)
                        {
                            return NotFound("Cant find reservation !!");
                        }
                        //Update
                        if (id != reservation.ReservationId)
                        {
                            return BadRequest("Invalid reservation Id!!");
                        }
                        if (reservation.Status == Models.Enum.Status.OnGoing || reservation.Status == Models.Enum.Status.Cancel || reservation.Status == Models.Enum.Status.CheckOut)
                        {
                            return BadRequest("Can't use this function because reservation status is not Pending!!");
                        }
                        else
                        {
                            table.Status = false;
                            table.UpdatedAt = DateTime.Now;
                            reservation.Status = Models.Enum.Status.OnGoing;
                            var tracker = _context.Attach(table);
                            var tracker1 = _context.Attach(reservation);
                            tracker1.State = EntityState.Modified;
                            tracker.State = EntityState.Modified;
                            _context.SaveChanges();

                            ViewStaffReservation list = _mapper.Map<ViewStaffReservation>(reservation);
                            return Ok(new
                            {
                                Success = true,
                                data = list
                            });
                        }
                    }
                    else if (check == "True")
                    {
                        return BadRequest("You have been banned, you cant use this function!");
                    }
                    else
                    {
                        return BadRequest("You are not allowed to use this function!");
                    }

                }
            }
            catch
            {
                return BadRequest("Error!");
            }
        }
        [HttpPut("Cancel/{id}")]
        public IActionResult Cancel(int id)
        {
            try
            {
                //LINQ Query
                var stream = UserController.currentJWT;
                if (stream == null)
                {
                    return BadRequest("You need to Login to use this function!!");
                }
                else
                {
                    string author = Authorize();
                    string check = DecodeJWT();
                    if (check == "False" && author == "False")
                    {
                        var reservation = _context.Reservations.SingleOrDefault(x => x.ReservationId == id);
                        var table = _context.Tables.SingleOrDefault(x => x.TableId == reservation.TableId);
                        if (reservation == null)
                        {
                            return NotFound("Cant find reservation !!");
                        }
                        //Update
                        if (id != reservation.ReservationId)
                        {
                            return BadRequest("Invalid reservation Id!!");
                        }
                        if (reservation.Status == Models.Enum.Status.OnGoing || reservation.Status == Models.Enum.Status.Cancel || reservation.Status == Models.Enum.Status.CheckOut)
                        {
                            return BadRequest("Can't use this function because reservation status is not Pending !!");
                        }
                        else
                        {
                            table.Status = true;
                            table.UpdatedAt = DateTime.Now;
                            reservation.Status = Models.Enum.Status.Cancel;
                            var tracker = _context.Attach(table);
                            var tracker1 = _context.Attach(reservation);
                            tracker1.State = EntityState.Modified;
                            tracker.State = EntityState.Modified;
                            _context.SaveChanges();

                            ViewStaffReservation list = _mapper.Map<ViewStaffReservation>(reservation);
                            return Ok(new
                            {
                                Success = true,
                                data = list
                            });
                        }
                    }
                    else if (check == "True")
                    {
                        return BadRequest("You have been banned, you cant use this function!");
                    }
                    else
                    {
                        return BadRequest("You are not allowed to use this function!");
                    }

                }
            }
            catch
            {
                return BadRequest("Error!");
            }
        }
        [HttpGet("GetAllCustomers")]
        public IActionResult GetAllCustomers()
        {
            var stream = UserController.currentJWT;
            if (stream == null)
            {
                return BadRequest("You need to Login to use this function!!");
            }
            else
            {
                string author = Authorize();
                string check = DecodeJWT();
                if (check == "False" && author == "False")
                {
                    var customerList = _context.Customers.ToList();
                    List<ViewCustomers> list = _mapper.Map<List<ViewCustomers>>(customerList);
                    return Ok(list);
                }
                else if (check == "True")
                {
                    return BadRequest("You have been banned, you cant use this function!");
                }
                else
                {
                    return BadRequest("You are not allowed to use this function!");
                }
            }
        }

        //METHOD GET: lấy thành phần theo id
        [HttpGet("GetCustomer/{id}")]
        public IActionResult GetCustomer(int id)
        {
            //LINQ query
            try
            {
                var stream = UserController.currentJWT;
                if (stream == null)
                {
                    return BadRequest("You need to Login to use this function!!");
                }
                else
                {
                    string check = DecodeJWT();
                    string author = Authorize();
                    if (check == "False" && author == "False")
                    {
                        var customer = _context.Customers.SingleOrDefault(x => x.CustomerId == id);
                        if (customer == null)
                        {
                            return NotFound();
                        }
                        ViewCustomers list = _mapper.Map<ViewCustomers>(customer);
                        return Ok(list);
                    }
                    else if (check == "True")
                    {
                        return BadRequest("You have been banned, you cant use this function!");
                    }
                    else
                    {
                        return BadRequest("You are not allowed to use this function!");
                    }
                }
            }
            catch
            {
                return BadRequest("Error!");
            }

        }


        //METHOD PUT: Cập nhật (có truyền vào id)
        [HttpPut("UpdateCustomer/{id}")]
        public IActionResult UpdateCustomer(int id, CustomersGetAddDeleteView model)
        {
            try
            {
                //LINQ Query
                var stream = UserController.currentJWT;
                if (stream == null)
                {
                    return BadRequest("You need to Login to use this function!!");
                }
                else
                {
                    string author = Authorize();
                    string check = DecodeJWT();
                    if (check == "False" && author == "False")
                    {
                        var customer = _context.Customers.SingleOrDefault(x => x.CustomerId == id);
                        if (customer == null)
                        {
                            return NotFound("Cant find customer id!!");
                        }
                        if (id != customer.CustomerId)
                        {
                            return BadRequest("Invalid Id!!");
                        }
                        else
                        {
                            _mapper.Map(model, customer);
                            customer.UpdatedAt = DateTime.Now;
                            var tracker = _context.Attach(customer);
                            tracker.State = EntityState.Modified;
                            _context.SaveChanges();
                            ViewCustomers list = _mapper.Map<ViewCustomers>(customer);
                            return Ok(new
                            {
                                Success = true,
                                data = list
                            });
                        }
                    }
                    else if (check == "True")
                    {
                        return BadRequest("You have been banned, you cant use this function!");
                    }
                    else
                    {
                        return BadRequest("You are not allowed to use this function!");
                    }

                }
            }
            catch
            {
                return BadRequest("Error!");
            }
        }

        //METHOD DELETE: Xóa (có truyền vào id)
        [HttpDelete("RemoveCustomer/{id}")]
        public IActionResult RemoveCustomer(int id)
        {
            try
            {
                //LINQ Query
                var stream = UserController.currentJWT;
                if (stream == null)
                {
                    return BadRequest("You need to Login to use this function!!");
                }
                else
                {
                    string author = Authorize();
                    string check = DecodeJWT();
                    if (check == "False" && author == "False")
                    {
                        var reservation = _context.Customers.SingleOrDefault(x => x.CustomerId == id);
                        if (reservation == null)
                        {
                            return NotFound();
                        }
                        //Remove
                        _context.Remove(reservation);
                        _context.SaveChanges();
                        return Ok(new
                        {
                            Success = true
                        });

                    }
                    else if (check == "True")
                    {
                        return BadRequest("You have been banned, you cant use this function!");
                    }
                    else
                    {
                        return BadRequest("You are not allowed to use this function!");
                    }
                }
            }
            catch
            {
                return BadRequest("Error!");
            }
        }
        [HttpGet]
        public static string DecodeJWT()
        {
            var stream = UserController.currentJWT;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            var jti = tokenS.Claims.First(claim => claim.Type == "IsDeleted").Value.ToString();
            return jti;
        }
        [HttpGet]
        public static string Authorize()
        {
            var stream = UserController.currentJWT;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            var jti = tokenS.Claims.First(claim => claim.Type == "IsAdmin").Value.ToString();
            return jti;
        }
    }
}
