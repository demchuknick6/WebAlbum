using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using WebAlbum.DomainModel;
using WebAlbum.DomainServices;
using WebAlbum.Web.Controllers;
using WebAlbum.Web.Models.Photos;

namespace WebAlbum.Tests.Controllers
{
    [TestClass]
    public class PhotosControllerTest
    {
        private PhotosController _photosController;
        private IGenericRepository<Photo> _photoRepository;
        private IUnitOfWork _unitOfWork;
        private HttpPostedFileBase _file;

        [TestInitialize]
        public void SetupContext()
        {
            var substituteDbSet = Substitute.For<IDbSet<Photo>>();
            var photosList = new List<Photo>()
            {
                new Photo()
                {
                    AlbumId = 1, PhotoTitle = "New1", PhotoId = 1, Album = new Album()
                        {AlbumId = 1, AlbumTitle = "New1", Public = true, UserId = "NewUserId1"}
                },
                new Photo()
                {
                    AlbumId = 1, PhotoTitle = "New2", PhotoId = 2, Album = new Album()
                        {AlbumId = 1, AlbumTitle = "New1", Public = true, UserId = "NewUserId1"}
                },
                new Photo()
                {
                    AlbumId = 2, PhotoTitle = "New3", PhotoId = 3, Album = new Album()
                        {AlbumId = 2, AlbumTitle = "New2", Public = true, UserId = "NewUserId2"}
                }
            }.AsQueryable();

            substituteDbSet.Provider.Returns(photosList.Provider);
            substituteDbSet.Expression.Returns(photosList.Expression);
            substituteDbSet.ElementType.Returns(photosList.ElementType);
            substituteDbSet.GetEnumerator().Returns(photosList.GetEnumerator());

            _photoRepository = Substitute.For<IGenericRepository<Photo>>();
            _photoRepository.AsQueryable().Returns(substituteDbSet.AsQueryable());
            _unitOfWork = Substitute.For<IUnitOfWork>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Photo, NewPhotoViewModel>().ReverseMap();
                cfg.CreateMap<Photo, PhotoViewModel>()
                    .ForMember(dest => dest.Content,
                        o => o.MapFrom(
                            src => "data:image/png;base64," + 
                                   Convert.ToBase64String(src.Content.ToArray())));

                cfg.CreateMap<PhotoViewModel, Photo>()
                    .ForMember(x => x.Content, o => o.Ignore());
            });

            var mapper = config.CreateMapper();

            _photosController = new PhotosController(_unitOfWork, _photoRepository, mapper)
            {
                GetUserId = () => "NewUserId1"
            };

            _file = Substitute.For<HttpPostedFileBase>();
        }

        [TestMethod]
        public void ActionAddIsPartialViewResultAndNotNull()
        {
            var result = _photosController.Add(1) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }

        [TestMethod]
        public void ActionAddHasPhotoViewModel()
        {
            var result = _photosController.Add(1) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.ViewData.Model,
                typeof(NewPhotoViewModel));
        }

        [TestMethod]
        public void AddPostActionDoesNotAddPhotoIfModelErrorAndRedirectsToAlbumView()
        {
            var photo = new NewPhotoViewModel();
            _photosController.ModelState.AddModelError("Title", "NewErrorMsg");
            var result = _photosController.Add(photo, _file) as RedirectToRouteResult;

            _photoRepository.DidNotReceive().Insert(Arg.Any<Photo>());
            _unitOfWork.DidNotReceive().Save();

            Assert.IsNotNull(result);
            Assert.AreEqual("Album", result.RouteValues["action"]);
        }

        [TestMethod]
        public void AddPostActionDoesNotAddPhotoIfFileHasWrongFormat()
        {
            var photo = new NewPhotoViewModel();

            using (var stream = new MemoryStream())
            using (var bmp = new Bitmap(1, 1))
            {
                var graphics = Graphics.FromImage(bmp);
                graphics.FillRectangle(Brushes.Black, 0, 0, 1, 1);
                bmp.Save(stream, ImageFormat.Tiff);
                //Set up wrong file format
                _file.FileName.Returns("someFileName.tiff");
                _file.InputStream.Returns(stream);
                _file.ContentLength.Returns((int)stream.Length);

                _photosController.Add(photo, _file);
                _photoRepository.DidNotReceive().Insert(Arg.Any<Photo>());
                _unitOfWork.DidNotReceive().Save();
            }
        }

        [TestMethod]
        public void AddPostActionRepoReceivedInsertCallAndUnitOfWorkSaveCall()
        {
            var photo = new NewPhotoViewModel();

            using (var stream = new MemoryStream())
            using (var bmp = new Bitmap(1, 1))
            {
                var graphics = Graphics.FromImage(bmp);
                graphics.FillRectangle(Brushes.Black, 0, 0, 1, 1);
                bmp.Save(stream, ImageFormat.Jpeg);
                //Set up correct file format
                _file.FileName.Returns("testFileName.jpeg");
                _file.InputStream.Returns(stream);
                _file.ContentLength.Returns((int)stream.Length);

                _photosController.Add(photo, _file);
                _photoRepository.Received().Insert(Arg.Any<Photo>());
                _unitOfWork.Received().Save();
            }
        }


        [TestMethod]
        public void Action_PhotosReturnsTwoPhotosTypeOfPhotoViewModel()
        {
            var result = _photosController._Photos(1) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.ViewData.Model,
                typeof(IEnumerable<PhotoViewModel>));

            var photos = (List<PhotoViewModel>)result.ViewData.Model;
            Assert.AreEqual(2, photos.Count());
            Assert.AreEqual("New1", photos[0].PhotoTitle);
            Assert.AreEqual("New2", photos[1].PhotoTitle);
        }

        [TestMethod]
        public void Action_PhotosReturnsHttpBadRequestIfPhotoIsNull()
        {
            var result = _photosController._Photos(null);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(400, ((HttpStatusCodeResult)result).StatusCode);
        }
    }
}
