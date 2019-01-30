using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using WebAlbum.DomainModel;
using WebAlbum.DomainServices;
using WebAlbum.Web.Controllers;
using WebAlbum.Web.Models.Albums;

namespace WebAlbum.Tests.Controllers
{
    [TestClass]
    public class AlbumsControllerTest
    {
        private AlbumsController _albumsController;
        private IGenericRepository<Album> _albumRepository;
        private IUnitOfWork _unitOfWork;

        [TestInitialize]
        public void SetupContext()
        {
            var substituteDbSet = Substitute.For<IDbSet<Album>>();
            var albumsList = new List<Album>()
            {
                new Album() {AlbumId = 1, AlbumTitle = "New1", Public = true, UserId = "NewUserId1"},
                new Album() {AlbumId = 2, AlbumTitle = "New2", Public = false, UserId = "NewUserId1"},
                new Album() {AlbumId = 3, AlbumTitle = "New3", Public = false, UserId = "NewUserId2"}
            }.AsQueryable();

            substituteDbSet.Provider.Returns(albumsList.Provider);
            substituteDbSet.Expression.Returns(albumsList.Expression);
            substituteDbSet.ElementType.Returns(albumsList.ElementType);
            substituteDbSet.GetEnumerator().Returns(albumsList.GetEnumerator());

            _albumRepository = Substitute.For<IGenericRepository<Album>>();
            _albumRepository.AsQueryable().Returns(substituteDbSet.AsQueryable());
            _unitOfWork = Substitute.For<IUnitOfWork>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<NewAlbumViewModel, Album>().ReverseMap();
                cfg.CreateMap<Album, AlbumViewModel>().ReverseMap();
            });

            var mapper = config.CreateMapper();

            _albumsController = new AlbumsController(_unitOfWork, _albumRepository, mapper)
            {
                GetUserId = () => "NewUserId1"
            };
        }

        [TestMethod]
        public void IndexViewResultNotNull()
        {
            var result = _albumsController.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void Action_AlbumsReturnsTwoAlbumsTypeOfAlbumListViewModel()
        {
            var result = _albumsController._Albums() as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.ViewData.Model,
                typeof(AlbumListViewModel));

            var albums = (AlbumListViewModel)result.ViewData.Model;
            Assert.AreEqual(2, albums.Albums.Count());
            Assert.IsTrue(albums.Albums.Any(x => x.AlbumTitle == "New1"));
            Assert.IsFalse(albums.Albums.Any(x => x.AlbumTitle == "New3"));
        }

        [TestMethod]
        public void ActionCreateIsPartialViewResult()
        {
            var result = _albumsController.Create() as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }

        [TestMethod]
        public void CreatePostActionDoesNotCreateAlbumWhenHasModelError()
        {
            var album = new NewAlbumViewModel();
            _albumsController.ModelState.AddModelError("Title", "NewErrorMsg");
            _albumsController.Create(album);
            _albumRepository.DidNotReceive().Insert(Arg.Any<Album>());
            _unitOfWork.DidNotReceive().Save();
        }

        [TestMethod]
        public void CreatePostActionRepoReceivedInsertCallAndUnitOfWorkSaveCall()
        {
            var album = new NewAlbumViewModel();
            _albumsController.Create(album);
            _albumRepository.Received().Insert(Arg.Any<Album>());
            _unitOfWork.Received().Save();
        }

        [TestMethod]
        public void AlbumActionReturnsHttpNotFoundWhenAlbumDoesNotExist()
        {
            var result = _albumsController.Album(10);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void ActionCheckAlbumTitleReturnsFalseWhenAlbumExists()
        {
            var result = _albumsController.CheckAlbumTitle("New1") as JsonResult;
            Assert.IsNotNull(result);
            Assert.IsFalse((bool) result.Data);
        }

        [TestMethod]
        public void EditPostActionDoesNotCreateAlbumWhenHasModelError()
        {
            var album = new AlbumViewModel();
            _albumsController.ModelState.AddModelError("Title", "NewErrorMsg");
            _albumsController.Edit(album);
            _albumRepository.DidNotReceive().Update(Arg.Any<Album>());
            _unitOfWork.DidNotReceive().Save();
        }

        [TestMethod]
        public void EditPostActionRepoReceivedUpdateCallAndUnitOfWorkSaveCall()
        {
            var album = new AlbumViewModel();
            _albumsController.Edit(album);
            _albumRepository.Received().Update(Arg.Any<Album>());
            _unitOfWork.Received().Save();
        }

        [TestMethod]
        public void EditPostActionHasRedirectToAlbumView()
        {
            var album = new AlbumViewModel();
            var result = _albumsController.Edit(album) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Album", result.RouteValues["action"]);
        }

        [TestMethod]
        public void DeleteActionReturnsHttpBadRequestWhenAlbumIsNull()
        {
            var result = _albumsController.Delete(null);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(400, ((HttpStatusCodeResult)result).StatusCode);
        }
    }
}
