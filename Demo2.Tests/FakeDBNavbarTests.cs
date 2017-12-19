using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Demo2.Models;
using FluentAssertions;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Links;
using Sitecore.Sites;
using Sitecore.Web;
using Xunit;

namespace Demo2.Tests
{
    public class FakeDBNavbarTests
    {
        private Navbar2Controller _sut;

        public FakeDBNavbarTests()
        {
            _sut = new Navbar2Controller();
            Sitecore.Context.Site = GetSiteWithStartItemSetToHome();
        }

        [Fact]
        public void HomeItemSet()
        {
            
            Sitecore.Context.Site.StartPath.Should().BeEquivalentTo("/sitecore/content/home");
        }

        [Fact]
        public void NoChildren_NoElements()
        {
            using (var db = new Db()
            {
                new DbItem("Home")
            })
            {
                Sitecore.Context.Database = db.Database;

                ViewResult result = _sut.Index();

                (result.Model as IEnumerable<NavElement>).Should().BeEmpty();

            }
        }

        [Fact]
        public void Children_ReturnsThem()
        {
            using (var db = new Db()
            {
                new DbItem("Home") {new DbItem("item 1"), new DbItem("item 2"), new DbItem("item 3")}
            })
            {
                
                Sitecore.Context.Database = db.Database;

                ViewResult result = _sut.Index();

                (result.Model as IEnumerable<NavElement>).Should().HaveCount(3);

            }
        }

        [Fact]
        public void LinkManagerWorks()
        {
            using (var db = new Db()
            {
                new DbItem("Home") {new DbItem("item 1"), new DbItem("item 2"), new DbItem("item 3")}
            })
            {

                Sitecore.Context.Database = db.Database;

                LinkManager.GetItemUrl(db.GetItem("/sitecore/content/home/item 1")).Should().Be("/en/item 1.aspx");
               
            }
        }

     
        [Fact]
        public void NameAndUrlSet()
        {
            using (var db = new Db()
            {
                new DbItem("Home") {new DbItem("item 1")}
            })
            {
                var viewResult = _sut.Index() as ViewResult;
                NavElement element = (viewResult.Model as IEnumerable<NavElement>).First();
                element.Text.Should().Be("item 1");
                element.Href.Should().Be("/en/item 1.aspx");
            }
        }


        [Fact]
        public void ActiveSet()
        {
            using (var db = new Db()
            {
                new DbItem("Home") {new DbItem("item 1"), new DbItem("item 2"), new DbItem("item 3")}
            })
            {
                Sitecore.Context.Item = db.GetItem("/sitecore/content/home/item 2");
                Sitecore.Context.Database = db.Database;

                ViewResult result = _sut.Index();
                NavElement[] elements = (result.Model as IEnumerable<NavElement>).ToArray();
                elements[0].Active.Should().BeFalse();
                elements[1].Active.Should().BeTrue();
                elements[2].Active.Should().BeFalse();
            }
        }


        //active set


        private static SiteContext GetSiteWithStartItemSetToHome()
        {
            var stringDictionary = new StringDictionary() {{"rootPath", "/sitecore/content"}, {"startItem", "home"}};
            var siteInfo = new SiteInfo(stringDictionary);
            return new SiteContext(siteInfo);
        }
    }

    public class Navbar2Controller: Controller
    {
        public ViewResult Index()
        {
            var navElements = new List<NavElement>();
            var home =  Sitecore.Context.Database.GetItem( Sitecore.Context.Site.StartPath);
          
            foreach (Item child in home.GetChildren())
            {
                navElements.Add(new NavElement
                {
                    Text = child.DisplayName,
                    Href = LinkManager.GetItemUrl(child),
                    Active = Sitecore.Context.Item != null && (child.ID==Sitecore.Context.Item.ID)
                });
            }
            return View("~/Views/Navbar.cshtml", navElements);
        }
    }
}
