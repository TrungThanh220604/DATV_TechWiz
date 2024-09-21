<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWiz.Data;
using TechWiz.Models;
=======
﻿using Microsoft.AspNetCore.Mvc;
>>>>>>> main

namespace TechWiz.Controllers
{
    public class AdminController : Controller
    {
<<<<<<< HEAD
		private readonly UserManager<User> _userManager;
		private readonly ApplicationDbContext _context;

		public AdminController(UserManager<User> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public class DashboardViewModel
		{
			public int UsersCount { get; set; }
			public int DestinationsCount { get; set; }
			public int TripsCount { get; set; }
			public int CurrencyCount { get; set; }
			public List<User> RecentUsers { get; set; }
			public List<List<string>> RecentUserRoles { get; set; } 
			public List<Trip> RecentTrips { get; set; }
		}



		public async Task<IActionResult> DashBoard()
		{
			var usersCount = _context.Users.Count();
			var destinationsCount = _context.Destinations.Count();
			var tripsCount = _context.Trips.Count();
			var currencyCount = _context.Currencies.Count();

			var recentUsers = await _context.Users
				.OrderByDescending(u => u.Id)
				.Take(5)
				.ToListAsync();

			var recentTrips = await _context.Trips
				.Include(t => t.destination)
				.OrderByDescending(t => t.StartTime)
				.Take(5)
				.ToListAsync();

			var recentUserRoles = new List<List<string>>(); 
			foreach (var user in recentUsers)
			{
				var roles = await _userManager.GetRolesAsync(user);
				recentUserRoles.Add(roles.ToList()); 
			}

			var model = new DashboardViewModel
			{
				UsersCount = usersCount,
				DestinationsCount = destinationsCount,
				TripsCount = tripsCount,
				CurrencyCount = currencyCount,
				RecentUsers = recentUsers,
				RecentUserRoles = recentUserRoles,
				RecentTrips = recentTrips
			};

			return View(model);
		}



		public class UserViewModel
		{
			public string LinkAvatar { get; set; }
			public string UserName { get; set; }
			public string Email { get; set; }
			public string Roles { get; set; }
		}

		//List Users
		public async Task<IActionResult> Users()
		{
			var users = _userManager.Users.ToList();
			var userViewModels = new List<UserViewModel>();

			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);
				userViewModels.Add(new UserViewModel
				{
					LinkAvatar = user.link_avatar,
					UserName = user.UserName,
					Email = user.Email,
					Roles = string.Join(", ", roles) 
				});
			}

			return View(userViewModels);
		}

		//List Trips
		public async Task<IActionResult> Trips()
		{
			var trips = await _context.Trips
				.Include(t => t.destination)
				.Include(t => t.user)
				.ToListAsync();

			return View(trips);
		}

		//List Currency
		public IActionResult Currencies()
		{
			var currencies = _context.Currencies.ToList(); 
			return View(currencies);
		}

		//Add currency
		[HttpPost]
		public async Task<IActionResult> AddCurrency(Currency currency)
		{
			if (ModelState.IsValid)
			{
				currency.updateTime = DateTime.Now; 
				_context.Add(currency);
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "Currency added successfully!";
				return RedirectToAction(nameof(Currencies));
			}
			return View("Currencies", currency);
		}

		//edit currency
		[HttpPost]
		public IActionResult EditCurrency(Currency currency)
		{
			if (ModelState.IsValid)
			{
				var existingCurrency = _context.Currencies.Find(currency.Id);
				if (existingCurrency != null)
				{
					existingCurrency.CurrencyCode = currency.CurrencyCode;
					existingCurrency.CurrencyName = currency.CurrencyName;
					existingCurrency.ExchangeRate = currency.ExchangeRate;
					existingCurrency.updateTime = DateTime.Now;

					_context.Currencies.Update(existingCurrency);
					_context.SaveChanges();

					TempData["SuccessMessage"] = "Currency updated successfully!";
					return RedirectToAction("Currencies");
				}
			}

			return View("Currencies", currency);
		}

		//delete currency
		[HttpPost]
		public IActionResult DeleteCurrency(int id)
		{
			var currency = _context.Currencies.Find(id);
			if (currency != null)
			{
				var usersWithCurrency = _context.Users.Where(u => u.currency != null && u.currency.Id == id).ToList();
				foreach (var user in usersWithCurrency)
				{
					user.currency = null; 
				}

				_context.SaveChanges();

				_context.Currencies.Remove(currency);
				_context.SaveChanges();

				TempData["SuccessMessage"] = "Currency deleted successfully!";
			}
			else
			{
				return NotFound(); 
			}

			return Json(new { success = true }); 
		}

		//List destinations
		public async Task<IActionResult> Destinations()
		{
			var destinations = await _context.Destinations.ToListAsync();
			return View(destinations);
		}

		// Add destination
		[HttpPost]
		public IActionResult AddDestination(string destination, string gps)
		{
			if (ModelState.IsValid)
			{
				var newDestination = new Destination
				{
					DesName = destination,
					link_GPS = gps
				};

				_context.Destinations.Add(newDestination);
				_context.SaveChanges();
				TempData["SuccessMessage"] = "Destination Added successfully!";

				return RedirectToAction("Destinations");
			}

			return View("Destinations", _context.Destinations.ToList());
		}

		//Edit Destination
		[HttpPost]
		public IActionResult EditDestination(Destination destination)
		{
			if (ModelState.IsValid)
			{
				var existingDestination = _context.Destinations.Find(destination.Id);
				if (existingDestination != null)
				{
					existingDestination.DesName = destination.DesName;
					existingDestination.link_GPS = destination.link_GPS;

					_context.Destinations.Update(existingDestination);
					_context.SaveChanges();

					TempData["SuccessMessage"] = "Destination updated successfully!";
					return RedirectToAction("Destinations");
				}
				else
				{
					ModelState.AddModelError("", "Destination not found.");
				}
			}
			else
			{
				ModelState.AddModelError("", "Invalid input data.");
			}

			return View("Destinations", _context.Destinations.ToList()); 
		}

		//delete Destination
		[HttpPost]
		public IActionResult DeleteDestination(int id)
		{
			var destination = _context.Destinations
				.Include(d => d.Galleries) 
				.Include(d => d.Trips) 
				.FirstOrDefault(d => d.Id == id);

			if (destination != null)
			{
				foreach (var gallery in destination.Galleries)
				{
					gallery.destination = null; 
				}

				foreach (var trip in destination.Trips)
				{
					trip.destination = null; 
				}

				_context.Update(destination);
				_context.SaveChanges();

				_context.Destinations.Remove(destination);
				_context.SaveChanges();

				TempData["SuccessMessage"] = "Destination deleted successfully!";
			}
			else
			{
				TempData["ErrorMessage"] = "Destination not found.";
			}

			return Json(new { success = true });
		}

		//Desnation detail
		public IActionResult DestinationDetail(int id)
		{
			var destination = _context.Destinations
				.Include(d => d.Galleries)
				.FirstOrDefault(d => d.Id == id);

			if (destination == null)
			{
				return NotFound();
			}

			return View(destination);
		}

		//add image
		[HttpPost]
		public async Task<IActionResult> AddPhoto(int destinationId, IFormFile photo)
		{
			if (photo != null && photo.Length > 0)
			{
				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
				var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img_destination", uniqueFileName);

				using (var stream = new FileStream(path, FileMode.Create))
				{
					await photo.CopyToAsync(stream);
				}

				var gallery = new Gallery
				{
					link_pic = uniqueFileName,
					destination = await _context.Destinations.FindAsync(destinationId)
				};

				_context.Galleries.Add(gallery);
				await _context.SaveChangesAsync();

				TempData["SuccessMessage"] = "Photo added successfully!";
			}

			return RedirectToAction("DestinationDetail", new { id = destinationId });
		}


		//delete image
		[HttpPost]
		public IActionResult DeletePhoto(int photoId, int destinationId)
		{
			var gallery = _context.Galleries.FirstOrDefault(g => g.Id == photoId);
			if (gallery != null)
			{
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img_destination", gallery.link_pic);
				if (System.IO.File.Exists(filePath))
				{
					System.IO.File.Delete(filePath);
				}

				_context.Galleries.Remove(gallery);
				_context.SaveChanges();
				TempData["SuccessMessage"] = "Photo deleted successfully!";
			}
			else
			{
				TempData["ErrorMessage"] = "Photo not found!";
			}

			return RedirectToAction("DestinationDetail", new { id = destinationId });
		}





	}
=======
        public IActionResult DashBoard()
        {
            return View();
        }
    }
>>>>>>> main
}
