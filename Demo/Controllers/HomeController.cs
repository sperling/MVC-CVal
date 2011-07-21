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

        public ActionResult Remote(Customer customer)
        {
            if (customer != null && customer.Remote == "secret")
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
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

        public string Password { get; set; }

        [MVCCval.Compare("ShowCustomer", "Password")]
        public string RetypePassword { get; set; }

        [MVCCval.Remote("ShowCustomer", "Remote", "Home")]
        public string Remote { get; set; }
    }

    public class Checkout
    {
        public bool ShowCustomer { get; set; }

        public Customer Customer { get; set; }
    }
}
