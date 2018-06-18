using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVCManukauTech.Models.DB;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using MVCManukauTech.Models;
using MVCManukauTech.ViewModels;
using MVCManukauTech.Extensions;
using Microsoft.Extensions.Logging;

namespace MVCManukauTech.Controllers
{
    public class CatalogController : Controller
    {
        private readonly FS01_YanHua_XSpyContext _context;
        private readonly ILogger _logger;

        public CatalogController(ILogger<AccountController> logger, FS01_YanHua_XSpyContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Catalog?CategoryName=XX
        public IActionResult Index()
        {
            if ((string)Request.Query["NotUseAngular"] == null)
            {
                ViewBag.UseAngular = true;
                string categoryN = Request.Query["CategoryName"];
                if (categoryN != null)
                {
                    ViewBag.CategoryName = categoryN;
                }
                //20180327 Yanhua Liu add for showing discount price;

                ViewBag.Discount = UserRolesAdmin.getDiscount(HttpContext.Session.Get<UserRolesAdmin>("Role"));
                return View();
            }
            //140903 JPC add CategoryName to SELECT list of fields
            string SQL = "SELECT ProductId, Products.CategoryId AS CategoryId, Name, ImageFileName, UnitCost"
                + ", SUBSTRING(Description, 1, 100) + '...' AS Description, CategoryName "
                + "FROM Products INNER JOIN Categories ON Products.CategoryId = Categories.CategoryId ";
            string categoryName = Request.Query["CategoryName"];

            if (categoryName != null)
            {
                //140903 JPC security check - if ProductId is dodgy then return bad request and log the fact 
                //  of a possible hacker attack.  Excessive length or containing possible control characters
                //  are cause for concern!  TODO move this into a separate reusable code method with more sophistication.
                if (categoryName.Length > 20 || categoryName.IndexOf("'") > -1 || categoryName.IndexOf("#") > -1)
                {
                    //TODO Code to log this event and send alert email to admin
                    return BadRequest(); // Http status code 400
                }

                //140903 JPC  Passed the above test so extend SQL
                //150807 JPC Security improvement @p0
                SQL += " WHERE CategoryName = @p0";
                //SQL += " WHERE CategoryName = '{0}'";
                //SQL = String.Format(SQL, CategoryName);
                //Send extra info to the view that this is the selected CategoryName
                ViewBag.CategoryName = categoryName;
            }

            //150807 JPC Security improvement implementation of @p0
            var products = _context.CatalogViewModel.FromSql(SQL, categoryName);
            return View(products.ToList());
        }
        //20180325 Yanhua Liu add for angular
        public string GetProducts()
        {
            string currentPage = Request.Query["CurrentPage"];
            if (currentPage == null)
            {
                return GetProductsTotal();
            }
            int curPage = Convert.ToInt32(currentPage);
            int pageCount = Convert.ToInt32((string)Request.Query["PageCount"]);
            string sql = "SELECT ProductId, Products.CategoryId AS CategoryId, Name, ImageFileName, UnitCost"
                + ", Description, CategoryName "
                + "FROM Products INNER JOIN Categories ON Products.CategoryId = Categories.CategoryId "
                ;
            string categoryName = Request.Query["CategoryName"];
            if (categoryName != null)
            {
                if (categoryName.Length > 20 || categoryName.IndexOf("'") > -1 || categoryName.IndexOf("#") > -1)
                {
                    return "";
                }
                sql += " WHERE CategoryName = @p0 ";
            }
            sql += "ORDER BY ProductId ASC "
                + "OFFSET " + (pageCount * (curPage - 1)).ToString() + " ROWS "
                + "FETCH NEXT " + pageCount.ToString() + " ROWS ONLY ";

            var products = _context.CatalogViewModel.FromSql(sql, categoryName);
            decimal roleDicount = UserRolesAdmin.getDiscount(HttpContext.Session.Get<UserRolesAdmin>("Role"));
            string json = JsonConvert.SerializeObject(new {data=products, discount=roleDicount});
            return json;
        }

        public string GetProductsTotal()
        {
            string sql = "SELECT count(ProductId) as total "
                + "FROM Products INNER JOIN Categories ON Products.CategoryId = Categories.CategoryId";
            string categoryName = Request.Query["CategoryName"];
            if (categoryName != null)
            {
                if (categoryName.Length > 20 || categoryName.IndexOf("'") > -1 || categoryName.IndexOf("#") > -1)
                {
                    return "";
                }
                sql += " WHERE CategoryName = @p0";
            }
            var productsTotal = _context.ProductTotal.FromSql(sql, categoryName);
            string json = JsonConvert.SerializeObject(productsTotal);
            return json;
        }

        public string PutProduct(string productId, double unitCost)
        {
            string sql = "UPDATE Products SET UnitCost = @p0 WHERE ProductId = @p1";
            int rowsChanged = _context.Database.ExecuteSqlCommand(sql, unitCost, productId);
            return rowsChanged.ToString();
        }

        // GET: Catalog/Details?ProductId=1MORE4ME
        public IActionResult Details(string ProductId)
        {
            if (ProductId == null)
            {
                return BadRequest(); // Http status code 400
            }
            //140903 JPC security check - if ProductId is dodgy then return bad request and log the fact 
            //  of a possible hacker attack.  Excessive length or containing possible control characters
            //  are cause for concern!  TODO move this into a separate reusable code method with more sophistication.
            if (ProductId.Length > 20 || ProductId.IndexOf("'") > -1 || ProductId.IndexOf("#") > -1)
            {
                //TODO Code to log this event and send alert email to admin
                return BadRequest(); // Http status code 400
            }

            //150807 JPC Security improvement implementation of @p0
            //20180312 JPC change to query based on class CatalogViewModel
            //  Change above code to give all of the description rather than summarising with the first 100 characters
            string SQL = "SELECT ProductId, Products.CategoryId AS CategoryId, Name, ImageFileName, UnitCost"
            + ", Description, CategoryName "
            + "FROM Products INNER JOIN Categories ON Products.CategoryId = Categories.CategoryId "
            + " WHERE ProductId = @p0";

            //140904 JPC case of one product to look at the details.
            //  SQL gives some kind of collection where we need to clean that up with ToList() then take element [0]
            //150807 JPC Security improvement implementation of @p0 substitute ProductId
            var product = _context.CatalogViewModel.FromSql(SQL, ProductId).ToList()[0];
            if (product == null)
            {
                return NotFound(); //Http status code 404
            }
            return View(product);
        }
    }
}