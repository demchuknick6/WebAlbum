using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using WebAlbum.DomainModel;
using WebAlbum.DomainServices;
using WebAlbum.Web.Models.Albums;

namespace WebAlbum.Web.Controllers
{
    [Authorize]
    public class AlbumsController : Controller
    {
        private readonly IGenericRepository<Album> _albumRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _userId;
        private const string ErrorMsg = "An error has occurred while processing your request. ";

        public AlbumsController(IUnitOfWork unitOfWork, 
            IGenericRepository<Album> albumRepository, IMapper mapper)
        {
            _albumRepository = albumRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Albums()
        {
            try
            {
                var albumsList = new AlbumListViewModel
                {
                    Albums = _mapper.Map<IEnumerable<Album>, List<AlbumViewModel>>
                    (_albumRepository
                        .AsQueryable().Where(u => u.UserId == _userId)
                        .OrderBy(p => p.DateCreated).ToList()).ToList()
                };
                return PartialView(albumsList);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [ChildActionOnly]
        public ActionResult Create()
        {
            var model = new NewAlbumViewModel() { Public = true };
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewAlbumViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["result"] = ErrorMsg;
            }
            try
            {
                var album = _mapper.Map<Album>(model);
                album.DateCreated = DateTime.UtcNow;
                if (_userId != null)
                    album.UserId = _userId;
                _albumRepository.Insert(album);
                _unitOfWork.Save();
                TempData["result"] = $"Album {model.AlbumTitle} has been added.";
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
            }

            return RedirectToAction("_Albums");
        }

        public ActionResult Album(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            try
            {
                var album = _mapper.Map<Album, AlbumViewModel>(_albumRepository.GetByKey(id));
                if (album == null)
                    return HttpNotFound();
                if (_userId != album.UserId)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                return View(album);
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public JsonResult CheckAlbumTitle(string albumTitle)
        {
            return Json(!_albumRepository.AsQueryable()
                    .Where(u => u.UserId == _userId)
                    .Any(lo => lo.AlbumTitle.Equals(albumTitle)),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int? id)
        {
            return Album(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AlbumViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var album = _mapper.Map<Album>(model);
                album.UserId = _userId;
                _albumRepository.Update(album);
                _unitOfWork.Save();
                TempData["result"] = $"Album {model.AlbumTitle} has been edited.";       
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
            }

            return RedirectToAction("Album", new {id = model.AlbumId});

        }

        public ActionResult Delete(int? id)
        {
            return Album(id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _albumRepository.DeleteByKey(id);
                _unitOfWork.Save();
                TempData["result"] = "Album has been deleted.";
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
