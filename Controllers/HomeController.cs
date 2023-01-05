using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UploadImage.Data;
using UploadImage.Models;
using UploadImage.ViewModel;

namespace UploadImage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment WebHostEnvironment;

        public HomeController(ApplicationDbContext context,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            WebHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var items = _context.Students.ToList();
            return View(items);
        }

        public IActionResult Edit(int id)
        {
            var student = _context.Students.Where(x=> x.Id == id).FirstOrDefault();
            return View(student);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentViewModel vm)
        {
            string stringFileName = UploadFile(vm);
            var Student = new Student
            {
                Name = vm.Name,
                ProfileImage = stringFileName
            };
            _context.Students.Add(Student);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        private string UploadFile(StudentViewModel vm)
        {
            string fileName = null;
            if(vm.ProfileImage != null)
            {
                string uploadDir = Path.Combine(WebHostEnvironment.WebRootPath, "Images");
                fileName = Guid.NewGuid().ToString() + "-" + vm.ProfileImage.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                vm.ProfileImage.CopyTo(fileStream);
            }
            return fileName;
        }
    }
}
