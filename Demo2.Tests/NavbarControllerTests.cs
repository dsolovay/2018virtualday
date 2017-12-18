using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Demo2.Controllers;
using Demo2.Models;
using FluentAssertions;
using Glass.Mapper.Sc;
using NSubstitute;
using NSubstitute.Core;
using Xunit;

namespace Demo2.Tests
{
    public class NavbarControllerTests
    {
        private ISitecoreContext _context;
        private NavbarController _sut;

        public NavbarControllerTests()
        {
            _context = Substitute.For<ISitecoreContext>();
            _sut = new NavbarController(_context);
        }

        // For top level children, add link

        [Fact]
        public void NavbarController_NoChildrenOfHome_ReturnsEmptyList()
        {
            ViewResult result = _sut.Index() as ViewResult;
         
            var resultCollection = result.Model as IEnumerable<Models.NavElement>;
            resultCollection.Should().BeEmpty();
        }

        [Fact]
        public void NavbarController_HasChildrenOfHome_ReturnsSameNumber()
        {

            _context.GetHomeItem<ISampleItem>().Children.Returns(new List<ISampleItem>
            {
                Substitute.For<ISampleItem>(),
                Substitute.For<ISampleItem>(),
                Substitute.For<ISampleItem>()
            });

            ViewResult result = _sut.Index() as ViewResult;
            
         
            var resultCollection = result.Model as IEnumerable<Models.NavElement>;
            resultCollection.Should().HaveCount(3);
        }
 
        // If grand children, add drop list.
    }
}
