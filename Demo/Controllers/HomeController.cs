using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View(new Checkout() { Customer = new Customer() });
        }

        [HttpPost]
        public ActionResult Index(Checkout checkout)
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            
            return View(checkout);
        }

        public ActionResult About()
        {
            return View();
        }
    }

    public class Customer
    {
        [Required]
        public string FirstName { get; set; }

        [MVCCval.Required("ShowCustomer")]
        public string LastName { get; set; }

        [MVCCval.Required("ShowCustomer")]
        public int IntValue { get; set; }

        [MVCCval.StringLength("ShowCustomer", 10)]
        public string MaxLength { get; set; }

        [MVCCval.StringLength("ShowCustomer", 10, MinimumLength=2)]
        public string MinAndMaxLength { get; set; }

        [MVCCval.Range("ShowCustomer", 3, 20)]
        public int RangeInt { get; set; }

        [Range(3, 20)]
        public int TestRangeInt { get; set; }

        [MVCCval.RegularExpression("ShowCustomer", @"^\d{4}[\/-]\d{1,2}[\/-]\d{1,2}$")]
        public string Date { get; set; }
    }

    public class Checkout
    {
        public bool ShowCustomer { get; set; }

        public Customer Customer { get; set; }
    }
}
