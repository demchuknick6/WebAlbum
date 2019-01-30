using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using WebAlbum.DomainModel;
using WebAlbum.DomainServices;
using WebAlbum.Web.Areas.Users.Models;
using WebAlbum.Web.Models.Photos;

namespace WebAlbum.Web.Areas.Users.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IGenericRepository<Album> _albumRepository;
        private readonly IGenericRepository<Photo> _photoRepository;
        private readonly IMapper _mapper;
        private const string ErrorMsg = "An error has occurred while processing your request. ";

        public HomeController(IGenericRepository<Album> albumRepository, 
            IGenericRepository<Photo> photoRepository, IMapper mapper)
        {
            _albumRepository = albumRepository;
            _photoRepository = photoRepository;
            _mapper = mapper;
        }

        public ActionResult Index(string condition = null, string searchString = null)
        {
            if (!Request.IsAjaxRequest())
                return View("Index");
            try
            {
                var list = _albumRepository
                    .AsQueryable().Where(a => a.Public).OrderBy(p => p.DateCreated).ToList();
                List<SearchAlbumViewModel> data;
                if (string.IsNullOrEmpty(searchString))
                {
                    data = _mapper.Map<List<Album>, List<SearchAlbumViewModel>>(list);
                    return PartialView("_SearchResults", data);
                }
                if (condition == "user")
                {
                    data = _mapper.Map<List<Album>, List<SearchAlbumViewModel>>(_albumRepository
                        .AsQueryable().Where(a => a.User.UserName.ToLower()
                        .Contains(searchString.ToLower()) && a.Public).ToList());
                }
                else
                {
                    data = _mapper.Map<List<Album>, List<SearchAlbumViewModel>>(list.Where(a =>
                            a.AlbumTitle.ToLower().Contains(searchString.ToLower()) && a.Public)
                        .ToList());
                }
                if (data.Count != 0)
                    return PartialView("_SearchResults", data);

                TempData["searchString"] = searchString;
                return PartialView("_NoResults");
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
            }

            return View("Index");
        }

        public ActionResult Albums(string userName)
        {
            try
            {
                var userAlbums = new UserViewModel()
                {
                    UserName = userName,
                    Albums = _mapper.Map<IEnumerable<Album>, List<SearchAlbumViewModel>>
                    (_albumRepository.AsQueryable()
                        .Where(u => u.User.UserName == userName && u.Public)
                        .OrderBy(p => p.DateCreated).ToList()).ToList()
                };
                return View(userAlbums);
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
                return RedirectToAction("Index", "Home");
            }   
        }

        public ActionResult Photos(string userName, string albumTitle)
        {
            try
            {
                var photoList = new PhotoListViewModel()
                {
                    UserName = userName,
                    AlbumTitle = albumTitle,
                    Photos = _mapper.Map<IEnumerable<Photo>, List<PhotoViewModel>>
                    (_photoRepository.AsQueryable()
                        .Where(u => u.Album.AlbumTitle == albumTitle &&
                                    u.Album.User.UserName == userName && u.Album.Public)
                        .OrderBy(p => p.DateCreated).ToList()).ToList()
                };
                return View(photoList);
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}