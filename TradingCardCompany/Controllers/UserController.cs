using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TradingCardCompany.Models;

namespace TradingCardCompany.Controllers
{
    public class UserController : Controller
    {
        TradingCardCompanyEntities context = new TradingCardCompanyEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult SignUp(Register register)
        {
            using(context = new TradingCardCompanyEntities())
            {
                context.Registers.Add(register);
                context.SaveChanges();
            }
            return RedirectToAction("Login");
        }

       // Login Functionality //

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login login)
        {
            using(context = new TradingCardCompanyEntities())
            {
                bool isValid = context.Registers.Any(x => x.UserName == login.UserName && x.Password == login.Password);
                if(isValid)
                {
                    FormsAuthentication.SetAuthCookie(login.UserName, false);
                    return RedirectToAction("ViewList", "Shopping");
                }

                ModelState.AddModelError("", "Invalid Username and Password");
            }
            return View();
        }

        // End of Login Functionality //

        //Logout Functionality //

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }
    }
}