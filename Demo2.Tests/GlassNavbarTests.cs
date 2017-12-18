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
    public class GlassNavbarTests
    {
        private ISitecoreContext _context;
        private NavbarController _sut;

        public GlassNavbarTests()
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

            _context.GetHomeItem<IBaseItem>().Children.Returns(new List<IBaseItem>
            {
                Substitute.For<IBaseItem>(),
                Substitute.For<IBaseItem>(),
                Substitute.For<IBaseItem>()
            });

            ViewResult result = _sut.Index() as ViewResult;


            var resultCollection = result.Model as IEnumerable<Models.NavElement>;
            resultCollection.Should().HaveCount(3);
        }


        [Fact]
        public void NavbarElement_KeepsNameAndHref()
        {

            IBaseItem child = Substitute.For<IBaseItem>();
            string name = "some name";
            child.Name.Returns(name);
            string someUrl = "some url";
            child.Url.Returns(someUrl);
            _context.GetHomeItem<IBaseItem>().Children.Returns(new List<IBaseItem>
                {
                    child
                });

            ViewResult result = _sut.Index() as ViewResult;


            var resultCollection = result.Model as IEnumerable<Models.NavElement>;
            resultCollection.Should().HaveCount(1);
            var model = resultCollection.First();
            model.Text.Should().Be(name);
            model.Href.Should().Be(someUrl);

        }


        [Fact]
        public void SelectedElement_GetsHighlight()
        {

            IBaseItem child = Substitute.For<IBaseItem>();
            string name = "some name";
            child.Name.Returns(name);
            string someUrl = "some url";
            child.Url.Returns(someUrl);
            _context.GetHomeItem<IBaseItem>().Children.Returns(new List<IBaseItem>
            {
                child
            });

            ViewResult result = _sut.Index() as ViewResult;


            var resultCollection = result.Model as IEnumerable<Models.NavElement>;
            resultCollection.Should().HaveCount(1);
            var model = resultCollection.First();
            model.Text.Should().Be(name);
            model.Href.Should().Be(someUrl);

        }


        [Fact]
        public void CurrentItemInList_MarkedActive()
        {
            IBaseItem item1, item2, item3;
            item1 = Substitute.For<IBaseItem>();
            item2 = Substitute.For<IBaseItem>();
            item3 = Substitute.For<IBaseItem>();

            Guid id1, id2, id3;
            id1 = Guid.NewGuid();
            id2 = Guid.NewGuid();
            id3 = Guid.NewGuid();

            item1.Id.Returns(id1);
            item2.Id.Returns(id2);
            item3.Id.Returns(id3);

            _context.GetHomeItem<IBaseItem>().Children.Returns(new List<IBaseItem> {item1, item2, item3});
            _context.GetCurrentItem<IBaseItem>().Id.Returns(id2);


            ViewResult result = _sut.Index() as ViewResult;
            var resultCollection = result.Model as IEnumerable<Models.NavElement>;


            resultCollection.Should().HaveCount(3);
            var list = resultCollection.ToArray();
            list[0].Active.Should().BeFalse();
            list[1].Active.Should().BeTrue();
            list[2].Active.Should().BeFalse();

        }
        // If grand children, add drop list.
    }
}
