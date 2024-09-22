using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Net.Mail;
using TechWiz.Data;
using TechWiz.Models;

namespace TechWiz.Controllers
{
    [Authorize(Roles = "client, admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<User> userManager;
        private readonly ILogger<UserController> logger;

        public UserController(ApplicationDbContext dbContext, UserManager<User> userManager, ILogger<UserController> logger)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Home()
        {
            var user = await userManager.GetUserAsync(User);
            var trips = await dbContext.Trips
                .Where(t => t.user.Id == user.Id)
        .Include(t => t.destination) 
        .Include(t => t.destination.Galleries) 
        .ToListAsync();


            var tripViewModels = trips.Select(t => new
            {
                t.Id,
                t.TripName,
                t.Budget,
                t.StartTime,
                t.EndTime,
                DestinationName = t.destination.DesName,
                FirstGalleryImage = t.destination.Galleries.FirstOrDefault().link_pic
            }).ToList();

            ViewBag.Trips = tripViewModels;


            return View();
        }
        [HttpGet]
        public IActionResult PlanTrip()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AboutUs()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Destination()
        {
            var destinations = await dbContext.Destinations.ToListAsync();
            ViewBag.Destinations = destinations;

            var firstGalleryImages = await dbContext.Galleries
                .GroupBy(g => g.destination.Id) 
                .Select(g => g.FirstOrDefault()) 
                .ToListAsync();

            ViewBag.FirstGalleryImages = firstGalleryImages;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DetailGallery(int id)
        {
            var destination = await dbContext.Destinations
                .Include(d => d.Galleries)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (destination == null)
            {
                return NotFound();
            }
            return View(destination);
        }
        [HttpGet]
        public async Task<IActionResult> DetailPlan(int id)
        {
			var trip = await dbContext.Trips
		    .Include(t => t.destination)
            .ThenInclude(d => d.Galleries)
            .Include(t => t.user) 
		    .FirstOrDefaultAsync(t => t.Id == id);

			var currency = await dbContext.Currencies.ToListAsync();
			ViewBag.Currency = currency;

			ViewBag.Trip = trip;
            ViewBag.DestinationName = trip.destination.DesName;
            ViewBag.LinkGPS = trip.destination.link_GPS; 
            ViewBag.FirstGalleryImage = trip.destination.Galleries.FirstOrDefault()?.link_pic;
            var categories = await dbContext.Categories
                .Where(c => c.trip.Id == id) 
                .ToListAsync();

            ViewBag.Categories = categories;
            var items = await dbContext.Itemss
                .Where(i => i.trip.Id == id) 
                .ToListAsync();

            ViewBag.Items = items;
            if (trip != null)
            {
                ViewBag.StartDate = trip.StartTime?.ToString("yyyy-MM-dd") ?? string.Empty;
                ViewBag.EndDate = trip.EndTime?.ToString("yyyy-MM-dd") ?? string.Empty;
            }

            return View(trip);
        }
        public async Task<double> GetExchangeRate(string currencyCode)
        {
            var currency = await dbContext.Currencies
                .FirstOrDefaultAsync(c => c.CurrencyCode == currencyCode);

            return currency?.ExchangeRate ?? 1; 
        }

        [HttpGet]
        public JsonResult Search(string query)
        {
            var suggestions = dbContext.Destinations
                .Where(d => d.DesName.Contains(query))
                .Select(d => new { d.Id,d.DesName })
                .ToList();

            return Json(suggestions);
        }
        [HttpPost]
        public async Task<IActionResult> AddTrip(AddTripViewModel viewModel)
        {
            // Lấy thông tin người dùng hiện tại từ Identity
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(); // Nếu người dùng không đăng nhập, trả về lỗi 401
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    logger.LogError(error.ErrorMessage);
                }
            }

            logger.LogInformation("AddTrip called with: DestinationId={DestinationId}, Budget={Budget}, StartTime={StartTime}, EndTime={EndTime}",
        viewModel.DestinationId, viewModel.Budget, viewModel.StartTime, viewModel.EndTime);
            // Lấy thông tin Destination từ cơ sở dữ liệu dựa trên ID
            var destination = await dbContext.Destinations.FindAsync(viewModel.DestinationId);

            string Namedes = destination.DesName;
            // Tạo trip mới
            var trip = new Trip
            {
                TripName = "Trip to "+Namedes,
                Budget = viewModel.Budget.Value,
                StartTime = viewModel.StartTime.Value,
                EndTime = viewModel.EndTime.Value,
                destination = destination,
                user = user // Gắn người dùng hiện tại từ Identity
            };

            // Thêm trip vào cơ sở dữ liệu
            dbContext.Trips.Add(trip);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Home");
        }
        [HttpPost]
        public async Task<IActionResult> AddItem(string? ItemName,string? Note, float? Budget,DateTime? Date,int? CategoryId, int tripId)
        {
            var trip = await dbContext.Trips.FindAsync(tripId);
            var item = new Items
            {
                ItemName = ItemName,
                Note = Note,
                Budget = Budget,
                Date = Date ?? DateTime.Now, 
                category = await dbContext.Categories.FindAsync(CategoryId),
                trip = await dbContext.Trips.FindAsync(tripId) 
            };

            // Kiểm tra xem trip có tồn tại không
            if (item.trip == null)
            {
                return NotFound("Trip not found.");
            }
            if (item.category != null)
            {
                // If there is a category, deduct from its budget
                item.category.Budget -= Budget ?? 0;

                // Update the category in the database
                dbContext.Categories.Update(item.category);
            }
            else
            {
                // If no category, deduct from the trip's budget
                trip.Budget -= Budget ?? 0;

                // Update the trip in the database
                dbContext.Trips.Update(trip);
            }

            // Add the item to the database
            dbContext.Itemss.Add(item);
            await dbContext.SaveChangesAsync();

            // Lấy URL để trở về
            var referer = Request.Headers["Referer"].ToString();
            return string.IsNullOrEmpty(referer) ? RedirectToAction("Home", "User") : Redirect(referer);
        }
        [HttpPost]
        public IActionResult DeleteItem(int itemId)
        {
            var item = dbContext.Itemss.Include(i => i.category).Include(i => i.trip).FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                // If the item has a category, add its budget back to the category
                if (item.category != null)
                {
                    item.category.Budget += item.Budget;
                    dbContext.Categories.Update(item.category);
                }
                else if (item.trip != null)
                {
                    // If no category, add the budget back to the trip
                    item.trip.Budget += item.Budget;
                    dbContext.Trips.Update(item.trip);
                }

                // Remove the item from the database
                dbContext.Itemss.Remove(item);
                dbContext.SaveChanges();
            }

            // Redirect back to the previous page
            var referer = Request.Headers["Referer"].ToString();
            return string.IsNullOrEmpty(referer) ? RedirectToAction("Home", "User") : Redirect(referer);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string cateName, string? note, double? budget, int tripId)
        {
            if (string.IsNullOrWhiteSpace(cateName))
            {
                return BadRequest("Category name is required.");
            }
            var trip = await dbContext.Trips.FindAsync(tripId);
            var category = new Category
            {
                CateName = cateName,
                Note = note,
                Budget = budget,
                trip = await dbContext.Trips.FindAsync(tripId)
            };

            if (category.trip == null)
            {
                return NotFound("Trip not found.");
            }

            dbContext.Categories.Add(category);
            if (budget.HasValue)
            {
                trip.Budget -= budget.Value;
                dbContext.Trips.Update(trip);
            }
            await dbContext.SaveChangesAsync();
            var referer = Request.Headers["Referer"].ToString();
            return string.IsNullOrEmpty(referer) ? RedirectToAction("Home", "User") : Redirect(referer);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var category = dbContext.Categories.Include(c => c.trip).FirstOrDefault(c => c.Id == categoryId);

            if (category != null)
            {
                // Cộng ngân sách của danh mục vào ngân sách của chuyến đi
                if (category.trip != null)
                {
                    category.trip.Budget += category.Budget; // Cộng ngân sách của danh mục vào ngân sách của chuyến đi
                }

                // Xóa danh mục
                dbContext.Categories.Remove(category);
                dbContext.SaveChanges();
            }
            var referer = Request.Headers["Referer"].ToString();
            return string.IsNullOrEmpty(referer) ? RedirectToAction("Home") : Redirect(referer);
        }
        [HttpGet]
        public IActionResult GetValidDestinations()
        {
            var validDestinations = dbContext.Destinations
                .Select(d => new { id = d.Id, desName = d.DesName })
                .ToList();

            if (!validDestinations.Any())
            {
                return NotFound("No valid destinations found."); 
            }

            return Json(validDestinations);
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(int id, string cateName, string? note, double? budget)
        {
            var category = await dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(cateName))
            {
                return BadRequest("Category name is required.");
            }

            category.CateName = cateName;
            category.Note = note;
            category.Budget = budget;

            await dbContext.SaveChangesAsync();
            var referer = Request.Headers["Referer"].ToString();
            return string.IsNullOrEmpty(referer) ? RedirectToAction("Home") : Redirect(referer);
        }


        [HttpPost]
        public IActionResult ContactUs(SendMailDto sendMailDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("PlanTrip");
            }
            try
            {
                MailMessage mail = new MailMessage();
                string add = sendMailDto.MailTo.ToString();
                mail.From = new MailAddress(add);
                mail.To.Add("trungthanh22624@gmail.com");
                mail.Subject = "Contact";
                mail.IsBodyHtml = true;
                string content = "Name: " + sendMailDto.Name;
                content += "<br/> Mail : " + sendMailDto.MailTo;
                content += "<br/> Phone : " + sendMailDto.Phone;
                content += "<br/> Message : " + sendMailDto.Message;
                mail.Body = content;

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                NetworkCredential networkCredential = new NetworkCredential("trungthanhcva2206@gmail.com", "wwid tqke jmwf ehgf");
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = networkCredential;
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.Send(mail);
                ModelState.Clear();
            }
            catch (Exception ex)
            {

                throw;
            }
            return RedirectToAction("Home");
        }
    }



    
}
