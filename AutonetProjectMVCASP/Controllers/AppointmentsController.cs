using AutonetProjectMVCASP.Data;
using Microsoft.AspNetCore.Mvc;
using AutonetProjectMVCASP.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using AspNetCoreHero.ToastNotification.Abstractions;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc.Rendering;

using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using System.IO;
using OfficeOpenXml;
using MailKit.Security;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using System.Configuration;
using System.Net;
using Hangfire;
using SendGrid.Helpers.Mail;
using System.Web;


namespace AutonetProjectMVCASP.Controllers
{




    public class AppointmentsData
    {
        public string Location { get; set; }
        public IEnumerable<Appointments> Obj { get; set; }
        public AppointmentsData(string location, IEnumerable<Appointments> obj)
        {
            Location = location;
            Obj = obj;
        }

        public AppointmentsData(string location, ApplicationDbContext db)
        {
            Location = location;
            Obj = db.Appointments.Where(a => a.Location == location).ToList();
        }
    }

    public class LocDateData
    {
        public string Location { get; set; } = "default";
        public DateTime Date { get; set; } = DateTime.Now;

        public LocDateData(string location, DateTime date)
        {
            Location = location;
            Date = date;
        }

        public LocDateData()
        {

        }
    }

    //From now on always use static methods with hangfire, this piece of garbage tries to instantiate stuff it just can't. After 2 hours of pure hell, it finally works
    public static class EmailMethods
    {
        //also you can't use IConfiguration in hangfire, I baked all the data in for the schedule
        public static async Task SendEmailAsync(string to, string subject, string body, IConfiguration configuration)
        {
            var _smtpSettings = new SmtpSettings();
            configuration.GetSection("SmtpSettings").Bind(_smtpSettings);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("carservicemanagerproject@gmail.com", "carservicemanagerproject@gmail.com"));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.Auto); // Connect to the SMTP server
                await client.AuthenticateAsync("carservicemanagerproject@gmail.com", "krwcxbwjwoozckvu"); // Authenticate if required
                await client.SendAsync(message); // Send the message
                await client.DisconnectAsync(true); // Disconnect from the SMTP server
            }
        }

        //used specifically with hangfire, always use this with that
        public static async Task SendEmailAsyncFixed(string to, string subject, string body)
        {
           
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("carservicemanagerproject@gmail.com", "carservicemanagerproject@gmail.com"));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;
                await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.Auto); // Connect to the SMTP server
                await client.AuthenticateAsync("carservicemanagerproject@gmail.com", "krwcxbwjwoozckvu"); // Authenticate if required
                await client.SendAsync(message); // Send the message
                await client.DisconnectAsync(true); // Disconnect from the SMTP server
            }
        }
    }


    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly INotyfService _toastNotification;
        private readonly SmtpSettings _smtpSettings;
        private readonly IConfiguration _configuration;
        //private readonly EmailController _emailController;


        public AppointmentsController(ApplicationDbContext db,
            ILogger<AppointmentsController> logger,
            INotyfService toastNotification,
            IConfiguration configuration)
        {
            _db = db;
            _logger = logger;
            _toastNotification = toastNotification;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            _configuration = configuration;

            _smtpSettings = new SmtpSettings();
            configuration.GetSection("SmtpSettings").Bind(_smtpSettings);

            //_emailController = emailController;
        }


        //Location selection page for appointments
        [HttpGet]
        public IActionResult Select()
        {
            //remove old appointments
            _db.Appointments.RemoveRange(_db.Appointments.Where(a => a.Time < DateTime.Now));
            _db.SaveChanges();

            //create list of locations
            IEnumerable<Models.Locations> loc = _db.Locations;

            //testing for hangfire
            //if ((User != null) && (User.Identity.IsAuthenticated))
            //{
            //    var mailSubject = $"Welcome " + User.Identity.Name;
            //    var mailBody = $"<p>Thanks for join us!</p>";
            //    var jobId = BackgroundJob.Schedule(() => EmailMethods.SendEmailAsyncFixed(User.Identity.Name, mailSubject, mailBody), TimeSpan.FromSeconds(10));

            //}


            return View(loc);
        }
        //Appointment table for the given location, used for making appointments in the next five working days
        [HttpGet]
        public IActionResult Index(string location)
        {
            //check if user is logged in
            bool LoggedIn = (User != null) && (User.Identity.IsAuthenticated);
            if (!LoggedIn)
            {
                _toastNotification.Information("You need to be logged in to make an appointment", 5);
            }
            //good data?
            if ((location == null))
            {
                ModelState.AddModelError("Location", "The location must be a valid location");
            }
            //data for the view
            ViewData["Location"] = location;
            ViewData["ActualLocation"] = _db?.Locations.Find(location);


            AppointmentsData obj = new AppointmentsData(location, _db);
            IEnumerable<Models.Appointments> loc = obj.Obj;
            return View(loc);
        }
        //Unused for now, kept for later updates
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointments obj)
        {
            //static checks
            IEnumerable<Models.Locations> loc = _db.Locations;
            if (obj.Time <= DateTime.Now)
            {
                ModelState.AddModelError("Time", "The date and time must be in the future");
            }

            if (obj.Time.DayOfWeek == DayOfWeek.Saturday || obj.Time.DayOfWeek == DayOfWeek.Sunday)
            {
                ModelState.AddModelError("Time", "The date and time must be a weekday");
            }

            if (obj.Time.Hour < _db.Locations.Find(obj.Location).StartTime.Hour || obj.Time.Hour > _db.Locations.Find(obj.Location).EndTime.Hour)
            {
                ModelState.AddModelError("Time", "The date and time must be between 9am and 5pm");
            }

            bool isLocation = false;
            foreach (var item in loc)
            {
                if (item.Place == obj.Location)
                {
                    isLocation = true;
                }
            }

            if (!isLocation)
            {
                ModelState.AddModelError("Location", "The location must be a valid location");
            }


            if (ModelState.IsValid)
            {
                // Schedule email reminder
                var reminderTime = obj.Time.AddDays(-1); // Send reminder one day before the appointment
                var subject = "Reminder: Appointment Tomorrow";
                var body = $"This is a reminder that your appointment is scheduled for {obj.Time.ToString("dddd, MMMM dd, yyyy HH:mm")}.";

                //get the email we use
                var email = obj.UserId;

                //SendEmailAsync(email, subject, body);

                //schedule the email
                BackgroundJob.Schedule(() => EmailMethods.SendEmailAsync(email, subject, body,_configuration),
                    reminderTime);

                //Save appointment to database
                _db.Appointments.Add(obj);
                _db.SaveChanges();
                _toastNotification.Success("Creation Successful!", 3);
                return RedirectToAction("Select");
            }



            return View(obj);

        }

        //Exact copy of the function above without hangfire, used for testing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNoHangfire(Appointments obj)
        {
            IEnumerable<Models.Locations> loc = _db.Locations;
            if (obj.Time <= DateTime.Now)
            {
                ModelState.AddModelError("Time", "The date and time must be in the future");
            }

            if (obj.Time.DayOfWeek == DayOfWeek.Saturday || obj.Time.DayOfWeek == DayOfWeek.Sunday)
            {
                ModelState.AddModelError("Time", "The date and time must be a weekday");
            }

            if (obj.Time.Hour < _db.Locations.Find(obj.Location).StartTime.Hour || obj.Time.Hour > _db.Locations.Find(obj.Location).EndTime.Hour)
            {
                ModelState.AddModelError("Time", "The date and time must be between 9am and 5pm");
            }

            bool isLocation = false;
            foreach (var item in loc)
            {
                if (item.Place == obj.Location)
                {
                    isLocation = true;
                }
            }

            if (!isLocation)
            {
                ModelState.AddModelError("Location", "The location must be a valid location");
            }


            if (ModelState.IsValid)
            {
                // Schedule email reminder
                var reminderTime = obj.Time.AddDays(-1); // Send reminder one day before the appointment
                var subject = "Reminder: Appointment Tomorrow";
                var body = $"This is a reminder that your appointment is scheduled for {obj.Time.ToString("dddd, MMMM dd, yyyy HH:mm")}.";

                //get the email we use
                var email = obj.UserId;

                //SendEmailAsync(email, subject, body);


                //BackgroundJob.Schedule(() => EmailMethods.SendEmailAsync(email, subject, body, _configuration),
                    //reminderTime);

                //Save appointment to database
                _db.Appointments.Add(obj);
                _db.SaveChanges();
                _toastNotification.Success("Creation Successful!", 3);
                return RedirectToAction("Select");
            }



            return View(obj);

        }

        //[HttpGet]
        //public IActionResult CreateWithDataTesting(LocDateData info)
        //{
        //    //ViewBag.DateData = date;
        //    var model = new Appointments
        //    {
        //        Location = info.Location,
        //        Time = info.Date
        //    };

        //    // Retrieve the list of Employees from the database
        //    var locationEmployees = (new { Id = 0, Name = "Testing", Surname = "Employee" };

        //    if (locationEmployees.Count == 0)
        //    {
        //        locationEmployees.Add(new { Id = -1, Name = "Unknown Employee", Surname = "" });
        //    }

        //    // Ensure ViewBag.Employees is initialized
        //    ViewBag.Employees = locationEmployees;


        //    return View(model);
        //}

        //Creation with the data from the previous page, location and time given
        [HttpGet]
        public IActionResult CreateWithData(LocDateData info)
        {
            //ViewBag.DateData = date;
            //data reformating for view
            var model = new Appointments
            {
                Location = info.Location,
                Time = info.Date
            };

            // Retrieve the list of Employees from the database
            var locationEmployees = _db.LocationEmployees
                                    .Where(le => le.LocationPlace == info.Location)
                                    .Select(le => new
                                    {
                                        // Select only the properties you need
                                        Id = le.EmployeeId,
                                        Name = le.Employee.Name,
                                        Surname = le.Employee.Surname
                                    })
                                    .ToList();
            // Add a default employee if no employees are found
            if (locationEmployees.Count == 0)
            {
                locationEmployees.Add(new { Id = -1, Name = "Unknown Employee", Surname = "" });
            }

            // Ensure ViewBag.Employees is initialized
            ViewBag.Employees = locationEmployees;


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateWithData(Appointments obj)
        {
            //static checks, but because of the constraints of the page, all data must be valid
            if (ModelState.IsValid)
            {

                // Schedule email
                var subjectNow = "New Appointment";
                var bodyNow = $"You have created a new appointment, which will start at {obj.Time.ToString("dddd, MMMM dd, yyyy HH:mm")}.";

                //get the email we use
                var email = obj.UserId;

                //Send creation confirmation
                EmailMethods.SendEmailAsync(email, subjectNow, bodyNow, _configuration);

                // Schedule email reminder

                DateTime reminderTime = obj.Time.AddDays(-1); // Send reminder one day before the appointment
                //reminderTime = reminderTime.AddHours(2); // No need for this, i had a different kind of problem
                var subject = "Reminder: Appointment Tomorrow";
                var body = $"This is a reminder that your appointment is scheduled for {obj.Time.ToString("dddd, MMMM dd, yyyy HH:mm")}.";

                

                //schedule the email
                obj.JobId = BackgroundJob.Schedule(() => EmailMethods.SendEmailAsyncFixed(email, subject, body),
                    reminderTime);

                //Testing code, if everything starts to break down again
                //DateTime testTime1 = DateTime.Now;
                //testTime1 = testTime1.AddMinutes(1);
                //obj.JobId = BackgroundJob.Schedule(() => EmailMethods.SendEmailAsyncFixed(email, "Test1", "No compensation"),
                //    testTime1);

                //DateTime testTime2 = DateTime.Now;
                //testTime2 = testTime2.AddHours(2);
                //testTime2 = testTime2.AddMinutes(1);
                //obj.JobId = BackgroundJob.Schedule(() => EmailMethods.SendEmailAsyncFixed(email, "Test2", "2 hour compensation"),
                //    testTime2);

                //save appointment to database
                _db.Appointments.Add(obj);
                _db.SaveChanges();
                _toastNotification.Success("Creation Successful!", 3);
                return RedirectToAction("Index", new RouteValueDictionary { { "location", obj.Location } });
            }

            _toastNotification.Error("Name field is required", 3);

            string previousUrl = Request.Headers["Referer"].ToString();
            return Redirect(previousUrl);
        }

        [HttpGet]
        public IActionResult Remove(int? id)
        {           
            //data check
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //base data retrieval
            var obj = _db.Appointments.Find(id);

            if (obj == null)
            {
                return NotFound();
            }
            // Retrieve the list of Employees from the database
            var locationEmployeeName = _db.Employees.Where(e => e.Id == obj.EmployeeId).FirstOrDefault().Name != null ? 
                _db.Employees.Where(e => e.Id == obj.EmployeeId).FirstOrDefault().Name : 
                "No Employee Associated With This Appointment";

            var locationEmployeeSurname = _db.Employees.Where(e => e.Id == obj.EmployeeId).FirstOrDefault().Surname != null ?
                _db.Employees.Where(e => e.Id == obj.EmployeeId).FirstOrDefault().Surname :
                "No Employee Associated With This Appointment";

            // Ensure ViewBag.Employees is initialized
            ViewBag.locationEmployeeName = locationEmployeeName;
            ViewBag.locationEmployeeSurname = locationEmployeeSurname;

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(Appointments obj)
        {

            // Schedule email
            var subjectNow = "Appointment Deletion";
            var bodyNow = $"You have deleted an appointment with the following start time:{obj.Time.ToString("dddd, MMMM dd, yyyy HH:mm")}.";

            //get the email we use
            var email = obj.UserId;

            //Send creation confirmation
            EmailMethods.SendEmailAsync(email, subjectNow, bodyNow,_configuration);

            //retrieval, deletion, and notification
            string loc = obj.Location;

            BackgroundJob.Delete(obj.JobId);

            _db.Appointments.Remove(obj);
            _db.SaveChanges();

            _toastNotification.Success("Removal Successful!", 3);
            //back to main page of the given location
            return RedirectToAction("Index", new RouteValueDictionary { { "location", obj.Location } });


        }
        //Appointments associated with the given user
        [HttpGet]
        public IActionResult Person()
        {
            IEnumerable<Models.Appointments> app = _db.Appointments;

            if (User.Identity.IsAuthenticated)
            {
                return View(app);
            }
            else
            {
                _toastNotification.Error("Unexpected Login Error", 3);
                return RedirectToAction("Index", "Home");
            }
        }

        public List<Appointments> GetData()
        {
            return _db.Appointments.ToList();
        }


        //Pdf generation of all data
        [HttpGet]
        public IActionResult GeneratePdf()
        {
            // Define the file path where the PDF will be saved
            string filePath = "AllAppointments.pdf";

            try
            {
                // Create a FileStream to write the PDF content
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    PdfWriter writer = new PdfWriter(fileStream);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);

                    // Create a font for the title with bigger size and bold style
                    var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                    // Add content to the PDF document
                    document.Add(new Paragraph("All appointments in the database")
                        .SetFont(boldFont) // Set the font to the boldFont
                        .SetFontSize(16)    // Set the font size
                        .SetTextAlignment(TextAlignment.CENTER)); // Align the text to the center;

                    //Creating table
                    float[] columnWidths = { 75F, 75F, 75F, 75F, 75F, 75F };
                    Table table = new Table(UnitValue.CreatePointArray(columnWidths));

                    //Add Headers
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Id")).SetFont(boldFont));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Appointment Name")).SetFont(boldFont));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Time of Appointment")).SetFont(boldFont));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Location")).SetFont(boldFont));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("User")).SetFont(boldFont));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Employee")).SetFont(boldFont));



                    // Add more content as needed
                    var data = GetData();
                    foreach (var item in data)
                    {
                        //document.Add(new Paragraph(item.ToString()));
                        table.AddCell(item.Id.ToString());
                        table.AddCell(item.Name);
                        table.AddCell(item.Time.ToString());
                        table.AddCell(item.Location);

                        string userName = "No User"; // Default value if user is not found or Email is null

                        // Find the User object by UserId
                        var user = _db.Users.FirstOrDefault(u => u.UserName == item.UserId);

                        // Check if the User object is not null and Email is not null
                        if (user != null && user.UserName != null)
                        {
                            userName = user.UserName;
                        }

                        table.AddCell(userName);

                        string employeeName = "No Employee"; // Default value if employee is not found or Name is null

                        // Find the Employee object by EmployeeId
                        var employee = _db.Employees.Find(item.EmployeeId);

                        // Check if the Employee object is not null and Name is not null
                        if (employee != null && employee.Name != null)
                        {
                            employeeName = employee.Name;
                        }

                        table.AddCell(employeeName);
                    }

                    //Add the table to the document
                    document.Add(table);

                    // Close the document
                    document.Close();
                }

                // Return the PDF file as a response
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/pdf", filePath);
            }
            catch (IOException ex)
            {
                // Handle IO exceptions
                return BadRequest("Error generating PDF: " + ex.Message);
            }
        }
        //Pdf generation of the appointments associated with the given user
        [HttpGet]
        public IActionResult GeneratePersonPdf(Appointments obj)
        {
            string Name = User.Identity.Name;
            // Define the file path where the PDF will be saved
            string filePath = Name + "Appointments.pdf";

            try
            {
                // Create a FileStream to write the PDF content
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    PdfWriter writer = new PdfWriter(fileStream);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);

                    // Create a font for the title with bigger size and bold style
                    var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                    // Add content to the PDF document
                    document.Add(new Paragraph(Name + "'s appointments in the database")
                        .SetFont(boldFont) // Set the font to the boldFont
                        .SetFontSize(16)    // Set the font size
                        .SetTextAlignment(TextAlignment.CENTER)); // Align the text to the center;

                    //Creating table
                    float[] columnWidths = { 75F, 75F, 75F, 75F, 75F, 75F };
                    Table table = new Table(UnitValue.CreatePointArray(columnWidths));

                    //Add Headers
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Id")).SetFont(boldFont));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Appointment Name")).SetFont(boldFont));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Time of Appointment")).SetFont(boldFont));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Location")).SetFont(boldFont));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("User")).SetFont(boldFont));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Employee")).SetFont(boldFont));



                    // Add more content as needed
                    var data = GetData();
                    foreach (var item in data)
                    {
                        if (item.UserId == Name)
                        {

                        
                            //document.Add(new Paragraph(item.ToString()));
                            table.AddCell(item.Id.ToString());
                            table.AddCell(item.Name);
                            table.AddCell(item.Time.ToString());
                            table.AddCell(item.Location);

                            string userName = "No User"; // Default value if user is not found or Email is null

                            // Find the User object by UserId
                            var user = _db.Users.FirstOrDefault(u => u.UserName == item.UserId);

                            // Check if the User object is not null and Email is not null
                            if (user != null && user.UserName != null)
                            {
                                userName = user.UserName;
                            }

                            table.AddCell(userName);

                            string employeeName = "No Employee"; // Default value if employee is not found or Name is null

                            // Find the Employee object by EmployeeId
                            var employee = _db.Employees.Find(item.EmployeeId);

                            // Check if the Employee object is not null and Name is not null
                            if (employee != null && employee.Name != null)
                            {
                                employeeName = employee.Name;
                            }

                            table.AddCell(employeeName);
                        }
                    }

                    //Add the table to the document
                    document.Add(table);

                    // Close the document
                    document.Close();
                }

                // Return the PDF file as a response
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/pdf", filePath);
            }
            catch (IOException ex)
            {
                // Handle IO exceptions
                return BadRequest("Error generating PDF: " + ex.Message);
            }
        }
        //Helper function for creating an excel file
        public byte[] GenerateExcelFile (List<Appointments> data)
        {
            using (var package = new ExcelPackage())
            {
                //create base excel file
                var workSheet = package.Workbook.Worksheets.Add("Appointments");
                workSheet.Cells.LoadFromCollection(data, true);
                return package.GetAsByteArray();
            }
        }
        //Excel generation of all data
        [HttpGet]
        public IActionResult GenerateExcel()
        {
            var data = _db.Appointments.ToList();
            var fileBytes = GenerateExcelFile(data);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Appointments.xlsx");
        }
        //Excel generation of the appointments associated with the given user
        [HttpGet]
        public IActionResult GeneratePersonExcel()
        {
            string Name = User.Identity.Name;
            var data = _db.Appointments.Where(a => a.UserId == Name).ToList();
            var fileBytes = GenerateExcelFile(data);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Name + ".xlsx");
        }

        

    }
}
