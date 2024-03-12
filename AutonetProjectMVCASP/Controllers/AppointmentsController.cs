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



    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly INotyfService _toastNotification;


        public AppointmentsController(ApplicationDbContext db, ILogger<AppointmentsController> logger, INotyfService toastNotification)
        {
            _db = db;
            _logger = logger;
            _toastNotification = toastNotification;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        }



        [HttpGet]
        public IActionResult Select()
        {
            //remove old appointments
            _db.Appointments.RemoveRange(_db.Appointments.Where(a => a.Time < DateTime.Now));
            _db.SaveChanges();

            //create list of locations
            IEnumerable<Models.Locations> loc = _db.Locations;

            return View(loc);
        }
        [HttpGet]
        public IActionResult Index(string location)
        {
            bool LoggedIn = (User != null) && (User.Identity.IsAuthenticated);
            if (!LoggedIn)
            {
                _toastNotification.Information("You need to be logged in to make an appointment", 5);
            }

            if ((location == null))
            {
                ModelState.AddModelError("Location", "The location must be a valid location");
            }

            ViewData["Location"] = location;
            ViewData["ActualLocation"] = _db?.Locations.Find(location);


            AppointmentsData obj = new AppointmentsData(location, _db);
            IEnumerable<Models.Appointments> loc = obj.Obj;
            return View(loc);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateWithData(LocDateData info)
        {
            //ViewBag.DateData = date;
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

            // Ensure ViewBag.Employees is initialized
            ViewBag.Employees = locationEmployees;


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointments obj)
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
                _db.Appointments.Add(obj);
                _db.SaveChanges();
                _toastNotification.Success("Creation Successful!", 3);
                return RedirectToAction("Select");
            }



            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateWithData(Appointments obj)
        {
            if (ModelState.IsValid)
            {
                _db.Appointments.Add(obj);
                _db.SaveChanges();
                _toastNotification.Success("Creation Successful!", 3);
                return RedirectToAction("Index", new RouteValueDictionary { { "location", obj.Location } });
            }

            TempData["success"] = "Task completed!";
            return View(obj);
        }

        [HttpGet]
        public IActionResult Remove(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Appointments.Find(id);

            if (obj == null)
            {
                return NotFound();
            }



            //_db.Appointments.Remove(obj);
            //_db.SaveChanges();

            //TempData["success"] = "Task completed!";



            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(Appointments obj)
        {

            string loc = obj.Location;

            _db.Appointments.Remove(obj);
            _db.SaveChanges();

            _toastNotification.Success("Removal Successful!", 3);

            return RedirectToAction("Index", new RouteValueDictionary { { "location", obj.Location } });




        }

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

        [HttpGet]
        public IActionResult GeneratePersonPdf(Appointments obj)
        {
            string Name = obj.UserId;
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

        public byte[] GenerateExcelFile (List<Appointments> data)
        {
            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("Appointments");
                workSheet.Cells.LoadFromCollection(data, true);
                return package.GetAsByteArray();
            }
        }
        [HttpGet]
        public IActionResult GenerateExcel()
        {
            var data = _db.Appointments.ToList();
            var fileBytes = GenerateExcelFile(data);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Appointments.xlsx");
        }
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
