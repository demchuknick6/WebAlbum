using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebAlbum.Web.Models.Photos;

namespace WebAlbum.Web.Areas.Users.Models
{
    public class SearchAlbumViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int AlbumId { get; set; }

        [Display(Name = "Album Title")]
        public string AlbumTitle { get; set; }

        //[ScaffoldColumn(false)]
        public string UserId { get; set; }

        public UserViewModel User { get; set; }

        public ICollection<PhotoViewModel> Photos { get; set; }
    }
}