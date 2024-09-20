using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using TechWiz.Models;

namespace TechWiz.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Home()
        {
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
