using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TableReservation.Models;
using TableReservation.ViewModels;

namespace TableReservation.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _con;
        private readonly IMapper _mapper;
        public ReviewController(AppDbContext context, IConfiguration con, IMapper mapper)
        {
            _context = context;
            _con = con;
            _mapper = mapper;
        }
        [HttpGet("GetAllReviews")]
        public IActionResult GetAllReviews()
        {

            var reviewList = _context.Reviews.ToList();
            List<ViewReviews> list = _mapper.Map<List<ViewReviews>>(reviewList);
            return Ok(list);
        }




        //METHOD GET: lấy thành phần theo id
        [HttpGet("GetReview/{id}")]
        public IActionResult GetReview(int customerId, int restaurantId)
        {
            try
            {
                var review = _context.Reviews.SingleOrDefault(x => x.CustomerId == customerId && x.RestaurantId == restaurantId);
                if (review == null)
                {
                    return NotFound();
                }
                ViewReviews list = _mapper.Map<ViewReviews>(review);
                return Ok(list);


            }
            catch (Exception)
            {
                return BadRequest("Error!");
            }

        }



        //METHOD POST: Create tạo mới
        [HttpPost("CreateReview")]
        public IActionResult CreateReview(int customerId, int restaurantId, ViewReviews model)
        {
            try
            {

                var review = new Reviews();
                _mapper.Map(model, review);
                review.CustomerId = customerId;
                review.RestaurantId = restaurantId;
                review.CreatedAt = DateTime.Now;
                review.UpdatedAt = DateTime.Now;
                _context.Add(review);
                _context.SaveChanges();
                ViewReviews list = _mapper.Map<ViewReviews>(review);
                return Ok(new
                {
                    Success = true,
                    Data = review
                });
            }
            catch (Exception)
            {
                return BadRequest("Error!");
            }

        }

        //METHOD PUT: Cập nhật (có truyền vào id)
        [HttpPut("UpdateReview/{id}")]
        public IActionResult UpdateReview(int customerId, int restaurantId, ViewReviews model)
        {
            try
            {

                var review = _context.Reviews.SingleOrDefault(x => x.CustomerId == customerId && x.RestaurantId == restaurantId);
                if (review == null)
                {
                    return NotFound();
                }
                else
                {
                    _mapper.Map(model, review);
                    review.UpdatedAt = DateTime.Now;
                    var tracker = _context.Attach(review);
                    tracker.State = EntityState.Modified;
                    _context.SaveChanges();
                    ViewReviews list = _mapper.Map<ViewReviews>(review);
                    return Ok(new
                    {
                        Success = true,
                        Data = list
                    });
                }




            }
            catch
            {
                return BadRequest("Error!");
            }
        }

        //METHOD DELETE: Xóa (có truyền vào id)
        [HttpDelete("DeleteReview/{id}")]
        public IActionResult RemoveReview(int customerId, int restaurantId)
        {
            try
            {
                var review = _context.Reviews.SingleOrDefault(x => x.CustomerId == customerId && x.RestaurantId == restaurantId);
                if (review == null)
                {
                    return NotFound();
                }
                //Remove
                _context.Remove(review);
                _context.SaveChanges();
                ViewReviews list = _mapper.Map<ViewReviews>(review);
                return Ok(new
                {
                    Success = true,
                    Data = list
                });

            }
            catch
            {
                return BadRequest("Error!");
            }
        }
    }
}
