using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCManukauTech.Extensions;
using MVCManukauTech.Models;
using MVCManukauTech.ViewModels;

namespace MVCManukauTech.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();

        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            ViewBag.isAdmin = UserRolesAdmin.getRoleName(HttpContext.Session.Get<UserRolesAdmin>("Role")).Equals("Admin");
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //20180327 Yanhua Liu add for testing upload images  of TinyMCE 
        //refers : https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
        public string UploadFile(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                return "file not selected";
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/upload", file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
                stream.Position = 0;
                stream.Close();
            }
            var host = HttpContext.Request.Host;

            //string json = JsonConvert.SerializeObject("{location:"+ host + "/wwwroot/images/upload/" + file.FileName+ "}");
            string json = "" + host + "/images/upload/" + file.FileName;
            return json;
        }

        //20180328 Yanhua Liu add for contact page editing
        //refers : https://stackoverflow.com/questions/7569904/easiest-way-to-read-from-and-write-to-files
        public string SaveContactContent(string content, string filename)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/incHtml", "Contact.txt");
            using (StreamWriter stream = new StreamWriter(path))
            {
                stream.Write(content);
                stream.Flush();
                stream.Close();
            }
            return "OK";
        }
        public string LoadContactContent(string filename)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/incHtml", filename+".txt");
            string content = "";
            using (StreamReader stream = new StreamReader(path))
            {
                content += stream.ReadToEnd();
                stream.Close();
            }
            return content;
        }
    }
}
