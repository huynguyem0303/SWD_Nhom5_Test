using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using TableReservation.Models;
using TableReservation.ViewModels;

namespace TableReservation.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _con;
        private readonly IMapper _mapper;
        public AdminController(AppDbContext context, IConfiguration con, IMapper mapper)
        {
            _context = context;
            _con = con;
            _mapper = mapper;
        }
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
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
                if (check == "False" && author == "True")
                {
                    var userList = _context.Users.ToList();
                    return Ok(userList);
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
        [HttpGet("GetUser/{id}")]
        public IActionResult GetUserById(int id)
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
                    if (check == "False" && author == "True")
                    {
                        var user = _context.Users.SingleOrDefault(x => x.UserId == id);
                        if (user == null)
                        {
                            return NotFound();
                        }
                        return Ok(user);
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
        [HttpPost("CreateUser")]
        public IActionResult CreateUser(ViewUsers model)
        {
            try
            {
                var stream = UserController.currentJWT;
                if (stream == null)
                {
                    return BadRequest("You need to Login to use this function!!");
                }
                else
                {
                    var checkemail = _context.Users.ToList().Where(p => p.Email == model.Email).FirstOrDefault();
                    string check = DecodeJWT();
                    string author = Authorize();
                    if (check == "False" && checkemail == null && author == "True")
                    {
                        var user = new Users()
                        {
                            Email = model.Email,
                            Password = model.Password,
                            Name = model.Name,
                            isAdmin = false,
                            isDeleted = false,
                            LastLogin = DateTime.Now
                        };
                        _context.Add(user);
                        _context.SaveChanges();
                        return Ok(new
                        {
                            Success = true,
                            Data = user
                        });
                    }
                    else if (checkemail != null)
                    {
                        return BadRequest("Duplicate Email!");
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
            catch (Exception)
            {
                return BadRequest("Error!");
            }

        }

        //METHOD PUT: Cập nhật (có truyền vào id)
        [HttpPut("UpdateUser/{id}")]
        public IActionResult UpdateUser(int id, ViewUsers userUpdate)
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
                    if (check == "False" && author == "True")
                    {
                        var user = _context.Users.SingleOrDefault(x => x.UserId == id);
                        if (user == null)
                        {
                            return NotFound();
                        }
                        //Update
                        if (id != user.UserId)
                        {
                            return BadRequest();
                        }
                        else
                        {
                            user.Email = userUpdate.Email;
                            user.Password = userUpdate.Password;
                            user.Name = userUpdate.Name;
                            user.isAdmin = userUpdate.isAdmin;
                            var tracker = _context.Attach(user);
                            tracker.State = EntityState.Modified;
                            _context.SaveChanges();
                            return Ok(new
                            {
                                Success = true,
                                Data = user
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
        [HttpDelete("DeleteUser/{id}")]
        public IActionResult RemoveUser(int id)
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
                    if (check == "False" && author == "True")
                    {
                        var user = _context.Users.SingleOrDefault(x => x.UserId == id);
                        if (user == null)
                        {
                            return NotFound();
                        }
                        //Remove
                        user.isDeleted = true;
                        var tracker = _context.Attach(user);
                        tracker.State = EntityState.Modified;
                        _context.SaveChanges();
                        return Ok(new
                        {
                            Success = true,
                            Data = user
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
        [HttpGet("GetAllMenus")]
        public IActionResult GetAllMenus()
        {
            try { 
                var menuList = _context.Menus.ToList();
                List<ViewMenus> list = _mapper.Map<List<ViewMenus>>(menuList);
                return Ok(list);
            }
            catch (Exception)
            {
                return BadRequest("Error!");
            }
        }


        //METHOD GET: lấy thành phần theo id
        [HttpGet("GetMenu/{id}")]
        public IActionResult GetMenuById(int id)
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
                    if (check == "False" && author == "True")
                    {
                        var menu = _context.Menus.SingleOrDefault(x => x.MenuId == id);
                        if (menu == null)
                        {
                            return NotFound();
                        }
                        ViewMenus list = _mapper.Map<ViewMenus>(menu);
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
        [HttpPost("CreateMenu")]
        public IActionResult CreateMenu(int restaurantId, MenuGetAddEditView model)
        {
            try
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
                    if (check == "False" && author == "True")
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
                        else
                        {
                            var menu = new Menus();
                            _mapper.Map(model, menu);
                            menu.RestaurantId = restaurantId;
                            menu.CreatedAt = DateTime.Now;
                            menu.UpdatedAt = DateTime.Now;
                            _context.Add(menu);
                            _context.SaveChanges();
                            ViewMenus list = _mapper.Map<ViewMenus>(menu);
                            return Ok(new
                            {
                                Success = true,
                                Data = list
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
            catch (Exception)
            {
                return BadRequest("Error!");
            }

        }

        //METHOD PUT: Cập nhật (có truyền vào id)
        [HttpPut("UpdateMenu/{id}")]
        public IActionResult UpdateMenu(int id, MenuGetAddEditView model)
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
                    if (check == "False" && author == "True")
                    {
                        var menu = _context.Menus.SingleOrDefault(x => x.MenuId == id);
                        if (menu == null)
                        {
                            return NotFound("Cant find menu id!!");
                        }
                        //Update
                        if (id != menu.MenuId)
                        {
                            return BadRequest("Invalid Id!!");
                        }
                        else
                        {
                            _mapper.Map(model, menu);
                            menu.UpdatedAt = DateTime.Now;
                            var tracker = _context.Attach(menu);
                            tracker.State = EntityState.Modified;
                            _context.SaveChanges();
                            ViewMenus list = _mapper.Map<ViewMenus>(menu);
                            return Ok(new
                            {
                                Success = true,
                                Data = list
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
        [HttpDelete("DeleteMenu/{id}")]
        public IActionResult RemoveMenu(int id)
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
                    if (check == "False" && author == "True")
                    {
                        var menu = _context.Menus.SingleOrDefault(x => x.MenuId == id);
                        if (menu == null)
                        {
                            return NotFound();
                        }
                        //Remove
                        menu.IsDeleted = true;
                        menu.UpdatedAt = DateTime.Now;
                        var tracker = _context.Attach(menu);
                        tracker.State = EntityState.Modified;
                        _context.SaveChanges();
                        ViewMenus list = _mapper.Map<ViewMenus>(menu);
                        return Ok(new
                        {
                            Success = true,
                            Data = list
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

        [HttpGet("GetAllTables")]
        public IActionResult GetAllTables()
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
                if (check == "False")
                {
                    var tableList = _context.Tables.ToList();
                    List<ViewTables> list = _mapper.Map<List<ViewTables>>(tableList);
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
        [HttpGet("GetTable/{id}")]
        public IActionResult GetTableById(int id)
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
                    string author = Authorize();
                    string check = DecodeJWT();

                    if (check == "False" && author == "True")
                    {
                        var table = _context.Tables.SingleOrDefault(x => x.TableId == id);
                        ViewTables list = _mapper.Map<ViewTables>(table);
                        if (table == null)
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
        [HttpPost("CreateTable")]
        public IActionResult CreateTable(int restaurantId, TableGetAddEditView model)
        {
            try
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
                    if (check == "False" && author == "True")
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
                        else
                        {
                            var table = new Tables();
                            _mapper.Map(model, table);
                            table.RestaurantId = restaurantId;
                            table.CreatedAt = DateTime.Now;
                            table.UpdatedAt = DateTime.Now;
                            _context.Add(table);
                            _context.SaveChanges();
                            ViewTables list = _mapper.Map<ViewTables>(table);
                            return Ok(new
                            {
                                Success = true,
                                Data = list
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
            catch (Exception)
            {
                return BadRequest("Error!");
            }

        }

        //METHOD PUT: Cập nhật (có truyền vào id)
        [HttpPut("UpdateTable/{id}")]
        public IActionResult UpdateTable(int id, TableGetAddEditView model)
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
                    if (check == "False" && author == "True")
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

        //METHOD DELETE: Xóa (có truyền vào id)
        [HttpDelete("DeleteTable/{id}")]
        public IActionResult RemoveTable(int id)
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
                    if (check == "False" && author == "True")
                    {
                        var table = _context.Tables.SingleOrDefault(x => x.TableId == id);
                        if (table == null)
                        {
                            return NotFound();
                        }
                        //Remove
                        _context.Remove(table);
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
        [HttpGet("GetAllRestaurants")]
        public IActionResult GetAllRestaurants()
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
                if (check == "False" && author == "True")
                {
                    var list = _context.Restaurants.ToList();
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
        [HttpGet("GetRestaurant/{id}")]
        public IActionResult GetRestaurantById(int id)
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

                    if (check == "False" && author == "True")
                    {
                        var restaurant = _context.Restaurants.SingleOrDefault(x => x.RestaurantId == id);
                        if (restaurant == null)
                        {
                            return NotFound();
                        }
                        return Ok(restaurant);
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
        [HttpPost("CreateRestaurant")]
        public IActionResult CreateRestaurant(ViewRestaurants model)
        {
            try
            {
                var stream = UserController.currentJWT;
                if (stream == null)
                {
                    return BadRequest("You need to Login to use this function!!");
                }
                else
                {
                    var checkPhone = _context.Restaurants.ToList().Where(p => p.RestaurantPhone == model.RestaurantPhone).FirstOrDefault();
                    string author = Authorize();
                    string check = DecodeJWT();
                    if (check == "False" && checkPhone == null && author == "True")
                    {
                        var restaurant = new Restaurants()
                        {
                            RestaurantName = model.RestaurantName,
                            RestaurantPhone = model.RestaurantPhone,
                            RestaurantAddress = model.RestaurantAddress,
                            OpenHours = model.OpenHours,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _context.Add(restaurant);
                        _context.SaveChanges();
                        return Ok(new
                        {
                            Success = true,
                            Data = restaurant
                        });
                    }
                    else if (checkPhone != null)
                    {
                        return BadRequest("Duplicate Phone!");
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
            catch (Exception)
            {
                return BadRequest("Error!");
            }

        }
        //METHOD PUT: Cập nhật (có truyền vào id)
        [HttpPut("UpdateRestaurant/{id}")]
        public IActionResult UpdateRestaurant(int id, ViewRestaurants model)
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
                    if (check == "False" && author == "True")
                    {
                        var restaurant = _context.Restaurants.SingleOrDefault(x => x.RestaurantId == id);
                        if (restaurant == null)
                        {
                            return NotFound("Cant find menu id!!");
                        }
                        //Update
                        if (id != restaurant.RestaurantId)
                        {
                            return BadRequest("Invalid Id!!");
                        }
                        else
                        {
                            restaurant.RestaurantPhone = model.RestaurantPhone;
                            restaurant.OpenHours = model.OpenHours;
                            restaurant.RestaurantName = model.RestaurantName;
                            restaurant.RestaurantAddress = model.RestaurantAddress;
                            restaurant.UpdatedAt = DateTime.Now;
                            var tracker = _context.Attach(restaurant);
                            tracker.State = EntityState.Modified;
                            _context.SaveChanges();
                            return Ok(new
                            {
                                Success = true,
                                data = restaurant
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
        [HttpDelete("DeleteRestaurant/{id}")]
        public IActionResult DeleteRestaurant(int id)
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
                    if (check == "False" && author == "True")
                    {
                        var restaurant = _context.Restaurants.SingleOrDefault(x => x.RestaurantId == id);
                        if (restaurant == null)
                        {
                            return NotFound();
                        }
                        //Remove
                        restaurant.Status = false;
                        var tracker = _context.Attach(restaurant);
                        tracker.State = EntityState.Modified;
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
        //=============
        // Decode JWT
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
