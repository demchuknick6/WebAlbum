using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using WebAlbum.DomainModel;
using WebAlbum.DomainServices;
using WebAlbum.Web.Areas.Users.Models;
using WebAlbum.Web.Models.Albums;
using WebAlbum.Web.Models.Photos;

namespace WebAlbum.Tests.Controllers
{
    [TestClass]
    public class UsersHomeControllerTest
    {
        private Web.Areas.Users.Controllers.HomeController _homeController;
        private IGenericRepository<Album> _albumRepository;
        private IGenericRepository<Photo> _photoRepository;

        [TestInitialize]
        public void SetupContext()
        {
            var substituteAlbumDbSet = Substitute.For<IDbSet<Album>>();
            var albumsList = new List<Album>()
            {
                new Album() {AlbumId = 1, AlbumTitle = "New1", Public = true, UserId = "NewUserId1"},
                new Album() {AlbumId = 2, AlbumTitle = "New2", Public = false, UserId = "NewUserId1"},
                new Album() {AlbumId = 3, AlbumTitle = "New3", Public = true, UserId = "NewUserId2"}
            }.AsQueryable();

            var substitutePhotoDbSet = Substitute.For<IDbSet<Photo>>();
            var photosList = new List<Photo>()
            {
                new Photo() {AlbumId = 1, PhotoTitle = "New1", PhotoId = 1},
                new Photo() {AlbumId = 1, PhotoTitle = "New2", PhotoId = 2},
                new Photo() {AlbumId = 2, PhotoTitle = "New3", PhotoId = 3}
            }.AsQueryable();
                    
            substituteAlbumDbSet.Provider.Returns(albumsList.Provider);
            substituteAlbumDbSet.Expression.Returns(albumsList.Expression);
            substituteAlbumDbSet.ElementType.Returns(albumsList.ElementType);
            substituteAlbumDbSet.GetEnumerator().Returns(albumsList.GetEnumerator());

            substitutePhotoDbSet.Provider.Returns(photosList.Provider);
            substitutePhotoDbSet.Expression.Returns(photosList.Expression);
            substitutePhotoDbSet.ElementType.Returns(photosList.ElementType);
            substitutePhotoDbSet.GetEnumerator().Returns(photosList.GetEnumerator());

            _albumRepository = Substitute.For<IGenericRepository<Album>>();
            _albumRepository.AsQueryable().Returns(substituteAlbumDbSet.AsQueryable());

            _photoRepository = Substitute.For<IGenericRepository<Photo>>();
            _photoRepository.AsQueryable().Returns(substitutePhotoDbSet.AsQueryable());

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<NewAlbumViewModel, Album>().ReverseMap();
                cfg.CreateMap<Album, AlbumViewModel>().ReverseMap();
                cfg.CreateMap<PhotoViewModel, Photo>()
                    .ForMember(x => x.Content, o => o.Ignore());
                cfg.CreateMap<Photo, NewPhotoViewModel>().ReverseMap();
                cfg.CreateMap<Photo, PhotoViewModel>()
                    .ForMember(dest => dest.Content,
                        o => o.MapFrom(
                            src => "data:image/png;base64," + 
                                   Convert.ToBase64String(src.Content.ToArray())));
            });

            var mapper = config.CreateMapper();

            _homeController = new Web.Areas.Users.Controllers.HomeController
                (_albumRepository, _photoRepository, mapper);       
        }

        [TestMethod]
        public void GetRequestToIndexReturnsView()
        {
            var request = Substitute.For<HttpRequestBase>();
            request.Headers.Returns(new System.Net.WebHeaderCollection
                { {"X-Requested-With", "HttpRequest"}});
            var context = Substitute.For<HttpContextBase>();
            context.Request.Returns(request);
            _homeController.ControllerContext = new ControllerContext
                (context, new RouteData(), _homeController);

            var result = _homeController.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void AjaxRequestToIndexActionReturnsPartialView()
        {
            var request = Substitute.For<HttpRequestBase>();
            request.Headers.Returns(new System.Net.WebHeaderCollection
                { {"X-Requested-With", "XMLHttpRequest"}});
            var context = Substitute.For<HttpContextBase>();
            context.Request.Returns(request);
            _homeController.ControllerContext = new ControllerContext
                (context, new RouteData(), _homeController);

            var result = _homeController.Index("user","") 
                as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.AreEqual("_SearchResults", result.ViewName);
        }

        [TestMethod]
        public void AjaxRequestToIndexReturnsTwoAlbumsTypeOfSearchAlbumViewModel()
        {
            var request = Substitute.For<HttpRequestBase>();
            request.Headers.Returns(new System.Net.WebHeaderCollection
                { {"X-Requested-With", "XMLHttpRequest"}});
            var context = Substitute.For<HttpContextBase>();
            context.Request.Returns(request);
            _homeController.ControllerContext = new ControllerContext
                (context, new RouteData(), _homeController);

            var result = _homeController.Index("album", "New") 
                as PartialViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.ViewData.Model,
                typeof(IEnumerable<SearchAlbumViewModel>));

            var albums = (List<SearchAlbumViewModel>)result.ViewData.Model;
            Assert.AreEqual(2, albums.Count());  //only public albums visible
            Assert.AreEqual("New1", albums[0].AlbumTitle);
            Assert.AreEqual("New3", albums[1].AlbumTitle);
        }

        [TestMethod]
        public void AjaxRequestToIndexReturns_NoResultsPartialView()
        {
            var request = Substitute.For<HttpRequestBase>();
            request.Headers.Returns(new System.Net.WebHeaderCollection
                { {"X-Requested-With", "XMLHttpRequest"}});
            var context = Substitute.For<HttpContextBase>();
            context.Request.Returns(request);
            _homeController.ControllerContext = new ControllerContext
                (context, new RouteData(), _homeController);

            var result = _homeController.Index("album", "nonexistentUser")
                as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.AreEqual("_NoResults", result.ViewName);
        }
    }
}
