using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using AutoSitecore;
using Demo2.Models;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Core;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit2;
using Sitecore.Abstractions;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Links;
using Sitecore.Sites;
using Sitecore.Web;
using Xunit;
using Attribute = System.Attribute;

namespace Demo2.Tests
{
    public class AutoSitecoreNavbar
    {
        private Navbar3Controller _sut;

        public AutoSitecoreNavbar()
        {
            _sut = new Navbar3Controller();
        }


        #region AutoSitecore basic funcationality

        [Theory, AutoSitecore]
        public void CanCreateItem(Item item)
        {
            item.Should().NotBeNull();
            item.Fields.Should().BeEmpty();
        }

        [Theory, AutoSitecore]
        public void CanCreateItemWithFields([ItemData(fields: true)]Item item)
        {
            item.Fields.Should().HaveCount(3);
        }

        [Theory, AutoSitecore]
        public void CanSetSiteRoot([ItemData(fields: true)]Item item, Database db)
        {
            Sitecore.Context.Database = db;
            Sitecore.Context.Site = GetSiteContext();
            Sitecore.Context.Site.StartPath.Should().Be("/sitecore/content/home");

            db.GetItem(Sitecore.Context.Site.StartPath).Returns(item);
            db.GetItem(Sitecore.Context.Site.StartPath).ID.Should().Be(item.ID);
        }

        #endregion

        #region NavbarTests
        [Theory, AutoSitecore]
        public void ReturnsCorrectView(Item home, Database db)
        {
            db.GetItem("/sitecore/content/home").Returns(home);
            Sitecore.Context.Database = db;
            Sitecore.Context.Site = GetSiteContext();
            home.GetChildren().Returns(new ChildList(home, new ItemList()));

            ViewResult result = _sut.Index();

            result.ViewName.Should().Be("~/Views/Navbar.cshtml");
        }

        [Theory, AutoSitecore]
        public void NoChilderen_NoElementsReturned(Item home, Database db)
        {
            db.GetItem("/sitecore/content/home").Returns(home);
            Sitecore.Context.Site = GetSiteContext();
            Sitecore.Context.Database = db;
            home.GetChildren().Returns(new ChildList(home, new ItemList()));

            ViewResult result = _sut.Index();

            result.Model.GetList().Should().BeEmpty();
        }

        [Theory, AutoSitecore]
        public void Childeren_ReturnsThem(Item home, Item child1, Item child2, Item child3, Database db)
        {
            home.GetChildren().Returns(new ChildList(home, new List<Item> {child1, child2, child3}));
            db.GetItem("/sitecore/content/home").Returns(home);
            Sitecore.Context.Site = GetSiteContext();
            Sitecore.Context.Database = db;

            ViewResult result = _sut.Index();

            result.Model.GetList().Should().HaveCount(3);
        }


        [Theory, CustomizedData]
        public void CustomAttribute(Item home, Item child1, Item child2, Item child3, Database db)
        {
            home.GetChildren().Returns(new ChildList(home, new List<Item> { child1, child2, child3 }));

            ViewResult result = _sut.Index();

            result.Model.GetList().Should().HaveCount(3);
        }

        [Theory, CustomizedData]
        public void SetsNameAndHref(Item home, Item child1, Database db, string someUrl)
        {
            home.GetChildren().Returns(new ChildList(home, new List<Item> {child1}));
            child1.Name.Should().NotBeNullOrEmpty();
            var fakeLinkManager = Substitute.For<BaseLinkManager>();
            ServiceLocator.SetServiceProvider(new LinkManagerService(fakeLinkManager));
            fakeLinkManager.GetItemUrl(child1).Returns(someUrl);
            
            ViewResult result = _sut.Index();

            NavElement navElement = result.Model.GetList().First();
            navElement.Text.Should().Be(child1.Name);
            navElement.Href.Should().Be(someUrl);
        }

        [Theory, CustomizedData]
        public void CanHandleStringNamedHome(string home)
        {
            home.Should().NotBeNullOrEmpty();
        }

        [Theory, CustomizedData]
        public void CanAttachHomeItem(Item home, Item child1, Database db)
        {
            db.GetItem("/sitecore/content/home").ID.Should().Be(home.ID);
        }
        #endregion

        private static SiteContext GetSiteContext()
        {
            return new SiteContext(new SiteInfo(new StringDictionary() { { "rootPath", "/sitecore/content" }, { "startItem", "home" } }));
        }
    }

    public class LinkManagerService : IServiceProvider
    {
        private readonly BaseLinkManager _fakeLinkManager;

        public LinkManagerService(BaseLinkManager fakeLinkManager)
        {
            _fakeLinkManager = fakeLinkManager;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(BaseLinkManager))
            {
                return _fakeLinkManager;
            }
            return null;
        }
    }

   

    public class CustomizedData : AutoSitecore
    {
        public CustomizedData()
        {
            var db = Fixture.Freeze<Database>();
            Sitecore.Context.Database = db;
            Sitecore.Context.Site =
                new SiteContext(new SiteInfo(new StringDictionary
                {
                    {"rootPath", "/sitecore/content"},
                    {"startItem", "home"}
                }));
            Item home = Fixture.Create<Item>();
            db.GetItem("/sitecore/content/home").Returns(home);

            Fixture.Customizations.Insert(0, new ReturnHomeItemSpecimentBuilder(home));
 
        }
    }

    public class ReturnHomeItemSpecimentBuilder : ISpecimenBuilder
    {
        private readonly Item _homeItem;

        public ReturnHomeItemSpecimentBuilder(Item homeItem)
        {
            _homeItem = homeItem;
        }

        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as System.Reflection.ParameterInfo;
            if (pi != null && pi.ParameterType == typeof(Item) && pi.Name.Equals("home", StringComparison.InvariantCultureIgnoreCase))
            {
                return _homeItem;
            }
            return new NoSpecimen();
        }
    }

    public class Navbar3Controller:Controller
    {
        public ViewResult Index()
        {
            Database db =  Sitecore.Context.Database;
            Item home = db.GetItem(Sitecore.Context.Site.StartPath);
            var children = home.GetChildren();
            List<NavElement> list = new List<NavElement>();
            foreach (Item child in children)
            {
                string url = LinkManager.GetItemUrl(child);
                list.Add(new NavElement{Text=child.Name, Href=url});
            }
            return View("~/Views/Navbar.cshtml", list);
        }
    }

    public class AutoSitecore : AutoDataAttribute
    {
        public AutoSitecore():base(new Fixture().Customize(new AutoSitecoreCustomization()))
        {
            
        }
    }


    public static class ModelExtensions{

        public static IEnumerable<NavElement> GetList(this object model)
        {
            return model as IEnumerable<NavElement>;
        }
    }
}
