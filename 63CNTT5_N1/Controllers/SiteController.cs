using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyClass.Model;
namespace _63CNTT5_N1.Controllers
{
    public class SiteController : Controller
    {
        // GET: Site
        public ActionResult Index()
        {
            MyDBContext db = new MyDBContext();
            int Count = db.Products.Count();
            ViewBag.choi = Count;
            return View();
        }
    }
}