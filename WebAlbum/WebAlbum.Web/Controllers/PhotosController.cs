using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
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
        private const string ErrorMsg = "An error has occurred while processing your request. ";


        public PhotosController(IUnitOfWork unitOfWork, 
            IGenericRepository<Photo> photoRepository, IMapper mapper)
        {
            _photoRepository = photoRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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
                photo.DateCreated = DateTime.UtcNow;
                photo.FileName = model.PhotoTitle + DateTime.UtcNow.ToString("s") 
                                                  + extension;
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
                var photos = _mapper.Map<IEnumerable<Photo>, List<PhotoViewModel>>
                (_photoRepository
                    .AsQueryable().Where(u => u.AlbumId == albumId)
                    .OrderBy(p => p.DateCreated).ToList()).ToList();
                return PartialView(photos);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            try
            {
                var photo = _mapper.Map<Photo, PhotoViewModel>(_photoRepository.GetByKey(id));
                if (photo == null)
                    return HttpNotFound();
                return View(photo);
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
                return RedirectToAction("Index","Albums");
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
                if (photo.AlbumId == albumId)
                {
                    _photoRepository.DeleteByKey(id);
                    _unitOfWork.Save();
                    TempData["result"] = $"Photo {photo.PhotoTitle} has been deleted.";
                }
                else
                    TempData["result"] = ErrorMsg;
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
            }

            return RedirectToAction("_Photos", new {albumId = albumId});
        }

        // GET: Photos
        /*public ActionResult Index()
        {
            return View();
        }

        // GET: Photos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        // GET: Photos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Photos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }*/
    }
}
