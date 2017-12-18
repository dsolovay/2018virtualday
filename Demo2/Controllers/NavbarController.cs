using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Glass.Mapper.Sc;

namespace Demo2.Controllers
{
    public class NavbarController : Controller
    {
        private readonly ISitecoreContext _sitecoreContext;

        public NavbarController(ISitecoreContext sitecoreContext)
        {
            _sitecoreContext = sitecoreContext;
        }

        // GET: Navbar
        public ActionResult Index()
        {
            var home = _sitecoreContext.GetHomeItem<ISampleItem>()
            return View("~/Views/Navbar.cshtml", new List<Models.NavElement>());
        }
    }
}