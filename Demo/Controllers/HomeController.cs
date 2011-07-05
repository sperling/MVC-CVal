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

        [MVC_Cval.Required(ConditionProperty = "ShowCustomer")]
        public string LastName { get; set; }

        [MVC_Cval.Required(ConditionProperty = "ShowCustomer")]
        public int IntValue { get; set; }
    }

    public class Checkout
    {
        public bool ShowCustomer { get; set; }

        public Customer Customer { get; set; }
    }
}
