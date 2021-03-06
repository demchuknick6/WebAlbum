﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using WebAlbum.DomainModel;
using WebAlbum.DomainServices;
using WebAlbum.Web.Models.Photos;

namespace WebAlbum.Web.Controllers
{
    [Authorize]
    public class PhotosController : Controller
    {
        private readonly IGenericRepository<Photo> _photoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public Func<string> GetUserId;
        private const string ErrorMsg = "An error has occurred while processing your request. ";


        public PhotosController(IUnitOfWork unitOfWork, 
            IGenericRepository<Photo> photoRepository, IMapper mapper)
        {
            _photoRepository = photoRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            GetUserId = () => System.Web.HttpContext.Current.User.Identity.GetUserId();
        }

        [ChildActionOnly]
        public ActionResult Add(int? albumId)
        {
            if (albumId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var photo = new NewPhotoViewModel {AlbumId = (int) albumId};
                return PartialView(photo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(NewPhotoViewModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid || file == null)
            {
                TempData["result"] = ErrorMsg;
                return RedirectToAction("Album", "Albums",
                    new {id = model.AlbumId});
            }

            var extensions = new List<string>() {".jpg", ".jpeg", ".png", ".bmp", ".gif"};
            try
            {
                var extension = Path.GetExtension(file.FileName)?.ToLower();
                if (!extensions.Contains(extension))
                {
                    TempData["result"] = "Only jpg, jpeg, gif, png and bmp formats are acceptable.";
                    return RedirectToAction("Album", "Albums",
                        new { id = model.AlbumId });
                }

                var photo = _mapper.Map<Photo>(model);
                photo.FileName = model.PhotoTitle +
                                 DateTime.UtcNow.ToString("s") + extension;                                            
                photo.Content = new byte[file.ContentLength];
                file.InputStream.Read(photo.Content, 0, file.ContentLength);
                _photoRepository.Insert(photo);
                _unitOfWork.Save();
                TempData["result"] = $"Photo {model.PhotoTitle} has been saved.";
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
            }

            return RedirectToAction("Album", "Albums",
                new { id = model.AlbumId });
        }

        [ChildActionOnly]
        public ActionResult _Photos(int? albumId)
        {
            if (albumId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            try
            {
                var userId = GetUserId();
                var photos = _mapper.Map<IEnumerable<Photo>, List<PhotoViewModel>>
                (_photoRepository
                    .AsQueryable().Where(u => u.AlbumId == albumId && u.Album.UserId == userId)
                    .OrderBy(p => p.PhotoId).ToList()).ToList();
                return PartialView(photos);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        #region EditPhotoRegion    
        /*private IEnumerable<AlbumViewModel> OwnAlbums()
        {
            var albums = new List<AlbumViewModel>();
            var userId = GetUserId();
            albums.AddRange(_mapper.Map<IEnumerable<Album>, IEnumerable<AlbumViewModel>>
            (_photoRepository.AsQueryable().Where(x => x.Album.UserId == userId)
                .Select(x => x.Album).Distinct().ToList()));
            return albums;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PhotoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AlbumId = new SelectList(OwnAlbums(), 
                    "AlbumId", "AlbumTitle", model.AlbumId);
                return View(model);
            }
            try
            {
                var photo = _mapper.Map<Photo>(model);
                _photoRepository.Update(photo);
                _unitOfWork.Save();
                TempData["result"] = $"Photo {model.PhotoTitle} has been edited.";
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
            }
            return RedirectToAction("Album", "Albums", 
                new { id = model.AlbumId });
        }*/
        #endregion

        public ActionResult Edit(int? id, int? albumId)
        {
            if (id == null || albumId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            try
            {
                var photo = _photoRepository.GetByKey(id);
                if (photo == null)
                    return HttpNotFound();
                if (photo.AlbumId != albumId || photo.Album.UserId != GetUserId())
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                var model = _mapper.Map<Photo, PhotoViewModel>(photo);
                //ViewBag.AlbumId = new SelectList(OwnAlbums(),
                //"AlbumId", "AlbumTitle", model.AlbumId);
                return View("Edit", model);
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
                return RedirectToAction("Index", "Albums");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, int? albumId)
        {
            if (id == null || albumId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            try
            {
                var photo = _photoRepository.GetByKey(id);
                if (photo == null)
                    return HttpNotFound();
                if (photo.AlbumId != albumId || photo.Album.UserId != GetUserId())
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

                _photoRepository.DeleteByKey(id);
                _unitOfWork.Save();
                TempData["result"] = $"Photo {photo.PhotoTitle} has been deleted.";
                return RedirectToAction("Album", "Albums",
                    new { id = albumId });
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
            }

            return RedirectToAction("Index", "Albums");
        }
    }
}
