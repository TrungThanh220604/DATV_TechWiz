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
            var trips = await dbContext.Trips
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
