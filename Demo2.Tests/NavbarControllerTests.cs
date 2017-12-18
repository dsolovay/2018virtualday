using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Demo2.Controllers;
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
        

        // If grand children, add drop list.
    }
}
