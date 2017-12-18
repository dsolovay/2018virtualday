using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Demo2.Models;
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
            var home = _sitecoreContext.GetHomeItem<IBaseItem>();

            List<NavElement> list = new List<NavElement>();
            foreach (var child in home.Children)
            {
                list.Add(new NavElement{Text = child.Name, Href=child.Url});
            }
            return View("~/Views/Navbar.cshtml", list);
        }
    }
}