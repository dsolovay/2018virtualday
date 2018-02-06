using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Demo1.Controllers;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Xunit.Sdk;

namespace Demo1Tests
{
    public class HomeControllerTests
    {
        private HomeController _homeController;
        private HttpContextBase _httpContextBase;

        public HomeControllerTests()
        {
            _homeController = new HomeController();
            _httpContextBase = Substitute.For<HttpContextBase>();

            _httpContextBase.Request.Cookies.Returns(new HttpCookieCollection());
            _httpContextBase.Response.Cookies.Returns(new HttpCookieCollection());

            _homeController.ControllerContext = new ControllerContext(_httpContextBase,new RouteData(), _homeController);
        }


        [Fact]
        public void AboutPage_NoCookie_SetsToOne()
        { 
            //act 
            _homeController.About();

            //asert
            _httpContextBase.Response.Cookies["visit_count"].Value.Should().Be("1");
        }

        [Fact]
        public void AboutPage_HasValue_Increments()
        {
            //arrange
            var cookie = new HttpCookie("visit_count", "1");
            _httpContextBase.Request.Cookies.Add(cookie);

            //act 
            _homeController.About();

            //asert
            _httpContextBase.Response.Cookies["visit_count"].Value.Should().Be("2");
        }
    }
}
