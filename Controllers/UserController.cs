using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Management.Smo;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TableReservation.Models;
using TableReservation.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace TableReservation.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _con;
        public static string? currentJWT;
        public UserController(AppDbContext context, IConfiguration con)
        {
            _context = context;
            _con = con;
        }

        private string CreateToken(Users user)
        {
           
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("Name",user.Name),
                new Claim("ID",user.UserId.ToString()),
                new Claim("IsAdmin",user.isAdmin.ToString()),
                new Claim("IsDeleted",user.isDeleted.ToString()),
                new Claim("LastLogin",user.LastLogin.ToString()),
                new Claim("TokenID",Guid.NewGuid().ToString()),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_con.GetSection("AppSettings:Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(2), signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        [HttpPost("Login")]
        public IActionResult Login(ViewLogin lo)
        {
            var check = _context.Users.SingleOrDefault(x => x.Email.Equals(lo.Email));
          
            if (check != null && check.Password==lo.Password)
            {
                check.LastLogin = DateTime.Now;
                currentJWT = CreateToken(check);
                var tracker = _context.Attach(check);
                tracker.State = EntityState.Modified;
                _context.SaveChanges();
                return Ok(new
                {
                    Success = true,
                    Message = "Login success",
                    Data = CreateToken(check)
                });
            }
            return BadRequest("Ivalid Email or Password!!");
        }
    }
}
